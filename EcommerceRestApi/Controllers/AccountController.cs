using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Helpers.Static;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly IUserService _service;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            IUserService service,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _service = service;
            _tokenService = tokenService;
        }

        // POST: api/account/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseModel)),
            ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseModel))]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList()
                });
            }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginVM.Password))
            {
                return Unauthorized(new ResponseModel { Message = "Invalid email!" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ResponseModel { Message = "Wrong password!" });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(600)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

            user.IsAuthenticated = true;
            await _userManager.UpdateAsync(user);

            return Ok(new ResponseModel { Message = "Login successful." });
        }


        // POST: api/account/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseModel)),
            ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ResponseModel))]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid input data.",
                    Errors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList()
                });
            }

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                return Conflict(new ResponseModel { Message = "This email address is already in use." });
            }

            var newUser = await DbFuncs.GetApplicationUserObjForRegister(registerVM, _context);


            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!newUserResponse.Succeeded)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "User registration failed.",
                    Errors = newUserResponse.Errors.Select(e => e.Description)
                });
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            newUser.IsAdmin = true;
            newUser.IsAdmin = false;
            await _userManager.UpdateAsync(newUser);

            return Ok(new ResponseModel { Message = "User registered successfully." });
        }

        [HttpGet("get-current-user")]
        public async Task<IActionResult> GetCurrentAuthenticatedUser()
        {
            // 1. Get authenticated user details
            if (User.Identity.IsAuthenticated)
            {
                ApplicationUser? user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    return Ok(user);
                }
            }
            return NotFound();
        }

        // POST: api/account/logout
        [Authorize]
        [HttpPost("logout")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseModel))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Session.Clear();
            return Ok();
        }

        // PUT: api/account/users/5
        [Authorize(Roles = UserRoles.User)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ResponseModel))]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseModel
                {
                    Message = "Invalid model data",
                    Errors = ModelState.Values
                                             .SelectMany(v => v.Errors)
                                             .Select(e => e.ErrorMessage)
                                             .ToList()
                });
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }


            await _service.UpdateUserAsync(id, model);
            return NoContent();
        }

        // DELETE: api/account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _service.GetUserByIDAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _service.DeleteUserAsync(id);
            return NoContent();
        }

        // GET: api/account/access-denied
        [HttpGet("access-denied")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ResponseModel))]
        public IActionResult AccessDenied()
        {
            return Unauthorized(new ResponseModel { Message = "Access denied. You do not have permission to access this resource." });
        }
    }
}
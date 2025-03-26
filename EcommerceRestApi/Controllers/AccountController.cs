using EcommerceRestApi.Helpers.Data.Auth;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Helpers.Static;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Common;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data." });
            }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginVM.Password))
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { Message = "Login failed. Please try again." });
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

            return Ok(new { Message = "Login successful." });
        }


        // POST: api/account/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterViewModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid input data." });
            }

            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                return Conflict(new { Message = "This email address is already in use." });
            }

            var newUser = await DbFuncs.GetApplicationUserObjForRegister(registerVM, _context);


            //await _context.Customers.AddAsync(newUser.Customers.First());


            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!newUserResponse.Succeeded)
            {
                return BadRequest(new { Message = "User registration failed.", Errors = newUserResponse.Errors });
            }

            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

            return Ok(new { Message = "User registered successfully.", User = await _context.Users.FirstAsync(u => u.Id == newUser.Id) });
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
        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            //await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Session.Clear();
            return Ok(new { Message = "Logout successful." });
        }

        // PUT: api/products/5
        [Authorize(Roles = UserRoles.User)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserUpdateVM))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
        public IActionResult AccessDenied()
        {
            return Unauthorized(new { Message = "Access denied. You do not have permission to access this resource." });
        }
    }
}
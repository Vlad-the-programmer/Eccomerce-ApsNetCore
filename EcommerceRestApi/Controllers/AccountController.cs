using EcommerceRestApi.Helpers.Data.AuthVms;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IAccountService _accountService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            IUserService service,
            IAccountService accountService,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _service = service;
            _accountService = accountService;
            _tokenService = tokenService;
        }

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

            //var clientType = Request.Headers["X-Client-Type"].ToString();
            //var isMobileApp = clientType.Equals("mobile", StringComparison.OrdinalIgnoreCase);

            //var response = await _accountService.Login(loginVM, isMobileApp);
            //if (response.Success)
            //{
            //    return Ok(response);
            //}
            //return BadRequest(response);
            var user = await _userManager.Users
                .Include(u => u.Customers)
                .FirstOrDefaultAsync(u => u.Email == loginVM.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginVM.Password))
            {
                return Unauthorized(new ResponseModel { Message = "Invalid email or password!" });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new ResponseModel { Message = "Login failed!" });
            }

            var clientType = Request.Headers["X-Client-Type"].ToString();
            var isMobileApp = clientType.Equals("mobile", StringComparison.OrdinalIgnoreCase);

            // Get the CurrentUserViewModel from the already loaded user
            var userViewModel = (CurrentUserViewModel)user;

            var jwtToken = _tokenService.GenerateJwtToken(_configuration, user);
            if (isMobileApp)
            {

                user.IsAuthenticated = true;
                await _userManager.UpdateAsync(user);

                return Ok(new LoginResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = jwtToken,
                    User = userViewModel
                });
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            var customer = user.Customers.FirstOrDefault();

            // Set cookie for web app
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("CustomerId", customer != null ? customer.Id.ToString() : string.Empty)
            };

            claims.AddRange(userClaims);

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

            var permissions = userClaims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful.",
                User = userViewModel,
                Permissions = permissions
            });
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

            ResponseModel response = await _accountService.Register(registerVM);

            if (response.Errors.Count > 0)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("get-current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentAuthenticatedUser()
        {
            // Try JWT first (mobile app)
            var jwtUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If JWT not found, try cookie (web app)
            if (string.IsNullOrEmpty(jwtUserId))
            {
                var cookieUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(cookieUserId))
                {
                    return Unauthorized(new ResponseModel { Message = "User not authenticated" });
                }

                var cookieUser = await _userManager.Users
                    .Include(u => u.Customers)
                    .FirstOrDefaultAsync(u => u.Id == cookieUserId);

                if (cookieUser == null)
                    return NotFound(new ResponseModel { Message = "User not found" });

                return Ok((CurrentUserViewModel)cookieUser);
            }

            // JWT authentication
            var user = await _userManager.Users
                .Include(u => u.Customers)
                .FirstOrDefaultAsync(u => u.Id == jwtUserId);

            if (user == null)
                return NotFound(new ResponseModel { Message = "User not found" });

            return Ok(OrderCustomerDTO.ToVM(user.Customers.FirstOrDefault(), _userManager));
        }

        // POST: api/account/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return Ok();
        }

        [HttpGet("get-update-user-model/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUpdateUserModel(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Current user rest: {currentUserId}");

            if (string.IsNullOrEmpty(id))
            {
                return NotFound("User not found ");
            }

            try
            {
                var user = await _service.GetUserByIDAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                var userUpdateVM = await _accountService.GetUpdateUserModel(user);

                return Ok(userUpdateVM);

            }
            catch (Exception e)
            {
                return BadRequest(new ResponseModel { Message = e.Message });
            }
        }

        // PUT: api/account/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateVM model)
        {
            if (User.Identity == null || User.FindFirstValue(ClaimTypes.NameIdentifier) != id)
            {
                return Forbid();
            }

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
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            if (User.Identity == null || User.FindFirstValue(ClaimTypes.NameIdentifier) != id)
            {
                return Forbid();
            }

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

        [HttpGet("user-profile")]
        //[Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var profile = await _service.GetUserProfile(userId);

                if (profile == null)
                {
                    return NotFound(new ResponseModel { Message = "User profile not found." });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }
        }
    }
}
using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.AuthVms;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceRestApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AccountService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context,
            ITokenService tokenService,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
        }


        public async Task<UserUpdateVM> GetUpdateUserModel(ApplicationUser user)
        {
            var customer = user.Customers?.FirstOrDefault();
            if (customer == null)
            {
                var userUpdateModelWithoutCustomer = new UserUpdateVM
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Username = user.UserName,
                    Nip = null,
                    State = null,
                    City = null,
                    Street = null,
                    HouseNumber = null,
                    FlatNumber = null,
                    PostalCode = null,
                    CountryName = null,
                };
                return userUpdateModelWithoutCustomer;
            }

            var address = await _context.Addresses
                .Include(a => a.Country)
                .Where(a => a.CustomerId == customer.Id)
                .FirstOrDefaultAsync();

            var userUpdateModel = new UserUpdateVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Username = user.UserName,
                Nip = customer.Nip,
                State = address?.State,
                City = address?.City,
                Street = address?.Street,
                HouseNumber = address?.HouseNumber,
                FlatNumber = address?.FlatNumber,
                PostalCode = address?.PostalCode,
                CountryName = address?.Country?.CountryName,
            };

            return userUpdateModel;
        }

        public async Task<LoginResponse> Login(LoginViewModel loginVM, bool isMobileApp)
        {
            var user = await _userManager.Users
                .Include(u => u.Customers)
                .FirstOrDefaultAsync(u => u.Email == loginVM.Email && u.IsActive);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginVM.Password))
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password!",
                    User = (CurrentUserViewModel)user,
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
            if (!result.Succeeded)
            {
                new LoginResponse
                {
                    Success = false,
                    Message = "Login failed!",
                    User = (CurrentUserViewModel)user,
                };
            }
            var userViewModel = (CurrentUserViewModel)user;

            var jwtToken = _tokenService.GenerateJwtToken(_configuration, user);
            if (isMobileApp)
            {

                user.IsAuthenticated = true;
                await _userManager.UpdateAsync(user);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful.",
                    Token = jwtToken,
                    User = userViewModel
                };
            }

            var userClaims = await _userManager.GetClaimsAsync(user);

            // Set cookie for web app
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("CustomerId", userViewModel.CustomerId?.ToString() ?? ""),
                new Claim(ClaimTypes.Role, user.Role)
            };

            claims.AddRange(userClaims);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(12)
            };

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            var guestCartId = _httpContextAccessor.HttpContext.Session.GetString("CartId");
            if (!string.IsNullOrEmpty(guestCartId))
            {
                var shoppingCart = new ShoppingCart(
                    _context,
                    _httpContextAccessor.HttpContext.Session,
                    _httpContextAccessor);

                await shoppingCart.MergeCartWithUser(user.Id, guestCartId);
            }

            user.IsAuthenticated = true;
            await _userManager.UpdateAsync(user);

            var permissions = userClaims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            return new LoginResponse
            {
                Success = true,
                Message = "Login successful.",
                User = userViewModel,
                Permissions = permissions
            };
        }

        public async Task Logout()
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<ResponseModel> Register(RegisterViewModel registerVM)
        {
            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                return new ResponseModel
                {
                    Message = "This email address is already in use.",
                    Errors = new List<string>().Append("This email address is already in use.").ToList()
                };
            }

            var newUser = await DbFuncs.GetApplicationUserObjForRegister(registerVM, _context);

            var newUserResponse = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (!newUserResponse.Succeeded)
            {
                return new ResponseModel
                {
                    Message = "User registration failed.",
                    Errors = newUserResponse.Errors.Select(e => e.Description).ToList()
                };
            }

            if (newUser.Email.Contains("admin"))
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);

                newUser.IsAdmin = true;
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);

                newUser.IsAdmin = false;
            }

            newUser.IsActive = true;

            await _userManager.UpdateAsync(newUser);

            return new ResponseModel { Message = "User registered successfully." };
        }
    }
}

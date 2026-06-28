using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.ResponseModels;
using EcommerceWebApp.Models.UpdateViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountController(ILogger<AccountController> logger,
                                    IApiService apiService,
                                    IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl = null)
        {
            try
            {
                var response = await _apiService.PostDataAsync(
                    GlobalConstants.LoginEndpoint,
                    JsonSerializer.Serialize(loginVM)
                );

                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(response, GlobalConstants.JsonSerializerOptions);

                if (loginResponse.Success)
                {
                    HttpContext.Session.SetString("CurrentUser", JsonSerializer.Serialize(loginResponse.User));

                    var user = loginResponse.User;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId),
                        new Claim(ClaimTypes.Name, user.FullName ?? "User"),
                        new Claim(ClaimTypes.Email, user.Email ?? ""),
                        new Claim("UserName", user.UserName ?? ""),
                        new Claim("CustomerId", user.CustomerId?.ToString() ?? ""),
                        new Claim(ClaimTypes.Role, user.Role ?? "User")
                    };

                    foreach (var permission in loginResponse.Permissions)
                    {
                        claims.Add(new Claim("Permission", permission));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    TempData["Success"] = "Login successful!";

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    TempData["Error"] = loginResponse.Message ?? "Login failed";
                    return View(loginVM);
                }
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                return View(loginVM);
            }
        }

        [HttpGet("register")]
        public async Task<IActionResult> Register()
        {
            ViewBag.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(
                                    GlobalConstants.CountriesEndpoint, _apiService);

            return View(new RegisterViewModel());
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            try
            {
                var response = await _apiService.PostDataAsync(
                                    GlobalConstants.RegisterEndpoint,
                                    JsonSerializer.Serialize<RegisterViewModel>(registerVM));
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                ViewBag.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(
                                GlobalConstants.CountriesEndpoint, _apiService);
                return View(registerVM);
            }

            return View("RegisterCompleted");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _apiService.PostDataAsync(GlobalConstants.LogoutEndpoint);

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction("Index", "Products");
        }

        [HttpGet("access-denied")]
        public IActionResult AccessDenied(string ReturnUrl)
        {
            try
            {
                var response = _apiService.GetDataAsync(
                                GlobalConstants.AccessDeniedEndpoint);
            }
            catch (HttpRequestException e)
            {
                TempData["Error: "] = e.Message;
            }
            return View();
        }

        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            Console.WriteLine(User);
            var updateModel = await AccountEndpointsHelperFuncs.GetUserUpdateModelObj(
                $"{GlobalConstants.GetUserUpdateModelEndpoint}/{id}", _apiService);

            if (updateModel == null)
            {
                return View("NotFound");
            }

            ViewBag.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);

            return View(updateModel);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserUpdateVM updatedUser)
        {

            if (updatedUser.Id != id) return RedirectToAction("AccessDenied");

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.UserUpdateEndpoint}/{id}",
                                                    JsonSerializer.Serialize(updatedUser));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                ViewBag.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);

                return View(updatedUser);
            }
            return RedirectToAction("Index", "Products");
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.UserDeleteEndpoint}/{id}");
                await Logout();
                TempData["Success"] = "Account deleted successfully";
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
            }

            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var userDetails = await AccountEndpointsHelperFuncs.GetUserProfile(
                                    $"{GlobalConstants.GetUserProfile}", _apiService);
            if (userDetails == null)
            {
                return View("NotFound");
            }
            return View(userDetails);
        }
    }
}
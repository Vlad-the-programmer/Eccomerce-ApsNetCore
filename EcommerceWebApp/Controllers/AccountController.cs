using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.UpdateViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
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

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl = null)
        {
            try
            {
                var response = await _apiService.PostDataAsync(
                    GlobalConstants.LoginEndpoint,
                    JsonSerializer.Serialize(loginVM)
                );
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                return View(loginVM);
            }

            TempData["Success"] = "Login successful!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Products");
        }
        public async Task<IActionResult> Register()
        {
            return View(new RegisterViewModel(
                await CountriesEndpointsHelperFuncs.GetCountriesNames(
                    GlobalConstants.CountriesEndpoint, _apiService))
            );
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerVM)
        {
            registerVM.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService) ?? new List<string>();

            //if (!ModelState.IsValid) return View(registerVM);

            try
            {
                var response = await _apiService.PostDataAsync(
                                    GlobalConstants.RegisterEndpoint,
                                    JsonSerializer.Serialize<RegisterViewModel>(registerVM));
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                return View(registerVM);
            }

            return View("RegisterCompleted");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _apiService.PostDataAsync(GlobalConstants.LogoutEndpoint);
                HttpContext.Session.Remove("CurrentUser");
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                //return View();
            }

            return RedirectToAction("Index", "Products");
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var userId = Int32.Parse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));
            if (userId != id) return RedirectToAction("AccessDenied");

            return View(User);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UserUpdateVM updatedUser)
        {
            var userId = Int32.Parse(_httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId != id) return RedirectToAction("AccessDenied");

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.UserUpdateEndpoint}/{id}",
                                                    JsonSerializer.Serialize(updatedUser));
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Error = ex.Message;

                return View(updatedUser);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.UserDeleteEndpoint}/{id}");
                //HttpContext.Session.Remove("auth_token");
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                //return View();
            }

            return RedirectToAction("Index", "Products");
        }
    }
}

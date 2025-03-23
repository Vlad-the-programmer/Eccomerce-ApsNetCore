using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IApiService _apiService;
        public AccountController(ILogger<AccountController> logger, 
                                    IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            try
            {
                var response = await _apiService.PostDataAsync(
                    GlobalConstants.LoginEndpoint, 
                    JsonSerializer.Serialize(loginVM)
                );
            } catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                return View(loginVM);
            }
            
            

            TempData["Success"] = "Login successful!";
            return RedirectToAction("Index", "Home");
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
            registerVM.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);

            if (!ModelState.IsValid) return View(registerVM);

            //var user = new ApplicationUserViewModel();
            try
            {
                var response = await _apiService.PostDataAsync( 
                                    GlobalConstants.RegisterEndpoint, 
                                    JsonSerializer.Serialize<ApplicationUserViewModel>(registerVM));
                //user = JsonSerializer.Deserialize<ApplicationUserViewModel>(response); // Deserialize from string 

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
            } catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            try
            {
                var response = _apiService.GetDataAsync(
                                GlobalConstants.AccessDeniedEndpoint);
            } catch(HttpRequestException e)
            {
                TempData["Error: "] = e.Message;
            }
            return View();
        }
    }
}

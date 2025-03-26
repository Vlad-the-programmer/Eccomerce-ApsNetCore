using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.ResponseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            //if (!ModelState.IsValid) return View(loginVM);
            TokenResponse? tokenObj = new TokenResponse();
            try
            {
                var response = await _apiService.PostDataAsync(
                    GlobalConstants.LoginEndpoint, 
                    JsonSerializer.Serialize(loginVM)
                );

                tokenObj = JsonSerializer.Deserialize<TokenResponse>(response, GlobalConstants.JsonSerializerOptions);

                HttpContext.Session.SetString("auth_token", tokenObj.Token);
                JWTAuth.SetUserFromToken(tokenObj.Token, _httpContextAccessor);
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
            registerVM.Countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService) ?? new List<string>();

            //if (!ModelState.IsValid) return View(registerVM);

            try
            {
                var response = await _apiService.PostDataAsync( 
                                    GlobalConstants.RegisterEndpoint, 
                                    JsonSerializer.Serialize<ApplicationUserViewModel>(registerVM));
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
                HttpContext.Session.Remove("auth_token");
            } catch (HttpRequestException e)
            {
                TempData["Error"] = e.Message;
                //return View();
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

using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("shopcoinsettings")]
    public class ShopCoinSettingsController : Controller
    {
        private readonly IApiService _apiService;

        public ShopCoinSettingsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetDataAsync("api/shopcoinsettings");
                var settings = JsonSerializer.Deserialize<ShopCoinSettingsDto>(response, GlobalConstants.JsonSerializerOptions);

                return View(settings);
            }
            catch (HttpRequestException ex)
            {
                return View(new ShopCoinSettingsDto());
            }
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new ShopCoinSettingsDto());
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(ShopCoinSettingsDto dto)
        {
            try
            {
                await _apiService.PostDataAsync("api/shopcoinsettings", JsonSerializer.Serialize(dto));
                TempData["Success"] = "Settings created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet("update")]
        public async Task<IActionResult> Edit()
        {
            try
            {
                var response = await _apiService.GetDataAsync("api/shopcoinsettings");
                var settings = JsonSerializer.Deserialize<ShopCoinSettingsDto>(response, GlobalConstants.JsonSerializerOptions);
                return View(settings);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Shop Settings are not configured yet";
                return RedirectToAction("Index");
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Edit(ShopCoinSettingsDto dto)
        {
            try
            {
                await _apiService.UpdateDataAsync("api/shopcoinsettings", JsonSerializer.Serialize(dto));
                TempData["Success"] = "Settings updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}

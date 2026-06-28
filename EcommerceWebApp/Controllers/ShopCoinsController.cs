using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    public class ShopCoinsController : Controller
    {
        private readonly IApiService _apiService;

        public ShopCoinsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _apiService.GetDataAsync("api/shopcoins/history");
                var history = JsonSerializer.Deserialize<List<ShopCoinTransactionHistoryDto>>(response, GlobalConstants.JsonSerializerOptions);

                var customerBalanceResponse = await _apiService.GetDataAsync("api/shopcoins/balance/");
                var customerBalance = JsonSerializer.Deserialize<int>(customerBalanceResponse, GlobalConstants.JsonSerializerOptions);
                ViewBag.CustomerBalance = customerBalance;

                return View(history);
            }
            catch (HttpRequestException ex)
            {
                //ViewBag.ErrorMessage = "An error occurred while fetching the shop coins history.";
                return View(new List<ShopCoinTransactionHistoryDto>());
            }
        }
    }
}

using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Analitics;
using EcommerceWebApp.Models.Dtos.Analitics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("analytics")]
    public class AnaliticsController : Controller
    {
        private readonly IApiService _apiService;

        public AnaliticsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            try
            {
                string query = BuildDateQuery(from, to);

                var dashboardJson = await _apiService.GetDataAsync($"api/analitics/dashboard{query}");
                var revenueJson = await _apiService.GetDataAsync($"api/analitics/revenue{query}");
                var ordersJson = await _apiService.GetDataAsync($"api/analitics/orders{query}");
                var refundsJson = await _apiService.GetDataAsync($"api/analitics/refunds{query}");
                var returnsJson = await _apiService.GetDataAsync($"api/analitics/returns{query}");
                var customersJson = await _apiService.GetDataAsync($"api/analitics/customers");
                var topProductsJson = await _apiService.GetDataAsync($"api/analitics/top-products?count=5");

                Console.WriteLine($"api/analitics/dashboard" + query);
                Console.WriteLine($"api/analitics/revenue{query}");

                var vm = new AnalyticsViewModel
                {
                    Dashboard = JsonSerializer.Deserialize<DashboardSummaryDto>(dashboardJson, GlobalConstants.JsonSerializerOptions),
                    Revenue = JsonSerializer.Deserialize<RevenueAnalyticsDto>(revenueJson, GlobalConstants.JsonSerializerOptions),
                    Orders = JsonSerializer.Deserialize<OrdersAnalyticsDto>(ordersJson, GlobalConstants.JsonSerializerOptions),
                    Refunds = JsonSerializer.Deserialize<RefundAnalyticsDto>(refundsJson, GlobalConstants.JsonSerializerOptions),
                    Returns = JsonSerializer.Deserialize<ReturnAnalyticsDto>(returnsJson, GlobalConstants.JsonSerializerOptions),
                    Customers = JsonSerializer.Deserialize<CustomerAnalyticsDto>(customersJson, GlobalConstants.JsonSerializerOptions),
                    TopProducts = JsonSerializer.Deserialize<List<TopProductDto>>(topProductsJson, GlobalConstants.JsonSerializerOptions)
                };

                return View(vm);
            }
            catch (HttpRequestException ex)
            {
                return View(new AnalyticsViewModel());
            }
            catch (Exception)
            {
                return View(new AnalyticsViewModel());
            }
        }

        [HttpGet("export-orders")]
        public async Task<IActionResult> ExportOrders(DateTime? from, DateTime? to)
        {
            try
            {
                var url = $"{GlobalConstants.AnaliticsEndpoint}/export-orders";

                url += BuildDateQuery(from, to);

                var response = await _apiService.GetFileAsync(url);

                var fromStr = from?.ToString("yyyyMMdd") ?? "all";
                var toStr = to?.ToString("yyyyMMdd") ?? "now";

                return File(response,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Orders_{from}_{to}.xlsx");

            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.StatusCode == System.Net.HttpStatusCode.Forbidden
                    ? "You have no permission to access this."
                    : "Something went wrong.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet("inventory-dashboard")]
        public async Task<IActionResult> Inventory()
        {
            try
            {
                var json = await _apiService.GetDataAsync($"{GlobalConstants.AnaliticsEndpoint}/inventory-dashboard");

                var vm = JsonSerializer.Deserialize<InventoryDashboardDto>(json, GlobalConstants.JsonSerializerOptions);

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Something went wrong!";
                return RedirectToAction("Index");
            }
        }

        // ================================
        // Helper to build query string
        // ================================
        private string BuildDateQuery(DateTime? from, DateTime? to)
        {
            var queryParams = new List<string>();

            if (from.HasValue)
                queryParams.Add($"from={from.Value:yyyy-MM-dd}");

            if (to.HasValue)
                queryParams.Add($"to={to.Value:yyyy-MM-dd}");

            return queryParams.Count > 0
                ? "?" + string.Join("&", queryParams)
                : string.Empty;
        }
    }
}
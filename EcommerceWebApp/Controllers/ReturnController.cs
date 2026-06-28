using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("returns")]
    public class ReturnController : Controller
    {
        public readonly IApiService _apiService;

        public ReturnController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var returns = new List<ReturnDto>();
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            try
            {
                var response = string.Empty;

                if (userRole == "Admin" || userRole == "Stuff")
                {
                    response = await _apiService.GetDataAsync($"{GlobalConstants.ReturnsEndpoint}/all");
                    returns = JsonSerializer.Deserialize<List<ReturnDto>>(response, GlobalConstants.JsonSerializerOptions);

                    ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                    GlobalConstants.GetSearchComboboxDtosReturnsEndpoint, _apiService);
                    ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                        GlobalConstants.GetOrderByComboboxDtosReturnsEndpoint, _apiService);

                    return View("AllReturnsAdmin", returns);
                }
                else
                {
                    response = await _apiService.GetDataAsync(
                    $"{GlobalConstants.ReturnsEndpoint}/customer/{int.Parse(User.FindFirstValue("CustomerId"))}");
                    returns = JsonSerializer.Deserialize<List<ReturnDto>>(response, GlobalConstants.JsonSerializerOptions);
                }

                return View(returns);
            }
            catch (Exception ex)
            {
                if (userRole == "Admin" || userRole == "Stuff")
                {
                    return View("AllReturnsAdmin", returns);
                }
                return View(returns);
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] string searchString = "",
            [FromQuery] string? searchProperty = null,
            [FromQuery] string? sortProperty = null,
            [FromQuery] bool sortAscending = false)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(searchString))
                    queryParams.Add($"searchString={Uri.EscapeDataString(searchString)}");

                if (!string.IsNullOrWhiteSpace(searchProperty))
                    queryParams.Add($"searchProperty={Uri.EscapeDataString(searchProperty)}");

                if (!string.IsNullOrWhiteSpace(sortProperty))
                    queryParams.Add($"sortProperty={Uri.EscapeDataString(sortProperty)}");

                queryParams.Add($"sortAscending={sortAscending}");

                var query = queryParams.Count > 0
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;

                var response = await _apiService
                    .GetDataAsync($"{GlobalConstants.ReturnsEndpoint}/filter{query}");

                var refunds = JsonSerializer.Deserialize<List<ReturnDto>>(
                    response,
                    GlobalConstants.JsonSerializerOptions
                );

                ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                    GlobalConstants.GetSearchComboboxDtosReturnsEndpoint, _apiService);
                ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                    GlobalConstants.GetOrderByComboboxDtosReturnsEndpoint, _apiService);

                return View("AllReturnsAdmin", refunds);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction("Index");
            }
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Details(string code)
        {
            try
            {
                var response = await _apiService.GetDataAsync($"{GlobalConstants.ReturnsEndpoint}/{code}");
                var refund = JsonSerializer.Deserialize<ReturnDto>(response, GlobalConstants.JsonSerializerOptions);
                return View(refund);
            }
            catch (HttpRequestException ex)
            {
                return View();
            }
        }

        [HttpPost("change-status")]
        [HttpPost]
        public async Task<IActionResult> ChangeRefundStatus([FromBody] ChangeReturnStatusDto dto)
        {
            try
            {
                await _apiService.PostDataAsync(
                    $"{GlobalConstants.ReturnsEndpoint}/change-status",
                    JsonSerializer.Serialize(dto));

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{refundCode}")]
        public async Task<IActionResult> Delete(string refundCode)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.ReturnsEndpoint}/{refundCode}");

                return Ok(new
                {
                    success = true,
                    message = "Return cancelled successfully"
                });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "You have no permission to cancell the return"
                    });
                }
            }
            return BadRequest(new
            {
                success = false,
                message = "Failed to cancel return"
            });
        }
    }
}

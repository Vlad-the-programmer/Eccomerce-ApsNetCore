using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models.Dtos;
using EcommerceWebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("refunds")]
    public class RefundsController : Controller
    {
        private readonly IApiService _apiService;

        public RefundsController(IApiService apiService)
        {
            _apiService = apiService;

        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);
                var response = string.Empty;
                var refunds = new List<RefundDto>();

                if (userRole == "Admin" || userRole == "Stuff")
                {
                    response = await _apiService.GetDataAsync($"{GlobalConstants.RefundsEndpoint}/all");
                    refunds = JsonSerializer.Deserialize<List<RefundDto>>(response, GlobalConstants.JsonSerializerOptions);

                    ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                    GlobalConstants.GetSearchComboboxDtosRefundsEndpoint, _apiService);
                    ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                        GlobalConstants.GetOrderByComboboxDtosRefundsEndpoint, _apiService);

                    return View("AllRefundsAdmin", refunds);
                }
                else
                {
                    response = await _apiService.GetDataAsync(
                    $"{GlobalConstants.RefundsEndpoint}/customer/{int.Parse(User.FindFirstValue("CustomerId"))}");
                    refunds = JsonSerializer.Deserialize<List<RefundDto>>(response, GlobalConstants.JsonSerializerOptions);
                }

                return View(refunds);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] string searchString = "",
            [FromQuery] string? searchProperty = null,
            [FromQuery] string? sortProperty = null,
            [FromQuery] bool active = false,
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

                queryParams.Add($"active={active}");
                queryParams.Add($"sortAscending={sortAscending}");

                var query = queryParams.Count > 0
                    ? "?" + string.Join("&", queryParams)
                    : string.Empty;

                var response = await _apiService
                    .GetDataAsync($"{GlobalConstants.RefundsEndpoint}/filter{query}");

                var refunds = JsonSerializer.Deserialize<List<RefundDto>>(
                    response,
                    GlobalConstants.JsonSerializerOptions
                );

                ViewBag.SearchComboxOptions = await ProductsEndpointsHelperFuncs.GetSearchComboBoxDtos(
                    GlobalConstants.GetSearchComboboxDtosRefundsEndpoint, _apiService);
                ViewBag.OrderbyComboxOptions = await ProductsEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                    GlobalConstants.GetOrderByComboboxDtosRefundsEndpoint, _apiService);

                return View("AllRefundsAdmin", refunds);
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
                var response = await _apiService.GetDataAsync($"{GlobalConstants.RefundsEndpoint}/{code}");
                var refund = JsonSerializer.Deserialize<RefundDto>(response, GlobalConstants.JsonSerializerOptions);
                return View(refund);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpGet("apply/{orderCode}")]
        public async Task<IActionResult> Apply(string orderCode)
        {
            try
            {
                var response = await _apiService.GetDataAsync($"{GlobalConstants.OrdersEndpoint}/{orderCode}");
                var order = JsonSerializer.Deserialize<OrderDTO>(response, GlobalConstants.JsonSerializerOptions);

                if (order == null)
                {
                    TempData["Error"] = "Order not found.";
                    return RedirectToAction("Index", "Orders");
                }

                if (order.OrderStatus != "Delivered")
                {
                    TempData["Error"] = "Only delivered orders can be refunded.";
                    return RedirectToAction("Status", "Orders", new { code = orderCode });
                }

                if (!order.IsPaid)
                {
                    TempData["Error"] = "Only paid orders can be refunded.";
                    return RedirectToAction("Status", "Orders", new { code = orderCode });
                }

                var orderItems = order.OrderItems.Select(oi => new RefundOrderItemViewModel
                {
                    OrderItemId = oi.Id.Value,
                    ProductId = oi.ProductId,
                    ProductName = oi.ProductName,
                    ProductBrand = oi.ProductBrand,
                    ProductPhoto = oi.ProductPhoto ?? string.Empty,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity,
                }).ToList();

                var viewModel = new RefundApplicationViewModel
                {
                    OrderCode = orderCode,
                    OrderItems = orderItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the refund application.";
                return RedirectToAction("Status", "Orders", new { code = orderCode });
            }
        }

        [HttpPost("apply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(RefundApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var response = await _apiService.GetDataAsync($"{GlobalConstants.OrdersEndpoint}/{model.OrderCode}");
                var order = JsonSerializer.Deserialize<OrderDTO>(response, GlobalConstants.JsonSerializerOptions);

                if (order != null)
                {
                    model.OrderItems = order.OrderItems.Select(oi => new RefundOrderItemViewModel
                    {
                        OrderItemId = oi.Id.Value,
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ProductBrand = oi.ProductBrand,
                        ProductPhoto = oi.ProductPhoto ?? string.Empty,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        RefundQuantity = 1, // Default to 1
                        IsSelected = false
                    }).ToList();
                }

                return View(model);
            }

            try
            {
                var createRefundDto = new CreateRefundDto
                {
                    OrderCode = model.OrderCode,
                    OrderItems = model.OrderItems
                        .Where(oi => oi.IsSelected)
                        .Select(oi => new CreateRefundItemDto
                        {
                            OrderItemId = oi.OrderItemId,
                            Quantity = oi.RefundQuantity > 0 ? oi.RefundQuantity : 1,
                            Reason = oi.Reason
                        }).ToList()
                };

                var jsonContent = JsonSerializer.Serialize(createRefundDto);
                await _apiService.PostDataAsync($"{GlobalConstants.RefundsEndpoint}/apply", jsonContent);

                TempData["Success"] = "Your refund request has been submitted successfully!";
                return RedirectToAction("Index", "Refunds");
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "An error occurred while submitting your refund request.";
                return RedirectToAction("Status", "Orders", new { code = model.OrderCode });
            }
        }

        [HttpPost("change-refund-status")]
        public async Task<IActionResult> ChangeRefundStatus([FromBody] ChangeRefundStatusRequest dto)
        {
            try
            {
                await _apiService.PostDataAsync(
                    $"{GlobalConstants.RefundsEndpoint}/change-refund-status", JsonSerializer.Serialize(dto));
                TempData["Success"] = "Refund status updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating the refund status.";
                return RedirectToAction("Index");
            }
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.RefundsEndpoint}/{code}");

                return Ok(new
                {
                    success = true,
                    message = "Refund cancelled successfully"
                });
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "You have no permission to cancell the refund"
                    });
                }
            }
            return BadRequest(new
            {
                success = false,
                message = "Failed to cancel refund"
            });
        }
    }

    public class ChangeRefundStatusRequest
    {
        public string RefundCode { get; set; }
        public string Status { get; set; }
    }
}

using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("orders")]
    public class OrdersController : Controller
    {
        private readonly IApiService _apiService;

        public OrdersController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customerIdClaim = User.FindFirst("CustomerId");
            int? customerId = null;

            if (customerIdClaim != null && int.TryParse(customerIdClaim.Value, out var tempId))
            {
                customerId = tempId;
            }

            bool isAdmin = User.IsInRole("Admin") ||
                           User.FindFirst(ClaimTypes.Role)?.Value == "Admin" ||
                           User.FindFirst(ClaimTypes.Role)?.Value == "Stuff" ||
                           User.FindFirst("Role")?.Value == "Admin" ||
                           User.FindFirst("Role")?.Value == "Stuff";


            List<OrderDTO> orders;

            if (isAdmin)
            {
                orders = await OrdersEndpointsHelperFuncs.GetOrders(GlobalConstants.OrdersEndpoint, _apiService);
            }
            else if (customerId.HasValue)
            {
                orders = await OrdersEndpointsHelperFuncs.GetOrders($"{GlobalConstants.UserOrdersEndpoint}/{customerId}", _apiService);
            }
            else
            {
                orders = new List<OrderDTO>();
            }

            ViewBag.SearchComboxOptions = await OrdersEndpointsHelperFuncs.GetSearchComboBoxDtos(
                GlobalConstants.GetSearchComboboxDtosOrdersEndpoint, _apiService);
            ViewBag.OrderbyComboxOptions = await OrdersEndpointsHelperFuncs.GetOrderByComboBoxDtos(
                GlobalConstants.GetOrderByComboboxDtosOrdersEndpoint, _apiService);

            if (TempData["CouponDiscount"] != null)
            {
                ViewBag.CouponDiscount = decimal.Parse(TempData["CouponDiscount"].ToString(), CultureInfo.InvariantCulture);
                ViewBag.AppliedCouponCode = TempData["AppliedCouponCode"]?.ToString();
                ViewBag.CouponSuccess = TempData["Success"]?.ToString();
            }
            else if (TempData["CouponError"] != null)
            {
                ViewBag.CouponError = TempData["CouponError"]?.ToString();
            }

            if (isAdmin) return View("ProcessOrderStaff", orders);

            return View(orders);
        }

        [HttpPost("change-order-status")]
        public async Task<IActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatusDto changeOrderStatusDto)
        {
            try
            {
                await _apiService.PostDataAsync($"{GlobalConstants.OrdersEndpoint}/change-order-status",
                    JsonSerializer.Serialize(changeOrderStatusDto));
            }
            catch (HttpRequestException e)
            {
                TempData["Error"] = "Failed to change the status of the order";
            }
            return RedirectToAction("Index");
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
              [FromQuery] string searchString = "",
              [FromQuery] string? searchProperty = null,
              [FromQuery] string? sortProperty = null,
              [FromQuery] bool sortAscending = false,
              [FromQuery] DateTime? fromDate = null,
              [FromQuery] DateTime? toDate = null)
        {
            List<OrderDTO> filteredOrders = await OrdersEndpointsHelperFuncs.GetFilteredOrders(
                                                        GlobalConstants.FilterOrdersEndpoint, searchString,
                                                        searchProperty, sortProperty, sortAscending,
                                                        fromDate, toDate, _apiService);

            ViewBag.SearchComboxOptions = await OrdersEndpointsHelperFuncs.GetSearchComboBoxDtos(GlobalConstants.GetSearchComboboxDtosOrdersEndpoint, _apiService);
            ViewBag.OrderbyComboxOptions = await OrdersEndpointsHelperFuncs.GetOrderByComboBoxDtos(GlobalConstants.GetOrderByComboboxDtosOrdersEndpoint, _apiService);

            bool isAdmin = User.IsInRole("Admin") ||
                           User.FindFirst(ClaimTypes.Role)?.Value == "Admin" ||
                           User.FindFirst(ClaimTypes.Role)?.Value == "Stuff" ||
                           User.FindFirst("Role")?.Value == "Admin" ||
                           User.FindFirst("Role")?.Value == "Stuff";

            if (isAdmin) return View("ProcessOrderStaff", filteredOrders);

            return View("Index", filteredOrders);
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> SelectCartItems([FromForm] List<int> selectedItemIds)
        {
            if (selectedItemIds == null || !selectedItemIds.Any())
            {
                TempData["Error"] = "Please select at least one item.";
                return RedirectToAction("Index", "Cart");
            }

            TempData["SelectedItemIds"] = string.Join(",", selectedItemIds);

            return RedirectToAction("CreateOrderForm");
        }

        [HttpGet("create-order-form")]
        public async Task<IActionResult> CreateOrderForm()
        {
            var selectedIdsString = TempData["SelectedItemIds"]?.ToString();
            List<int> selectedItemIds = new List<int>();

            if (!string.IsNullOrEmpty(selectedIdsString))
            {
                selectedItemIds = selectedIdsString.Split(',')
                    .Select(int.Parse)
                    .ToList();
                TempData.Keep("SelectedItemIds");
            }

            var cart = await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
            var cartItems = cart.ShoppingCartItems;

            if (selectedItemIds.Any())
            {
                cartItems = cartItems.Where(item => selectedItemIds.Contains(item.ProductId)).ToList();
                cart.CartTotal = cartItems.Sum(item => item.ProductPrice * item.Amount);
            }

            var deliveryMethods = await DeliveryMethodsEndpointsHelperFuncs.GetDeliveryMethods(GlobalConstants.DeliveryMethodsEndpoint, _apiService);
            var paymentMethods = await PaymentMethodsEndpointsHelperFuncs.GetPaymentMethods(GlobalConstants.PaymentMethodsEndpoint, _apiService);
            var countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);
            var orderModel = await OrdersEndpointsHelperFuncs.GetOrderCreateTemplate($"{GlobalConstants.GetOrderCreateModelEndpoint}/{cart.ShoppingCartId}", _apiService);

            ViewBag.CartItems = cartItems;
            ViewBag.CartTotal = cart.CartTotal;
            ViewBag.MaxCoinsUsed = await OrdersEndpointsHelperFuncs.CalculateCoinsAmountToSpend(_apiService,
                cart.CartTotal, User.FindFirstValue("CustomerId"));
            ViewBag.Countries = countries;
            ViewBag.DeliveryMethods = deliveryMethods;
            ViewBag.PaymentMethods = paymentMethods;
            ViewBag.SelectedItemIds = selectedItemIds;

            if (TempData["CouponDiscount"] != null)
            {
                ViewBag.CouponDiscount = decimal.Parse(TempData["CouponDiscount"].ToString(), CultureInfo.InvariantCulture);
                ViewBag.AppliedCouponCode = TempData["AppliedCouponCode"]?.ToString();
                ViewBag.CouponSuccess = TempData["Success"]?.ToString();

                ViewData["CouponDiscount"] = ViewBag.CouponDiscount;

                TempData.Keep("CouponDiscount");
                TempData.Keep("AppliedCouponCode");
            }
            else if (TempData["CouponError"] != null)
            {
                ViewBag.CouponError = TempData["CouponError"]?.ToString();
                TempData.Keep("CouponError");
            }

            return View("Create", orderModel);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewOrderViewModel order)
        {
            var selectedIdsString = TempData["SelectedItemIds"]?.ToString();
            List<int> selectedItemIds = new List<int>();

            if (!string.IsNullOrEmpty(selectedIdsString))
            {
                selectedItemIds = selectedIdsString.Split(',')
                    .Select(int.Parse)
                    .ToList();
            }

            order.SelectedItemsIds = selectedItemIds;
            order.OrderStatus = 1; // Pending

            if (order.SelectedItemsIds == null || !order.SelectedItemsIds.Any())
            {
                TempData["Error"] = "No items selected for the order.";
                return RedirectToAction("Index", "Cart");
            }

            var newOrderDto = new OrderDTO();
            try
            {
                var response = await OrdersEndpointsHelperFuncs.SubmitOrder(
                    GlobalConstants.OrderCreateEndpoint,
                    order,
                    _apiService);
                newOrderDto = JsonSerializer.Deserialize<OrderDTO>(response, GlobalConstants.JsonSerializerOptions);
                return RedirectToAction(nameof(Status), new { code = newOrderDto?.Code });
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Something went wrong";
                Debug.WriteLine($"{ex.Message} {ex.InnerException?.Message}");
                newOrderDto = new OrderDTO();
            }

            return View(order);
        }

        [HttpGet("status/{code}")]
        public async Task<IActionResult> Status(string code)
        {
            var order = await OrdersEndpointsHelperFuncs.GetOrderByCode(GlobalConstants.OrdersEndpoint, code, _apiService);

            if (order == null)
            {
                return View("NotFound");
            }

            Dictionary<OrderItemDTO, int> products = new Dictionary<OrderItemDTO, int>();

            foreach (var item in order.OrderItems)
            {
                products.Add(item, item.Quantity);
            }

            ViewBag.Products = products;

            try
            {
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var response = await _apiService.GetDataAsync(
                    $"{GlobalConstants.RefundsEndpoint}/order/{order.Code}");
                var refunds = JsonSerializer.Deserialize<List<RefundDto>>(response, GlobalConstants.JsonSerializerOptions);

                ViewBag.CanApplyForRefund = true;

                if (refunds.Count > 0 && refunds.Where(r => r.IsActive && r.Code == order.Code).ToList()?.Count > 0)
                {
                    ViewBag.CanApplyForRefund = false;
                }

            }
            catch (Exception ex)
            {
                ViewBag.CanApplyForRefund = false;
            }
            return View(order);
        }

        [HttpPost("cancel/{code}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string code, string _method)
        {
            if (_method == "DELETE")
            {
                try
                {
                    await _apiService.DeleteDataAsync($"{GlobalConstants.OrderDeleteEndpoint}/{code}");
                }
                catch (HttpRequestException ex)
                {
                    TempData["Error"] = ex.Message;
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction(nameof(Status), new { code = code });
        }

        [HttpPut("update/{code}")]
        public async Task<IActionResult> Edit(string code, NewOrderViewModel order)
        {
            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.OrderUpdateEndpoint}/{code}", JsonSerializer.Serialize(order));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}

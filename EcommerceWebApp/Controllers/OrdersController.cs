using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
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
                           User.FindFirst("Role")?.Value == "Admin";

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

            return View(orders);
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

            return View("Index", filteredOrders);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var cart = await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
            var cartItems = cart.ShoppingCartItems;

            var deliveryMethods = await DeliveryMethodsEndpointsHelperFuncs.GetDeliveryMethods(GlobalConstants.DeliveryMethodsEndpoint, _apiService);
            var paymentMethods = await PaymentMethodsEndpointsHelperFuncs.GetPaymentMethods(GlobalConstants.PaymentMethodsEndpoint, _apiService);
            var countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);
            var orderModel = await OrdersEndpointsHelperFuncs.GetOrderCreateTemplate($"{GlobalConstants.GetOrderCreateModelEndpoint}/{cart.ShoppingCartId}", _apiService);


            ViewBag.CartItems = cartItems;
            ViewBag.Countries = countries;
            ViewBag.DeliveryMethods = deliveryMethods;
            ViewBag.PaymentMethods = paymentMethods;

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
            return View(orderModel);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewOrderViewModel order)
        {

            order.OrderStatus = 1; // Pending

            var newOrderDto = new OrderDTO();
            try
            {
                var response = await OrdersEndpointsHelperFuncs.SubmitOrder(GlobalConstants.OrderCreateEndpoint, order, _apiService);
                newOrderDto = JsonSerializer.Deserialize<OrderDTO>(response, GlobalConstants.JsonSerializerOptions);
                return RedirectToAction(nameof(Status), new { code = newOrderDto?.Code });
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
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

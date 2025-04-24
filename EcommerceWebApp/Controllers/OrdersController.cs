using EcommerceRestApi.Models;
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
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
            var orders = await OrdersEndpointsHelperFuncs.GetOrders(GlobalConstants.OrdersEndpoint, _apiService);
            return View(orders);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var cartItems = new List<ShoppingCartItemVM>();
            var countries = new List<string>();
            var orderModel = new OrderViewModel();


            //await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
            cartItems = await CartEndpointsHelperFuncs.GetCartItems(GlobalConstants.GetCartItemsEndpoint, _apiService);
            countries = await CountriesEndpointsHelperFuncs.GetCountriesNames(GlobalConstants.CountriesEndpoint, _apiService);
            orderModel = await OrdersEndpointsHelperFuncs.GetOrderCreateTemplate(GlobalConstants.GetOrderCreateModelEndpoint, _apiService);


            ViewBag.CartItems = cartItems;
            ViewBag.Countries = countries;
            return View(orderModel);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel order)
        {
            try
            {
                var response = await OrdersEndpointsHelperFuncs.SubmitOrder(GlobalConstants.OrderCreateEndpoint, order, _apiService);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return View(order);
            }

            return RedirectToAction(nameof(Status), new { code = order.Code });
        }

        [HttpGet("status/{code}")]
        public async Task<IActionResult> Status(string code)
        {
            var order = await OrdersEndpointsHelperFuncs.GetOrderByCode(GlobalConstants.OrdersEndpoint, code, _apiService);

            if (order == null)
            {
                return View("NotFound");
            }

            Dictionary<OrderItemViewModel, int> products = new Dictionary<OrderItemViewModel, int>();

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
        public async Task<IActionResult> Edit(string code, OrderViewModel order)
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

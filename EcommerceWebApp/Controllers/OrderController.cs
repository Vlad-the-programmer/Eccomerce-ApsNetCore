using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    public class OrderController : Controller
    {
        private readonly IApiService _apiService;
        public OrderController(IApiService apiService) {
            _apiService = apiService;
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> Index(string code)
        {
            var order = await OrdersEndpointsHelperFuncs.GetOrderByCode(GlobalConstants.OrdersEndpoint, code, _apiService);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderViewModel order)
        {
            var response = await OrdersEndpointsHelperFuncs.SubmitOrder(GlobalConstants.OrderCreateEndpoint, order, _apiService);

            TempData["Error"] = response;
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

            Dictionary<OrderItemViewModel, int> products = new Dictionary<OrderItemViewModel, int>();

            foreach (var item in order.OrderItems)
            {
                products.Add(item, item.Quantity);
            }
            return View(products);
        }
    }
}

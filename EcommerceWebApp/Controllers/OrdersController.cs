using EcommerceRestApi.Models;
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

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
            //var order = await OrdersEndpointsHelperFuncs.GetOrderByCode(GlobalConstants.OrdersEndpoint, code, _apiService);
            var orders = await OrdersEndpointsHelperFuncs.GetOrders(GlobalConstants.OrdersEndpoint, _apiService);
            return View(orders);
        }

        [HttpGet("/{code}")]
        public async Task<IActionResult> GetOrder(string code)
        {
            var order = await OrdersEndpointsHelperFuncs.GetOrderByCode(GlobalConstants.OrdersEndpoint, code, _apiService);

            if (order == null)
            {
                return View("NotFound");
            }

            return View(order);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var cartItems = new List<ShoppingCartItemVM>();
            try
            {
                await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
                cartItems = await CartEndpointsHelperFuncs.GetCartItems(GlobalConstants.GetCartItemsEndpoint, _apiService);

            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }
            ViewBag.CartItems = cartItems;
            return View();
        }

        [HttpPost("create")]
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

            return RedirectToAction(nameof(GetOrder), new { code = order.Code });
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

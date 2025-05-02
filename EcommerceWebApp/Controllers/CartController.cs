using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly IApiService _apiService;

        public CartController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = new ShoppingCartDTO();
            try
            {
                cart = await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                cart = new ShoppingCartDTO();
            }

            return View(cart);
        }

        [HttpPost("add/{productId}")]
        public async Task<IActionResult> AddItemToCart(int productId)
        {
            var product = await ProductsEndpointsHelperFuncs.GetProductById($"{GlobalConstants.ProductsEndpoint}/{productId}", _apiService);

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var message = await CartEndpointsHelperFuncs.AddItemToCart(GlobalConstants.AddItemToCartEndpoint, _apiService, productId);
            return RedirectToAction("Index");
        }

        [HttpPost("remove/{productId}")]

        public async Task<IActionResult> RemoveItemFromCart(int productId)
        {
            var product = await ProductsEndpointsHelperFuncs.GetProductById($"{GlobalConstants.ProductsEndpoint}/{productId}", _apiService);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var message = await CartEndpointsHelperFuncs.RemoveItemFromCart(GlobalConstants.RemoveItemFromCartEndpoint,
                                                                                    _apiService, productId);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var response = await CartEndpointsHelperFuncs.ClearCart(GlobalConstants.ClearCartEndpoint, _apiService);

            return RedirectToAction(nameof(Index));
        }
    }
}

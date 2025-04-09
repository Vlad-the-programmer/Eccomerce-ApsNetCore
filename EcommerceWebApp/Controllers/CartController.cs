using EcommerceRestApi.Models;
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Helpers.Cart;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        public async Task<IActionResult> Index()
        {
            var cartItems = new List<ShoppingCartItemVM>();
            var cartTotal = 0.0;
            try
            {
                await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
                cartItems = await CartEndpointsHelperFuncs.GetCartItems(GlobalConstants.GetCartItemsEndpoint, _apiService);
                cartTotal = await CartEndpointsHelperFuncs.GetShoppingCartTotal(_apiService);

            } catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                Debug.WriteLine($"Error: {ex.Message}");
            }

            var model = new ShoppingCartViewModel()
            {
                ShoppingCartItems = cartItems,
                CartTotal = cartTotal,
            };
            return View(model);
        }

        public async Task<IActionResult> AddItemToCart(int product_id)
        {
            var cartItem = await CartEndpointsHelperFuncs.GetProductById(GlobalConstants.GetCartProductById + $"{product_id}", _apiService);

            if (cartItem == null) {
                return RedirectToAction(nameof(Index));
            }

            var message = await CartEndpointsHelperFuncs.AddItemToCart(GlobalConstants.AddItemToCartEndpoint, _apiService, cartItem);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveItemFromCart(int product_id)
        {
            var cartItem = await CartEndpointsHelperFuncs.GetProductById(GlobalConstants.GetCartProductById + $"{product_id}", _apiService);

            if (cartItem == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var message = await CartEndpointsHelperFuncs.RemoveItemFromCart(GlobalConstants.RemoveItemFromCartEndpoint,
                                                                                    _apiService, cartItem);
           
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ClearCart()
        {
            var response = await CartEndpointsHelperFuncs.ClearCart(GlobalConstants.ClearCartEndpoint, _apiService);

            return RedirectToAction(nameof(Index));
        }
    }
}

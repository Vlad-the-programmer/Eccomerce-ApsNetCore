using EcommerceRestApi.Models;
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Helpers.Cart;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IApiService _apiService;

        public CartController(IApiService apiService) 
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            await CartEndpointsHelperFuncs.GetCreateCart(GlobalConstants.GetCartEndpoint, _apiService);
            var cartItems = await CartEndpointsHelperFuncs.GetCartItems(GlobalConstants.GetCartItemsEndpoint, _apiService);
            var cartTotal = await CartEndpointsHelperFuncs.GetShoppingCartTotal(_apiService);

            var response = new ShoppingCartViewModel()
            {
                ShoppingCartItems = cartItems,
                CartTotal = cartTotal,
            };
            return View();
        }

        public async Task<IActionResult> AddItemToCart(int product_id)
        {
            var cartItem = await CartEndpointsHelperFuncs.GetProductById(GlobalConstants.GetCartProductById + $"{product_id}", _apiService);

            if (cartItem == null) {
                return RedirectToAction(nameof(Index));
            }

            var response = await CartEndpointsHelperFuncs.AddItemToCart(GlobalConstants.AddItemToCartEndpoint, _apiService, cartItem);

            if (response != null)
            {
                return View(response);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveItemFromCart(int product_id)
        {
            var cartItem = await CartEndpointsHelperFuncs.GetProductById(GlobalConstants.GetCartProductById + $"{product_id}", _apiService);

            if (cartItem == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var response = await CartEndpointsHelperFuncs.RemoveItemFromCart(GlobalConstants.RemoveItemFromCartEndpoint,
                                                                                    _apiService, cartItem);
            
            if (response != null)
            { 
                return View(response);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ClearCart()
        {
            var response = await CartEndpointsHelperFuncs.ClearCart(GlobalConstants.ClearCartEndpoint, _apiService);

            if (response != null)
            {
                return View(response);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

using EcommerceRestApi.Models;
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
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
            return View();
        }

        public async Task<IActionResult> AddItemToCart(ShoppingCartItemVM cartItem)
        {
            var response = await CartEndpointsHelperFuncs.AddItemToCart(GlobalConstants.AddItemToCartEndpoint, _apiService, cartItem);

            if (response != null)
            {
                return View(response);
            }
            return View();
        }

        public async Task<IActionResult> RemoveItemFromCart(ShoppingCartItemVM cartItem)
        {
            var response = await CartEndpointsHelperFuncs.RemoveItemFromCart(GlobalConstants.RemoveItemFromCartEndpoint,
                                                                                    _apiService, cartItem);

            if (response != null)
            {
                return View(response);
            }
            return View();
        }

        public async Task<IActionResult> ClearCart(ShoppingCartItemVM cartItem)
        {
            var response = await CartEndpointsHelperFuncs.ClearCart(GlobalConstants.ClearCartEndpoint, _apiService);

            if (response != null)
            {
                return View(response);
            }
            return View();
        }
    }
}

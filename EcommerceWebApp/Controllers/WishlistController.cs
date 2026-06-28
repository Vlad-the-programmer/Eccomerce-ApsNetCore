using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IApiService _apiService;

        public WishlistController(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var json = await _apiService.PostDataAsync("api/wishlist/create");

                var wishlist = JsonSerializer.Deserialize<WishListDto>(
                    json,
                    GlobalConstants.JsonSerializerOptions
                );

                return View(wishlist);
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;

                return View(new WishListDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            try
            {
                await _apiService.PostDataAsync($"api/wishlist/add-item/{productId}");

                TempData["Success"] = "Item added to wishlist!";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                await _apiService.DeleteDataAsync($"api/wishlist/remove/{id}");

                TempData["Success"] = "Item removed from wishlist!";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost("transfer-to-cart")]
        public async Task<IActionResult> AddItemsToCart()
        {
            try
            {
                await _apiService.PostDataAsync("api/wishlist/transfer-to-cart");

                TempData["Success"] = "Items added to cart!";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                await _apiService.DeleteDataAsync($"api/wishlist/clear");

                TempData["Success"] = "Wishlist cleared!";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
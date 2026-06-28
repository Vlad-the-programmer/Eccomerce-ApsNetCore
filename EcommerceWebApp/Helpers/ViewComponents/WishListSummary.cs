using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Helpers.ViewComponents
{
    public class WishListSummary : ViewComponent
    {
        public readonly IApiService _apiService;

        public WishListSummary(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                var json = await _apiService.PostDataAsync("api/wishlist/create");

                var wishlist = JsonSerializer.Deserialize<WishListDto>(
                    json,
                    GlobalConstants.JsonSerializerOptions
                );

                return View(wishlist.WishlistItems.Any() ? wishlist.WishlistItems.Count : 0);
            }
            catch (HttpRequestException ex)
            {
                return View(0);
            }
        }
    }
}

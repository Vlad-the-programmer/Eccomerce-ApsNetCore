using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    [Route("userReviews")]
    public class ReviewsController : Controller
    {
        private readonly IApiService _apiService;

        public ReviewsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(ReviewUpdateViewModel model)
        {
            try
            {
                var review = await ReviewsEndpointsHelperFuncs.GetReviewById(GlobalConstants.ReviewsEndpoint,
                                                                model.Id, _apiService);
                if (review == null)
                {
                    // Create
                    await _apiService.PostDataAsync(GlobalConstants.ReviewCreateEndpoint,
                                                    JsonSerializer.Serialize(model));
                }
                else
                {
                    // Update
                    await _apiService.UpdateDataAsync($"{GlobalConstants.ReviewUpdateEndpoint}/{model.Id}",
                                                      JsonSerializer.Serialize(model));
                }

            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _apiService.DeleteDataAsync($"{GlobalConstants.ReviewsDeleteEndpoint}/{id}");
            }
            catch (HttpRequestException ex)
            {
                return View("NotFound");
            }
            return View();
        }
    }
}

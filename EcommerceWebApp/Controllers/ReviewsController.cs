using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers;
using EcommerceWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceWebApp.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IApiService _apiService;

        public ReviewsController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrUpdate(ReviewViewModel model)
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
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }
            return RedirectToAction("Details", "Products", new { id = model.ProductId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var review = await ReviewsEndpointsHelperFuncs.GetReviewById(GlobalConstants.ReviewsEndpoint, id, _apiService);

            if (review == null)
            {
                return View("NotFound");
            }
            return View(review);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, ReviewUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return View("NotFound");
            }

            try
            {
                await _apiService.UpdateDataAsync($"{GlobalConstants.ReviewUpdateEndpoint}/{id}",
                                                        JsonSerializer.Serialize(model));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
            return RedirectToAction(nameof(Details), nameof(ProductsController), new { id = model.ProductId });
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

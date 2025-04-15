using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class ReviewsEndpointsHelperFuncs
    {
        public async static Task<List<ReviewViewModel>> GetReviews(string endpoint, IApiService apiService)
        {
            var reviews = new List<ReviewViewModel>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                reviews = JsonSerializer.Deserialize<List<ReviewViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            return reviews;
        }

        public async static Task<ReviewViewModel?> GetReviewById(string endpoint, int id, IApiService apiService)
        {
            var review = new ReviewViewModel();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}/{id}"); // response is a string
                review = JsonSerializer.Deserialize<ReviewViewModel?>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { review = null; }

            return review;
        }

    }
}

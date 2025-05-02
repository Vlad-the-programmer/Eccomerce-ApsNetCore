using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class ReviewsEndpointsHelperFuncs
    {
        public async static Task<List<ReviewDTO>> GetReviews(string endpoint, IApiService apiService)
        {
            var reviews = new List<ReviewDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                reviews = JsonSerializer.Deserialize<List<ReviewDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            return reviews;
        }

        public async static Task<ReviewDTO?> GetReviewById(string endpoint, int id, IApiService apiService)
        {
            var review = new ReviewDTO();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}/{id}"); // response is a string
                review = JsonSerializer.Deserialize<ReviewDTO?>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { review = null; }

            return review;
        }

    }
}

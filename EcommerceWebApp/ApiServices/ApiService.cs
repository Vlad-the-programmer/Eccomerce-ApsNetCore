using EcommerceWebApp.Models.AppViewModels;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace EcommerceWebApp.ApiServices
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri("https://localhost:5001");
            var token = _httpContextAccessor.HttpContext.Session.GetString("auth_token");

            //if (string.IsNullOrEmpty(token))
            //    throw new UnauthorizedAccessException("User is not authenticated!");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> GetDataAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode(); // Throws if the status code is not successful
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PostDataAsync(string endpoint, string jsonContent = "")
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();

                var errorResponse = errorJson != "" ? JsonSerializer.Deserialize<ErrorViewModel>(errorJson) : new ErrorViewModel();
                throw new HttpRequestException(errorResponse?.Message ?? "An error occurred.");
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}

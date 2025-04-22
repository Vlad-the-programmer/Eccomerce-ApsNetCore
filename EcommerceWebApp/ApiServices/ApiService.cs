using EcommerceWebApp.Models.AppViewModels;
using System.Text;
using System.Text.Json;

namespace EcommerceWebApp.ApiServices
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
                try
                {
                    var errorResponse = errorJson != "" ? JsonSerializer.Deserialize<ErrorViewModel>(errorJson, GlobalConstants.JsonSerializerOptions) : new ErrorViewModel();
                    var errorMessage = string.Join(Environment.NewLine, errorResponse?.Errors ?? new List<string>());
                    throw new HttpRequestException(errorMessage);
                }
                catch (JsonException ex)
                {
                    throw new HttpRequestException(errorJson);
                }
            }
            return await response.Content.ReadAsStringAsync();
        }
        public async Task<string> DeleteDataAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();

                var errorResponse = errorJson != "" ? JsonSerializer.Deserialize<ErrorViewModel>(errorJson) : new ErrorViewModel();
                throw new HttpRequestException(errorResponse?.Message ?? "An error occurred.");
            }
            return await response.Content.ReadAsStringAsync();
        }


        public async Task<string> UpdateDataAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
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

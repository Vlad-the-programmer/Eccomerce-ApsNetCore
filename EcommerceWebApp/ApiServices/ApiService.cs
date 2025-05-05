using EcommerceWebApp.Models.AppViewModels;
using System.Text;
using System.Text.Json;

namespace EcommerceWebApp.ApiServices
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiService> _logger;

        public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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
                await ThrowHttpRequestExceptionHelperFunc(response);
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteDataAsync(string endpoint)
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            if (!response.IsSuccessStatusCode)
            {
                await ThrowHttpRequestExceptionHelperFunc(response);
            }

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<string> UpdateDataAsync(string endpoint, string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                await ThrowHttpRequestExceptionHelperFunc(response);
            }

            return await response.Content.ReadAsStringAsync();
        }

        private async Task ThrowHttpRequestExceptionHelperFunc(HttpResponseMessage response)
        {
            var errorJson = await response.Content.ReadAsStringAsync();

            try
            {
                ErrorViewModel? errorResponse = !string.IsNullOrWhiteSpace(errorJson)
                    ? JsonSerializer.Deserialize<ErrorViewModel>(errorJson, GlobalConstants.JsonSerializerOptions)
                    : new ErrorViewModel();

                IList<string> errors = new List<string>();

                if (errorResponse?.Errors == null || !errorResponse.Errors.Any())
                {
                    if (!string.IsNullOrWhiteSpace(errorResponse?.Message))
                    {
                        errors.Add(errorResponse.Message);
                    }
                }
                else
                {
                    errors = errorResponse.Errors;
                }

                var errorMessage = string.Join(Environment.NewLine, errors);
                throw new HttpRequestException(errorMessage);

            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize error response: {Response}", response.ReasonPhrase);
                throw new HttpRequestException($"Unexpected error response: {errorJson}");
            }
        }
    }
}

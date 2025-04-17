using EcommerceWebApp.Models.AppViewModels;
using System.Text;
using System.Text.Json;

namespace EcommerceWebApp.ApiServices
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            //_httpContextAccessor = httpContextAccessor;

            //var handler = new HttpClientHandler
            //{
            //    UseCookies = true,  // Enable cookies
            //    AllowAutoRedirect = true, // Follow redirects
            //    CookieContainer = new CookieContainer(), // Store cookies
            //};

            //_httpClient = new HttpClient(handler)
            //{
            //    BaseAddress = new Uri(AppConstants.BASE_URL)
            //};

            // Retrieve token from session if needed
            //var token = _httpContextAccessor.HttpContext?.Session.GetString("auth_token");
            //if (!string.IsNullOrEmpty(token))
            //{
            //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //}

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                var errorMessage = string.Join(Environment.NewLine, errorResponse?.Errors ?? new List<string>());
                throw new HttpRequestException(errorMessage);
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

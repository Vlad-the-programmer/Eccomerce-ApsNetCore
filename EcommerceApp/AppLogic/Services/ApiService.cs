using EcommerceApp.AppLogic.Dtos;
using EcommerceApp.AppLogic.VMs;
using EcommerceApp.Helpers.Session;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EcommerceApp.AppLogic.Services
{
    public static class ApiService
    {
        private static HttpClient _client = new HttpClient();

        static ApiService()
        {
            AttachToken().GetAwaiter().GetResult();
        }

        public static HttpClient Client => _client;

        public static async Task<bool> LoginAsync(LoginRequest model)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("X-Client-Type", "mobile");
                _client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await _client.PostAsJsonAsync("http://localhost:5000/api/account/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Login failed: {response.StatusCode} - {error}");
                    return false;
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response content: {content}");

                var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null && result.Success && !string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                    SessionService.Instance.CurrentUser = result.User;
                    return true;
                }

                System.Diagnostics.Debug.WriteLine($"Login response missing token or success false: {result?.Success}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public static async Task AttachToken()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AttachToken error: {ex.Message}");
            }
        }

        public static void RemoveToken()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            SecureStorage.Remove("auth_token");
            SessionService.Instance.CurrentUser = null;
        }

        public static async Task<T?> GetAsync<T>(string url)
        {
            await AttachToken();
            var response = await _client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }

        public static async Task<T?> PostAsync<T>(string url, object data)
        {
            await AttachToken();
            var response = await _client.PostAsJsonAsync(url, data);
            if (!response.IsSuccessStatusCode)
                return default;

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public OrderCustomerDTO? User { get; set; }
    }
}
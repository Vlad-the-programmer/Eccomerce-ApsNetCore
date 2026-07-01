using EcommerceMobileApp.AppLogic.Dtos;
using EcommerceMobileApp.AppLogic.VMs;
using EcommerceMobileApp.Helpers.Session;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace EcommerceMobileApp.AppLogic.Services
{
    public static class ApiService
    {
        private static HttpClient _client;
        private static string _baseUrl;

        static ApiService()
        {
            _baseUrl = "http://localhost:5000";

            // Disable SSL validation for development
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback =
                (message, cert, chain, errors) => true;
            _client = new HttpClient(handler);

            _client.BaseAddress = new Uri(_baseUrl.TrimEnd('/') + '/');
            _client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client.DefaultRequestHeaders.Add("User-Agent", "EcommerceMobileApp");
            _client.DefaultRequestHeaders.Add("X-Client-Type", "mobile");
            _client.Timeout = TimeSpan.FromSeconds(30);

            System.Diagnostics.Debug.WriteLine($"API Base URL: {_baseUrl}");
            System.Diagnostics.Debug.WriteLine($"BaseAddress: {_client.BaseAddress}");
        }

        public static HttpClient Client => _client;
        public static string BaseUrl => _baseUrl;

        public static async Task<bool> LoginAsync(LoginRequest model)
        {
            try
            {
                _client.DefaultRequestHeaders.Authorization = null;

                var response = await _client.PostAsJsonAsync("api/account/login", model);

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

                    var savedToken = await SecureStorage.GetAsync("auth_token");
                    System.Diagnostics.Debug.WriteLine($"Token saved: {!string.IsNullOrEmpty(savedToken)}");

                    var session = SessionService.Instance;
                    session.CurrentUser = result.User;
                    session.UserProfile = await new AccountDetailsViewModel().LoadUserAsync() ?? null!;

                    await Task.Delay(500);

                    var savedUser = await SecureStorage.GetAsync("current_user");
                    if (!string.IsNullOrEmpty(savedUser))
                    {
                        System.Diagnostics.Debug.WriteLine($"User saved successfully: {savedUser.Substring(0, Math.Min(100, savedUser.Length))}...");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("User was NOT saved to storage - saving again");
                        await session.SaveToStorageAsync();
                    }

                    var isLoggedIn = await session.IsLoggedInAsync();
                    System.Diagnostics.Debug.WriteLine($"IsLoggedIn after login: {isLoggedIn}");
                    System.Diagnostics.Debug.WriteLine($"CurrentUser: {session.CurrentUser?.Email}");
                    System.Diagnostics.Debug.WriteLine($"UserId: {session.CurrentUser?.UserId}");
                    System.Diagnostics.Debug.WriteLine($"IsAuthenticated: {session.CurrentUser?.IsAuthenticated}");

                    return true;
                }

                System.Diagnostics.Debug.WriteLine($"Login failed: {result?.Success} - {result?.Message}");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message} {ex.InnerException?.Message}");
                return false;
            }
        }

        public static async Task<bool> AttachToken()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    System.Diagnostics.Debug.WriteLine($"Token attached successfully");
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No token found in SecureStorage");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AttachToken error: {ex.Message}");
                return false;
            }
        }

        public static async Task<string?> GetTokenFromStorage()
        {
            try
            {
                return await SecureStorage.GetAsync("auth_token");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTokenFromStorage error: {ex.Message}");
                return null;
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
            var tokenAttached = await AttachToken();

            if (!tokenAttached)
            {
                System.Diagnostics.Debug.WriteLine("No token available - request will likely fail");
            }
            System.Diagnostics.Debug.WriteLine($"User: {await SecureStorage.GetAsync("current_user")}");

            try
            {
                var requestUrl = url.TrimStart('/');
                var fullUrl = $"{_client.BaseAddress}{requestUrl}";
                System.Diagnostics.Debug.WriteLine($"GET Request: {fullUrl}");

                var authHeader = _client.DefaultRequestHeaders.Authorization;
                System.Diagnostics.Debug.WriteLine($"Authorization: {authHeader?.Scheme} {(authHeader?.Parameter?.Length > 0 ? authHeader.Parameter.Substring(0, Math.Min(20, authHeader.Parameter.Length)) + "..." : "null")}");

                var response = await _client.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"GET failed: {response.StatusCode} - {error}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        System.Diagnostics.Debug.WriteLine("Received 401 - removing token");
                        RemoveToken();
                    }

                    return default;
                }

                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"GET Response: {(content.Length > 100 ? content.Substring(0, 100) + "..." : content)}");

                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GET error: {ex.Message}");
                return default;
            }
        }

        public static async Task<T?> PostAsync<T>(string url, object data)
        {
            var tokenAttached = await AttachToken();

            if (!tokenAttached)
            {
                System.Diagnostics.Debug.WriteLine("No token available - request will likely fail");
            }

            try
            {
                var requestUrl = url.TrimStart('/');
                var fullUrl = $"{_client.BaseAddress}{requestUrl}";
                System.Diagnostics.Debug.WriteLine($"POST Request: {fullUrl}");

                var response = await _client.PostAsJsonAsync(requestUrl, data);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"POST failed: {response.StatusCode} - {error}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        System.Diagnostics.Debug.WriteLine("Received 401 - removing token");
                        RemoveToken();
                    }

                    return default;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"POST error: {ex.Message}");
                return default;
            }
        }

        public static async Task<T?> PostAbsoluteAsync<T>(string url, object data)
        {
            var tokenAttached = await AttachToken();

            if (!tokenAttached)
            {
                System.Diagnostics.Debug.WriteLine("No token available - request will likely fail");
            }

            try
            {
                var fullUrl = url.StartsWith("http") ? url : $"{_baseUrl.TrimEnd('/')}/{url.TrimStart('/')}";
                System.Diagnostics.Debug.WriteLine($"POST Absolute Request: {fullUrl}");

                var response = await _client.PostAsJsonAsync(fullUrl, data);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"POST failed: {response.StatusCode} - {error}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        System.Diagnostics.Debug.WriteLine("Received 401 - removing token");
                        RemoveToken();
                    }

                    return default;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"POST error: {ex.Message}");
                return default;
            }
        }

        public static async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenFromStorage();
            return !string.IsNullOrEmpty(token);
        }

        public static async Task<CurrentUserViewModel?> GetCurrentUserAsync()
        {
            if (!await IsAuthenticatedAsync())
                return null;

            return await GetAsync<CurrentUserViewModel>("api/account/get-current-user");
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public CurrentUserViewModel? User { get; set; }
    }
}
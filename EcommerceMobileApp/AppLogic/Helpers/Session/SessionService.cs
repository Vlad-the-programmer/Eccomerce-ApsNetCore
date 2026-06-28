using EcommerceMobileApp.AppLogic.Dtos;
using System.Text.Json;

namespace EcommerceMobileApp.Helpers.Session
{
    public class SessionService
    {
        private static SessionService _instance;
        private static readonly object _lock = new object();
        private static readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1, 1);
        private bool _isLoaded = false;
        private bool _isLoading = false;

        public static SessionService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new SessionService();
                    }
                }
                return _instance;
            }
        }

        private CurrentUserViewModel _currentUser;
        public CurrentUserViewModel CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                if (value != null)
                {
                    // Save to storage
                    Task.Run(async () => await SaveToStorageAsync());
                }
                else
                {
                    // Clear storage
                    Task.Run(async () => await ClearStorageAsync());
                }
            }
        }

        private List<OrderDto> _userOrders = new List<OrderDto>();
        public IList<OrderDto> UserOrders
        {
            get => _userOrders;
            set
            {
                _userOrders = value?.ToList() ?? new List<OrderDto>();
                Task.Run(async () => await SaveOrdersToStorageAsync());
            }
        }

        private SessionService()
        {
            // Load synchronously in constructor
            LoadFromStorageSync();
        }

        private void LoadFromStorageSync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading from storage (sync)...");

                var userTask = SecureStorage.GetAsync("current_user");
                userTask.Wait();
                var userJson = userTask.Result;

                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<CurrentUserViewModel>(userJson);
                    System.Diagnostics.Debug.WriteLine($"Loaded user from storage: {_currentUser?.Email}");
                    System.Diagnostics.Debug.WriteLine($"User IsAuthenticated: {_currentUser?.IsAuthenticated}");
                    System.Diagnostics.Debug.WriteLine($"User Id: {_currentUser?.UserId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No user found in storage");
                }

                var ordersTask = SecureStorage.GetAsync("user_orders");
                ordersTask.Wait();
                var ordersJson = ordersTask.Result;

                if (!string.IsNullOrEmpty(ordersJson))
                {
                    _userOrders = JsonSerializer.Deserialize<List<OrderDto>>(ordersJson) ?? new List<OrderDto>();
                    System.Diagnostics.Debug.WriteLine($"Loaded {_userOrders.Count} orders from storage");
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFromStorageSync error: {ex.Message}");
                _isLoaded = true;
            }
        }

        public async Task LoadFromStorageAsync()
        {
            if (_isLoading) return;
            _isLoading = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("Loading from storage (async)...");

                var userJson = await SecureStorage.GetAsync("current_user");
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<CurrentUserViewModel>(userJson);
                    System.Diagnostics.Debug.WriteLine($"Loaded user from storage: {_currentUser?.Email}");
                    System.Diagnostics.Debug.WriteLine($"User IsAuthenticated: {_currentUser?.IsAuthenticated}");
                    System.Diagnostics.Debug.WriteLine($"User Id: {_currentUser?.UserId}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No user found in storage");
                }

                var ordersJson = await SecureStorage.GetAsync("user_orders");
                if (!string.IsNullOrEmpty(ordersJson))
                {
                    _userOrders = JsonSerializer.Deserialize<List<OrderDto>>(ordersJson) ?? new List<OrderDto>();
                    System.Diagnostics.Debug.WriteLine($"Loaded {_userOrders.Count} orders from storage");
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFromStorageAsync error: {ex.Message}");
                _isLoaded = true;
            }
            finally
            {
                _isLoading = false;
            }
        }

        public async Task SaveToStorageAsync()
        {
            await _saveLock.WaitAsync();
            try
            {
                if (_currentUser != null)
                {
                    var json = JsonSerializer.Serialize(_currentUser);
                    await SecureStorage.SetAsync("current_user", json);
                    System.Diagnostics.Debug.WriteLine($"Saved user to storage: {_currentUser?.Email}");
                    System.Diagnostics.Debug.WriteLine($"User IsAuthenticated: {_currentUser?.IsAuthenticated}");
                    System.Diagnostics.Debug.WriteLine($"User Id: {_currentUser?.UserId}");

                    var saved = await SecureStorage.GetAsync("current_user");
                    System.Diagnostics.Debug.WriteLine($"Verification: User saved successfully: {!string.IsNullOrEmpty(saved)}");
                }
                else
                {
                    SecureStorage.Remove("current_user");
                    System.Diagnostics.Debug.WriteLine("Removed user from storage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveToStorageAsync error: {ex.Message}");
            }
            finally
            {
                _saveLock.Release();
            }
        }

        public async Task ClearStorageAsync()
        {
            try
            {
                SecureStorage.Remove("current_user");
                SecureStorage.Remove("user_orders");
                System.Diagnostics.Debug.WriteLine("Cleared user storage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearStorageAsync error: {ex.Message}");
            }
        }

        public async Task SaveOrdersToStorageAsync()
        {
            await _saveLock.WaitAsync();
            try
            {
                if (_userOrders != null && _userOrders.Any())
                {
                    var json = JsonSerializer.Serialize(_userOrders);
                    await SecureStorage.SetAsync("user_orders", json);
                    System.Diagnostics.Debug.WriteLine($"Saved {_userOrders.Count} orders to storage");
                }
                else
                {
                    SecureStorage.Remove("user_orders");
                    System.Diagnostics.Debug.WriteLine("Removed orders from storage");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveOrdersToStorageAsync error: {ex.Message}");
            }
            finally
            {
                _saveLock.Release();
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _currentUser = null;
                _userOrders = new List<OrderDto>();

                SecureStorage.Remove("current_user");
                SecureStorage.Remove("user_orders");
                SecureStorage.Remove("auth_token");

                System.Diagnostics.Debug.WriteLine("Logged out successfully");
                _isLoaded = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogoutAsync error: {ex.Message}");
            }
        }

        public bool IsLoggedIn()
        {
            // Use the current user directly - don't reload from storage
            var result = _currentUser != null &&
                         !string.IsNullOrEmpty(_currentUser.UserId) &&
                         _currentUser.IsAuthenticated;

            System.Diagnostics.Debug.WriteLine($"IsLoggedIn: {result}");
            if (_currentUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"CurrentUser: {_currentUser.Email}");
                System.Diagnostics.Debug.WriteLine($"UserId: {_currentUser.UserId}");
                System.Diagnostics.Debug.WriteLine($"IsAuthenticated: {_currentUser.IsAuthenticated}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CurrentUser is null");
            }

            return result;
        }

        public async Task<bool> IsLoggedInAsync()
        {
            // Use the current user directly - don't reload from storage
            var result = _currentUser != null &&
                         !string.IsNullOrEmpty(_currentUser.UserId) &&
                         _currentUser.IsAuthenticated;

            System.Diagnostics.Debug.WriteLine($"IsLoggedInAsync: {result}");
            if (_currentUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"CurrentUser: {_currentUser.Email}");
                System.Diagnostics.Debug.WriteLine($"UserId: {_currentUser.UserId}");
                System.Diagnostics.Debug.WriteLine($"IsAuthenticated: {_currentUser.IsAuthenticated}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CurrentUser is null");
            }

            return result;
        }

        public async Task ReloadAsync()
        {
            await LoadFromStorageAsync();
        }

        public CurrentUserViewModel GetCurrentUser()
        {
            return _currentUser;
        }

        public async Task<CurrentUserViewModel> GetCurrentUserAsync()
        {
            return _currentUser;
        }

        // Method to set user and save
        public async Task SetCurrentUserAsync(CurrentUserViewModel user)
        {
            System.Diagnostics.Debug.WriteLine($"SetCurrentUserAsync called with user: {user?.Email}");
            if (user != null)
            {
                // Ensure IsAuthenticated is true
                user.IsAuthenticated = true;
                _currentUser = user;
                await SaveToStorageAsync();
                _isLoaded = true;
                System.Diagnostics.Debug.WriteLine($"User set successfully: {_currentUser?.Email}, IsAuthenticated: {_currentUser?.IsAuthenticated}");
            }
            else
            {
                _currentUser = null;
                await ClearStorageAsync();
            }
        }

        // Method to update user without changing IsAuthenticated
        public async Task UpdateCurrentUserAsync(CurrentUserViewModel user)
        {
            if (user != null)
            {
                // Preserve IsAuthenticated if it was true
                if (_currentUser != null && _currentUser.IsAuthenticated)
                {
                    user.IsAuthenticated = true;
                }
                _currentUser = user;
                await SaveToStorageAsync();
            }
        }
    }
}
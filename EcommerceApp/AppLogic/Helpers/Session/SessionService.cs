using EcommerceApp.AppLogic.Dtos;
using System.Text.Json;

namespace EcommerceApp.Helpers.Session
{
    public class SessionService
    {
        private static SessionService _instance;
        private static readonly object _lock = new object();

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

        private OrderCustomerDTO _currentUser;
        public OrderCustomerDTO CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                SaveToStorage();
            }
        }

        private List<OrderDto> _userOrders = new List<OrderDto>();
        public IList<OrderDto> UserOrders
        {
            get => _userOrders;
            set
            {
                _userOrders = value?.ToList() ?? new List<OrderDto>();
                SaveOrdersToStorage();
            }
        }

        private SessionService()
        {
            // Private constructor for singleton
            LoadFromStorage().ConfigureAwait(false);
        }

        public async Task LoadFromStorage()
        {
            try
            {
                var userJson = await SecureStorage.GetAsync("current_user");
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<OrderCustomerDTO>(userJson);
                }

                var ordersJson = await SecureStorage.GetAsync("user_orders");
                if (!string.IsNullOrEmpty(ordersJson))
                {
                    _userOrders = JsonSerializer.Deserialize<List<OrderDto>>(ordersJson) ?? new List<OrderDto>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadFromStorage error: {ex.Message}");
            }
        }

        private async void SaveToStorage()
        {
            try
            {
                if (_currentUser != null)
                {
                    var json = JsonSerializer.Serialize(_currentUser);
                    await SecureStorage.SetAsync("current_user", json);
                }
                else
                {
                    SecureStorage.Remove("current_user");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveToStorage error: {ex.Message}");
            }
        }

        private async void SaveOrdersToStorage()
        {
            try
            {
                if (_userOrders != null && _userOrders.Any())
                {
                    var json = JsonSerializer.Serialize(_userOrders);
                    await SecureStorage.SetAsync("user_orders", json);
                }
                else
                {
                    SecureStorage.Remove("user_orders");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveOrdersToStorage error: {ex.Message}");
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            UserOrders.Clear();
            SecureStorage.Remove("current_user");
            SecureStorage.Remove("user_orders");
        }

        public bool IsLoggedIn()
        {
            return CurrentUser != null && CurrentUser.Id > 0 && CurrentUser.IsAuthenticated;
        }
    }
}
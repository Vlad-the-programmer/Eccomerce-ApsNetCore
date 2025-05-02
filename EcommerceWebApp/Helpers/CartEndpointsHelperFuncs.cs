using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CartEndpointsHelperFuncs
    {
        public static async Task<ShoppingCartDTO> GetCreateCart(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var cart = JsonSerializer.Deserialize<ShoppingCartDTO>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            return cart;
        }

        public static async Task<List<ShoppingCartItemDTO>> GetCartItems(string endpoint, IApiService apiService)
        {
            var cartItems = new List<ShoppingCartItemDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint);
                cartItems = JsonSerializer.Deserialize<List<ShoppingCartItemDTO>>(response, GlobalConstants.JsonSerializerOptions);
            }
            catch (HttpRequestException ex)
            {
                cartItems = new List<ShoppingCartItemDTO>();
            }

            return cartItems;
        }

        public static async Task<ShoppingCartItemDTO?> GetProductById(string endpoint, int productId, IApiService apiService)
        {
            var cartItem = new ShoppingCartItemDTO();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}/{productId}"); // response is a string

                cartItem = JsonSerializer.Deserialize<ShoppingCartItemDTO>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex)
            {
                cartItem = null;
            }

            return cartItem;
        }

        public static async Task<string> AddItemToCart(string endpoint, IApiService apiService, int productId)
        {
            try
            {
                await apiService.PostDataAsync($"{endpoint}/{productId}"); // response is a string

            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public static async Task<string> RemoveItemFromCart(string endpoint,
                                                            IApiService apiService,
                                                            int productId)
        {
            try
            {
                await apiService.PostDataAsync($"{endpoint}/{productId}"); // response is a string
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public static async Task<string> ClearCart(string endpoint, IApiService apiService)
        {
            try
            {
                await apiService.DeleteDataAsync(endpoint); // response is a string
            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public static async Task<decimal> GetShoppingCartTotal(IApiService apiService)
        {
            var total = (await GetCartItems(GlobalConstants.GetCartItemsEndpoint, apiService))
                                                .Select(n => n.ProductPrice * n.Amount).Sum();
            return total;
        }
    }
}

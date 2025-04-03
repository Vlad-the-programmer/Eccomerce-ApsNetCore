using EcommerceRestApi.Models;
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.ComponentModel;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CartEndpointsHelperFuncs
    {
        public static async Task GetCreateCart(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            //var user = JsonSerializer.Deserialize<ApplicationUserViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
        }

        public static async Task<List<ShoppingCartItemVM>> GetCartItems(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var cartItems = JsonSerializer.Deserialize<List<ShoppingCartItemVM>>(response, 
                                                        GlobalConstants.JsonSerializerOptions); // Deserialize from string
            return cartItems ?? new List<ShoppingCartItemVM>();
        }

        public static async Task<string> AddItemToCart(string endpoint, IApiService apiService, ShoppingCartItemVM cartItem)
        {
            try
            {
                await apiService.PostDataAsync(endpoint, JsonSerializer.Serialize(cartItem)); // response is a string

            }
            catch (HttpRequestException ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public static async Task<string> RemoveItemFromCart(string endpoint,    
                                                            IApiService apiService, 
                                                            ShoppingCartItemVM cartItem)
        {
            try
            {
                await apiService.PostDataAsync(endpoint, JsonSerializer.Serialize(cartItem)); // response is a string
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

        public static async Task<double> GetShoppingCartTotal(IApiService apiService)
        {
            var total = ( await GetCartItems(GlobalConstants.GetCartItemsEndpoint, apiService) )
                                                .Select(n => n.Product.Price * n.Amount).Sum();
            return total;
        }
    }
}

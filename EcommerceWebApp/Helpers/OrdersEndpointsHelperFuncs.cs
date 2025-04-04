using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class OrdersEndpointsHelperFuncs
    {
        public async static Task<List<OrderViewModel>> GetOrders(string endpoint, IApiService apiService)
        {
            var orders = new List<OrderViewModel>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                orders = JsonSerializer.Deserialize<List<OrderViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            return orders;
        }


        public async static Task<NewProductViewModel?> GetOrderByCode(string endpoint, string code,  IApiService apiService)
        {
            var order = new OrderViewModel();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                order = JsonSerializer.Deserialize<OrderViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { order = null; }
            return order;
        }
    }
}

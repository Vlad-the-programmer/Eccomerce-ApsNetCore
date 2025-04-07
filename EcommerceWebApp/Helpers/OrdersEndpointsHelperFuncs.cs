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


        public async static Task<OrderViewModel?> GetOrderByCode(string endpoint, string code,  IApiService apiService)
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

        public async static Task<string> SubmitOrder(string endpoint, OrderViewModel order, IApiService apiService)
        {
                var response = await apiService.PostDataAsync(endpoint, JsonSerializer.Serialize(order)); // response is a string
                return response;
        }

        public async static Task<string> UpdateOrder(string endpoint, OrderViewModel order, IApiService apiService)
        {
            try
            {
                var response = await apiService.UpdateDataAsync(endpoint, JsonSerializer.Serialize(order)); // response is a string
                return response;
            }
            catch (HttpRequestException ex) { return ex.Message; }

        }

        public async static Task<string> CancelOrder(string endpoint, string code, IApiService apiService)
        {
            try
            {
                var response = await apiService.DeleteDataAsync(endpoint + code); // response is a string
                return response;
            }
            catch (HttpRequestException ex) { return ex.Message; }

        }

        public async static Task<string> DeleteOrder(string endpoint,string code, IApiService apiService)
        {
            try
            {
                var response = await apiService.DeleteDataAsync(endpoint + code); // response is a string
                return response;
            }
            catch (HttpRequestException ex) { return ex.Message; }

        }
    }
}

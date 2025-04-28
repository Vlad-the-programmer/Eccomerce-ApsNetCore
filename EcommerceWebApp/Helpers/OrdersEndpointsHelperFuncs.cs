using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.Dtos;
using System.Diagnostics;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class OrdersEndpointsHelperFuncs
    {
        public async static Task<List<OrderDTO>> GetOrders(string endpoint, IApiService apiService)
        {
            var orders = new List<OrderDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                orders = JsonSerializer.Deserialize<List<OrderDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex)
            {
                orders = new List<OrderDTO>();
            }

            return orders;
        }


        public async static Task<OrderDTO?> GetOrderByCode(string endpoint, string code, IApiService apiService)
        {
            var order = new OrderDTO();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}/{code}"); // response is a string
                order = JsonSerializer.Deserialize<OrderDTO>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine("OrderByCode: " + ex);
                order = new OrderDTO();
            }
            return order;
        }

        public async static Task<OrderViewModel?> GetOrderCreateTemplate(string endpoint, IApiService apiService)
        {
            var orderModel = new OrderViewModel();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                orderModel = JsonSerializer.Deserialize<OrderViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex)
            {
                orderModel = new OrderViewModel();
            }
            return orderModel;
        }

        public async static Task<string> SubmitOrder(string endpoint, OrderViewModel order, IApiService apiService)
        {
            var response = await apiService.PostDataAsync(endpoint, JsonSerializer.Serialize(order, GlobalConstants.JsonSerializerOptions)); // response is a string
            return response;
        }

        public async static Task<string> UpdateOrder(string endpoint, OrderViewModel order, IApiService apiService)
        {
            var response = await apiService.UpdateDataAsync(endpoint, JsonSerializer.Serialize(order, GlobalConstants.JsonSerializerOptions)); // response is a string
            return response;


        }

        public async static Task<string> CancelOrder(string endpoint, string code, IApiService apiService)
        {
            try
            {
                var response = await apiService.DeleteDataAsync($"{endpoint}/{code}"); // response is a string
                return response;
            }
            catch (HttpRequestException ex) { return ex.Message; }

        }
    }
}

using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers.HelperFuncs;
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

        public async static Task<NewOrderViewModel?> GetOrderCreateTemplate(string endpoint, IApiService apiService)
        {
            var orderModel = new NewOrderViewModel();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                orderModel = JsonSerializer.Deserialize<NewOrderViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex)
            {
                orderModel = new NewOrderViewModel();
            }
            return orderModel;
        }

        public async static Task<string> SubmitOrder(string endpoint, NewOrderViewModel order, IApiService apiService)
        {
            var response = await apiService.PostDataAsync(endpoint, JsonSerializer.Serialize(order, GlobalConstants.JsonSerializerOptions)); // response is a string
            return response;
        }

        public async static Task<string> UpdateOrder(string endpoint, NewOrderViewModel order, IApiService apiService)
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

        public async static Task<List<SearchComboBoxDto>> GetSearchComboBoxDtos(string endpoint, IApiService apiService)
        {
            var searchComboboxDtos = new List<SearchComboBoxDto>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint);
                searchComboboxDtos = JsonSerializer.Deserialize<List<SearchComboBoxDto>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { searchComboboxDtos = null; }
            return searchComboboxDtos;
        }

        public async static Task<List<SearchComboBoxDto>> GetOrderByComboBoxDtos(string endpoint, IApiService apiService)
        {
            var orderbyComboboxDtos = new List<SearchComboBoxDto>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint);
                orderbyComboboxDtos = JsonSerializer.Deserialize<List<SearchComboBoxDto>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { orderbyComboboxDtos = null; }
            return orderbyComboboxDtos;
        }

        public async static Task<List<OrderDTO>> GetFilteredOrders(
            string endpoint,
            string searchString,
            string? searchProperty,
            string? sortProperty,
            bool sortAscending,
            DateTime? fromDate,
            DateTime? toDate,
            IApiService apiService)
        {
            try
            {
                var queryString = QueryStringBuilder.BuildQueryString(
                    ("searchString", searchString),
                    ("searchProperty", searchProperty),
                    ("sortProperty", sortProperty),
                    ("sortAscending", sortAscending.ToString().ToLower()),
                    ("fromDate", fromDate?.ToString("yyyy-MM-dd")),
                    ("toDate", toDate?.ToString("yyyy-MM-dd"))
                );

                var fullUrl = $"{endpoint}{queryString}";
                Debug.WriteLine($"Filter Orders URL: {fullUrl}");

                var response = await apiService.GetDataAsync(fullUrl);
                var products = JsonSerializer.Deserialize<List<OrderDTO>>(response, GlobalConstants.JsonSerializerOptions);

                return products ?? new List<OrderDTO>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<OrderDTO>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                return new List<OrderDTO>();
            }
        }
    }
}

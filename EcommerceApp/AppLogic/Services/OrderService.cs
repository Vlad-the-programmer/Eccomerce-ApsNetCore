using System.Net.Http.Json;

namespace EcommerceApp.AppLogic.Services
{
    public static class OrdersService
    {
        public static async Task<IEnumerable<Dtos.OrderDto>> GetUserOrders(int customerId)
        {

            var response = await ApiService.Client.GetAsync(
                $"http://localhost:5000/api/orders/customer/{customerId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<Dtos.OrderDto>();

            return await response.Content.ReadFromJsonAsync<IEnumerable<Dtos.OrderDto>>();
        }

        public static async Task<Dtos.OrderDto> GetOrder(string code)
        {
            var response = await ApiService.Client.GetAsync(
                $"http://localhost:5000/api/orders/{code}");

            return await response.Content.ReadFromJsonAsync<Dtos.OrderDto>();
        }
    }

}

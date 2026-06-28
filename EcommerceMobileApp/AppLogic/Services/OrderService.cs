namespace EcommerceMobileApp.AppLogic.Services
{
    public static class OrdersService
    {
        public static async Task<IEnumerable<Dtos.OrderDto>> GetUserOrders(int? customerId)
        {
            if (!customerId.HasValue)
            {
                throw new Exception("No customer id");
            }

            var response = await ApiService.GetAsync<IEnumerable<Dtos.OrderDto>>(
                $"api/orders/customer/{customerId}");

            return response;
        }

        public static async Task<Dtos.OrderDto> GetOrder(string code)
        {
            var response = await ApiService.GetAsync<Dtos.OrderDto>(
                $"api/orders/{code}");

            return response;
        }
    }

}

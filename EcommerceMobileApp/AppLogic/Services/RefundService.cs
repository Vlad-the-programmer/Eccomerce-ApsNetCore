namespace EcommerceMobileApp.AppLogic.Services
{
    public class RefundService
    {
        public static async Task<IEnumerable<Dtos.RefundDto>> GetUserRefunds(int? customerId)
        {
            if (!customerId.HasValue)
            {
                throw new Exception("No customer id");
            }

            var response = await ApiService.GetAsync<IEnumerable<Dtos.RefundDto>>(
                $"api/refunds/customer/{customerId}");

            return response;
        }

        public static async Task<Dtos.RefundDto> GetRefund(string code)
        {
            var response = await ApiService.GetAsync<Dtos.RefundDto>(
                $"api/refunds/{code}");

            return response;
        }
    }
}

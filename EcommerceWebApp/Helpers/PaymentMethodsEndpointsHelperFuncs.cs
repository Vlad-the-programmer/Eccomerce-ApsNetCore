using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class PaymentMethodsEndpointsHelperFuncs
    {
        public async static Task<List<PaymentMethodDTO>> GetPaymentMethods(string endpoint, IApiService apiService)
        {
            var methods = new List<PaymentMethodDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                methods = JsonSerializer.Deserialize<List<PaymentMethodDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex)
            {
                methods = new List<PaymentMethodDTO>();
            }

            return methods;
        }
    }
}

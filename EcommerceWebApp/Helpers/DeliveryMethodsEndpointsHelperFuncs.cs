using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class DeliveryMethodsEndpointsHelperFuncs
    {
        public async static Task<List<DeliveryMethodDTO>> GetDeliveryMethods(string endpoint, IApiService apiService)
        {
            var methods = new List<DeliveryMethodDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                methods = JsonSerializer.Deserialize<List<DeliveryMethodDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex)
            {
                methods = new List<DeliveryMethodDTO>();
            }

            return methods;
        }
    }
}

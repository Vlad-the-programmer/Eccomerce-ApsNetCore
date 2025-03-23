using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class AccountEndpointsHelperFuncs
    {
        public async static Task<ApplicationUserViewModel> GetCurrentUserObj(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var user = JsonSerializer.Deserialize<ApplicationUserViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            return user == null ? new ApplicationUserViewModel() : user;
        }



    }
}

        

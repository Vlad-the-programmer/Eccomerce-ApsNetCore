using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.UpdateViewModels;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class AccountEndpointsHelperFuncs
    {
        //public async static Task<ApplicationUserViewModel> GetCurrentUserObj(string endpoint, IApiService apiService)
        //{
        //    var response = await apiService.GetDataAsync(endpoint); // response is a string

        //    var user = JsonSerializer.Deserialize<ApplicationUserViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

        //    return user == null ? new ApplicationUserViewModel() : user;
        //}

        public async static Task<UserUpdateVM> GetUserUpdateModelObj(string endpoint, IApiService apiService)
        {
            var model = new UserUpdateVM();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string

                model = JsonSerializer.Deserialize<UserUpdateVM>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException e)
            {
                model = new UserUpdateVM();
            }

            return model;
        }

    }
}



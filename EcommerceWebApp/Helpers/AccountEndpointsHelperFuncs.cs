using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Helpers.Enums;
using EcommerceWebApp.Models;
using EcommerceWebApp.Models.AppViewModels;
using EcommerceWebApp.Models.UpdateViewModels;
using System.Diagnostics;
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

        public async static Task<StaffUpdateVM> GetStaffUpdateModelObj(string endpoint, IApiService apiService)
        {
            var model = new StaffUpdateVM();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string

                model = JsonSerializer.Deserialize<StaffUpdateVM>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException e)
            {
                model = new StaffUpdateVM();
            }

            return model;
        }

        public async static Task<ApplicationUserViewModel> GetUserProfile(string endpoint, IApiService apiService)
        {
            var model = new ApplicationUserViewModel();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string

                model = JsonSerializer.Deserialize<ApplicationUserViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException e)
            {
                model = new ApplicationUserViewModel();
            }

            return model;
        }

        public async static Task<List<CurrentUserDTO>> GetFilteredStaff(
          string endpoint,
          string searchString,
          string? searchProperty,
          string? sortProperty,
          UserStatusFilter filterStatus,
          bool sortAscending,
          IApiService apiService)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchString))
                    queryParams.Add($"searchString={Uri.EscapeDataString(searchString)}");

                if (!string.IsNullOrEmpty(searchProperty))
                    queryParams.Add($"searchProperty={Uri.EscapeDataString(searchProperty)}");

                if (!string.IsNullOrEmpty(sortProperty))
                    queryParams.Add($"sortProperty={Uri.EscapeDataString(sortProperty)}");

                queryParams.Add($"sortAscending={sortAscending.ToString().ToLower()}");

                string statusFilterValue = filterStatus switch
                {
                    UserStatusFilter.ActiveOnly => "active",
                    UserStatusFilter.InactiveOnly => "inactive",
                    _ => "all"
                };
                queryParams.Add($"statusFilter={statusFilterValue}");

                var queryString = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : "";
                var fullUrl = $"{endpoint}{queryString}";

                var response = await apiService.GetDataAsync(fullUrl);

                var staff = JsonSerializer.Deserialize<List<CurrentUserDTO>>(response, GlobalConstants.JsonSerializerOptions);

                return staff ?? new List<CurrentUserDTO>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<CurrentUserDTO>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                return new List<CurrentUserDTO>();
            }
        }
    }
}



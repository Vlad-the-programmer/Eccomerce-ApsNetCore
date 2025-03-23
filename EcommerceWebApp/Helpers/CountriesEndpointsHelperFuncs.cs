using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CountriesEndpointsHelperFuncs
    {
        public async static Task<List<CountryViewModel>> GetCountries(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var countries = JsonSerializer.Deserialize<List<CountryViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            return countries == null ? new List<CountryViewModel>() : countries;
        }

        public async static Task<List<string>> GetCountriesNames(string endpoint, IApiService apiService)
        {
            return (await GetCountries(endpoint, apiService)).Select(c => c.CountryName).ToList();
        }
    }
}

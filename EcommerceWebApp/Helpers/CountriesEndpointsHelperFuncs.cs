using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CountriesEndpointsHelperFuncs
    {
        public async static Task<List<CountryViewModel>> GetCountries(string endpoint, IApiService apiService)
        {
            var countries = new List<CountryViewModel>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string

                countries = JsonSerializer.Deserialize<List<CountryViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { }
            return countries;
        }

        public async static Task<List<string>> GetCountriesNames(string endpoint, IApiService apiService)
        {
            var countriesNames = new List<string>();
            try
            {
                countriesNames = (await GetCountries(endpoint, apiService)).Select(c => c.CountryName).ToList();
            }
            catch (ArgumentNullException ex)
            {
                countriesNames = new List<string>();
            }
            return countriesNames;
        }
    }
}

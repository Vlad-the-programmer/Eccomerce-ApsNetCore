using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CategoriesEndpointsHelperFuncs
    {
        public async static Task<List<CategoryViewModel>> GetCategories(string endpoint, IApiService apiService)
        {
            var categories = new List<CategoryViewModel>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            } catch (HttpRequestException ex) { }


            return categories;
        }

        public async static Task<List<SubcategoryViewModel>> GetSubCategories(string endpoint, IApiService apiService)
        {
            var subCategories = new List<SubcategoryViewModel>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                subCategories = JsonSerializer.Deserialize<List<SubcategoryViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            return subCategories;
        }

        public static Dictionary<string, string> GetCategoriesDictionaryWithNameCodeFields(List<CategoryViewModel> categories)
        {
            return categories.Count > 0 && categories.FirstOrDefault()?.Name != null ? categories.ToDictionary(c => c.Name, c => c.Code) : new Dictionary<string, string>() ;
        }

        public static Dictionary<string, string> GetSubCategoriesDictionaryWithNameCodeFields(List<SubcategoryViewModel> subCategories)
        {
            return subCategories.Count > 0 && subCategories.FirstOrDefault()?.Name != null ? subCategories.ToDictionary(c => c.Name, c => c.Code) : new Dictionary<string, string>();
        }
    }
}

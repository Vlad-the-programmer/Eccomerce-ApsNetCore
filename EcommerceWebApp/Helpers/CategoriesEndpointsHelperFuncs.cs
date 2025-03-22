using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CategoriesEndpointsHelperFuncs
    {
        public async static Task<List<CategoryViewModel>> GetCategories(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(response); // Deserialize from string

            return categories == null ? new List<CategoryViewModel>() : categories;
        }

        public async static Task<List<SubcategoryViewModel>> GetSubCategories(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var subCategories = JsonSerializer.Deserialize<List<SubcategoryViewModel>>(response); // Deserialize from string

            return subCategories == null ? new List<SubcategoryViewModel>() : subCategories;
        }

        public static Dictionary<string, string> GetCategoriesDictionaryWithNameCodeFields(List<CategoryViewModel> categories)
        {
            return categories.ToDictionary(c => c.Name, c => c.Code);
        }

    }
}

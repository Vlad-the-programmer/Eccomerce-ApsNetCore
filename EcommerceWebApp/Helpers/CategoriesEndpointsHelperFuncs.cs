using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class CategoriesEndpointsHelperFuncs
    {
        public async static Task<List<CategoryDTO>> GetCategories(string endpoint, IApiService apiService)
        {
            var categories = new List<CategoryDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                categories = JsonSerializer.Deserialize<List<CategoryDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            } catch (HttpRequestException ex) { }


            return categories;
        }

        public async static Task<List<SubcategoryDTO>> GetSubCategories(string endpoint, IApiService apiService)
        {
            var subCategories = new List<SubcategoryDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                subCategories = JsonSerializer.Deserialize<List<SubcategoryDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            return subCategories;
        }

        public static Dictionary<string, string> GetCategoriesDictionaryWithNameCodeFields(List<CategoryDTO> categories)
        {
            return categories.Count > 0 && categories.FirstOrDefault()?.Name != null ? categories.ToDictionary(c => c.Name, c => c.Code) : new Dictionary<string, string>() ;
        }

        public static Dictionary<string, string> GetSubCategoriesDictionaryWithNameCodeFields(List<SubcategoryDTO> subCategories)
        {
            return subCategories.Count > 0 && subCategories.FirstOrDefault()?.Name != null ? subCategories.ToDictionary(c => c.Name, c => c.Code) : new Dictionary<string, string>();
        }
    }
}

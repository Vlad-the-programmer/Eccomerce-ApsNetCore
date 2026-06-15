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
            }
            catch (HttpRequestException ex) { }


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


        public async static Task<CategoryDTO?> GetCategoryById(string endpoint, int id, IApiService apiService)
        {
            var category = new CategoryDTO();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}/{id}");
                category = JsonSerializer.Deserialize<CategoryDTO>(response, GlobalConstants.JsonSerializerOptions);
            }
            catch (HttpRequestException ex)
            {
                category = null;
            }
            return category;
        }

        public async static Task<SubcategoryDTO?> GetSubCategoryById(string endpoint, int id, IApiService apiService)
        {
            var category = new SubcategoryDTO();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}/{id}");
                category = JsonSerializer.Deserialize<SubcategoryDTO>(response, GlobalConstants.JsonSerializerOptions);
            }
            catch (HttpRequestException ex)
            {
                category = null;
            }
            return category;
        }

        public static Dictionary<string, string> GetCategoriesDictionaryWithNameCodeFields(List<CategoryDTO> categories)
        {
            return categories.Count > 0 && categories.FirstOrDefault()?.Name != null ? categories.ToDictionary(c => c.Name, c => c.Code) : new Dictionary<string, string>();
        }

        public static Dictionary<string, string> GetSubCategoriesDictionaryWithNameCodeFields(List<SubcategoryDTO> subCategories)
        {
            return subCategories.Count > 0 && subCategories.FirstOrDefault()?.Name != null ? subCategories.ToDictionary(c => c.Name, c => c.Code) : new Dictionary<string, string>();
        }

        public async static Task<string> SubmitCategory(string endpoint, CategoryDTO category, IApiService apiService)
        {
            var response = await apiService.PostDataAsync(endpoint, JsonSerializer.Serialize(category, GlobalConstants.JsonSerializerOptions)); // response is a string
            return response;
        }

        public async static Task<string> UpdateCategory(string endpoint, CategoryDTO category, IApiService apiService)
        {
            var response = await apiService.UpdateDataAsync(endpoint, JsonSerializer.Serialize(category, GlobalConstants.JsonSerializerOptions)); // response is a string
            return response;
        }

        public async static Task<string> DeleteCategory(string endpoint, string code, IApiService apiService)
        {
            try
            {
                var response = await apiService.DeleteDataAsync($"{endpoint}/{code}"); // response is a string
                return response;
            }
            catch (HttpRequestException ex) { return ex.Message; }

        }
    }
}

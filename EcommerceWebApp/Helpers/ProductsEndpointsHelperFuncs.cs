using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class ProductsEndpointsHelperFuncs
    {
        public async static Task<List<ProductDTO>> GetProducts(string endpoint, IApiService apiService)
        {
            var products = new List<ProductDTO>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                products = JsonSerializer.Deserialize<List<ProductDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex)
            {
                products = new List<ProductDTO>();
            }

            return products;
        }

        public async static Task<ProductDTO?> GetFeaturedProduct(string endpoint, IApiService apiService)
        {
            var activeProducts = new List<ProductDTO>();
            var featuredProduct = new ProductDTO();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                activeProducts = JsonSerializer.Deserialize<List<ProductDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            if (activeProducts != null && activeProducts.Count > 0)
            {
                var random = new Random();
                featuredProduct = activeProducts[random.Next(activeProducts.Count)];
            }
            else
            {
                featuredProduct = null;
            }
            return featuredProduct;
        }


        public async static Task<ProductDTO?> GetProductById(string endpoint, IApiService apiService)
        {
            var product = new ProductDTO();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                product = JsonSerializer.Deserialize<ProductDTO?>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { product = null; }
            return product;
        }

        public async static Task<List<ProductDTO>> GetFilteredProducts(string endpoint, string searchString, IApiService apiService)
        {
            var products = new List<ProductDTO>();
            try
            {
                var response = await apiService.GetDataAsync($"{endpoint}?searchString={searchString}"); // response is a string
                products = JsonSerializer.Deserialize<List<ProductDTO>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            }
            catch (HttpRequestException ex) { }

            return products;
        }
    }
}

using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class ProductsEndpointsHelperFuncs
    {
        public async static Task<List<NewProductViewModel>> GetProducts(string endpoint, IApiService apiService)
        {
            var products = new List<NewProductViewModel>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                products = JsonSerializer.Deserialize<List<NewProductViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
            } catch(HttpRequestException ex) { }

            return products;
        }

        public async static Task<NewProductViewModel?> GetFeaturedProduct(string endpoint, IApiService apiService)
        {
            var activeProducts = new List<NewProductViewModel>();
            var featuredProduct = new NewProductViewModel();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                activeProducts = JsonSerializer.Deserialize<List<NewProductViewModel>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string
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


        public async static Task<NewProductViewModel?> GetProductById(string endpoint, IApiService apiService)
        {
            var product = new NewProductViewModel();
            try
            {
                var response = await apiService.GetDataAsync(endpoint); // response is a string
                product = JsonSerializer.Deserialize<NewProductViewModel>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            } catch(HttpRequestException ex) { product = null; }
            return product;
        }
    }
}

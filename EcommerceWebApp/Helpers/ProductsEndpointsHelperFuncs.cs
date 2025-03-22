using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models;
using System.Text.Json;

namespace EcommerceWebApp.Helpers
{
    public class ProductsEndpointsHelperFuncs
    {
        public async static Task<List<NewProductViewModel>> GetProducts(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var products = JsonSerializer.Deserialize<List<NewProductViewModel>>(response); // Deserialize from string

            return products == null ? new List<NewProductViewModel>() : products;
        }

        public async static Task<NewProductViewModel> GetFeaturedProduct(string endpoint, IApiService apiService)
        {

            var response = await apiService.GetDataAsync(endpoint); // response is a string

            var activeProducts = JsonSerializer.Deserialize<List<NewProductViewModel>>(response); // Deserialize from string

            var featuredProduct = new NewProductViewModel();
            if (activeProducts != null && activeProducts.Count > 0)
            {
                var random = new Random();
                featuredProduct = activeProducts[random.Next(activeProducts.Count)];
            }
            return featuredProduct;
        }


        public async static Task<NewProductViewModel> GetProductById(string endpoint, IApiService apiService)
        {
            var response = await apiService.GetDataAsync(endpoint); // response is a string
            var product = JsonSerializer.Deserialize<NewProductViewModel>(response); // Deserialize from string
            return product != null ? product : new NewProductViewModel();
        }
    }
}

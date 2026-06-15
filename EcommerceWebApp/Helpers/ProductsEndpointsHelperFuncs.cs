using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using System.Diagnostics;
using System.Globalization;
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

        public async static Task<List<SearchComboBoxDto>> GetSearchComboBoxDtos(string endpoint, IApiService apiService)
        {
            var searchComboboxDtos = new List<SearchComboBoxDto>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint);
                searchComboboxDtos = JsonSerializer.Deserialize<List<SearchComboBoxDto>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { searchComboboxDtos = null; }
            return searchComboboxDtos;
        }

        public async static Task<List<SearchComboBoxDto>> GetOrderByComboBoxDtos(string endpoint, IApiService apiService)
        {
            var orderbyComboboxDtos = new List<SearchComboBoxDto>();
            try
            {
                var response = await apiService.GetDataAsync(endpoint);
                orderbyComboboxDtos = JsonSerializer.Deserialize<List<SearchComboBoxDto>>(response, GlobalConstants.JsonSerializerOptions); // Deserialize from string

            }
            catch (HttpRequestException ex) { orderbyComboboxDtos = null; }
            return orderbyComboboxDtos;
        }

        public async static Task<List<ProductDTO>> GetFilteredProducts(
          string endpoint,
          string searchString,
          string? searchProperty,
          string? sortProperty,
          decimal? fromPrice,
          decimal? ToPrice,
          string? categoryCode,
          string? subcategoryCode,
          int? minRating,
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

                if (fromPrice.HasValue)
                    queryParams.Add($"fromPrice={fromPrice.Value.ToString(CultureInfo.InvariantCulture)}");

                if (ToPrice.HasValue)
                    queryParams.Add($"toPrice={ToPrice.Value.ToString(CultureInfo.InvariantCulture)}");

                if (!string.IsNullOrEmpty(categoryCode))
                    queryParams.Add($"categoryCode={Uri.EscapeDataString(categoryCode)}");

                if (!string.IsNullOrEmpty(subcategoryCode))
                    queryParams.Add($"subcategoryCode={Uri.EscapeDataString(subcategoryCode)}");

                if (minRating.HasValue)
                    queryParams.Add($"minRating={minRating.Value.ToString(CultureInfo.InvariantCulture)}");


                var queryString = queryParams.Any() ? $"?{string.Join("&", queryParams)}" : "";
                var fullUrl = $"{endpoint}{queryString}";

                var response = await apiService.GetDataAsync(fullUrl);

                var products = JsonSerializer.Deserialize<List<ProductDTO>>(response, GlobalConstants.JsonSerializerOptions);

                return products ?? new List<ProductDTO>();
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"HTTP Error: {ex.Message}");
                return new List<ProductDTO>();
            }
            catch (JsonException ex)
            {
                Debug.WriteLine($"JSON Deserialization Error: {ex.Message}");
                return new List<ProductDTO>();
            }
        }
    }
}

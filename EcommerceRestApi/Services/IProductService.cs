using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using EcommerceRestApi.Services.Base;
using Inventory_Management_Sustem.Models.Dtos;

namespace EcommerceRestApi.Services
{
    public interface IProductsService : IEntityBaseRepository<Product>
    {
        Task<ProductDto> GetProductByIDAsync(int id);
        Task<List<ProductDto>> GetProducts();

        Task AddNewProductAsync(NewProductViewModel data);

        Task UpdateProductAsync(int id, ProductUpdateVM data);
        Task<List<ProductDto>> FilterProductsAsync(string searchString, string? searchProperty, string? sortProperty, decimal? fromPrice,
               decimal? ToPrice, string? categoryCode, string? subcategoryCode, int? minRating, bool sortAscending);
        List<SearchComboBoxDto> GetSearchComboBoxDtos();
        List<SearchComboBoxDto> GetOrderByComboBoxDtos();
        Task<int> IncreaseStockAsync(int productId, int quantityToAdd);
    }
}

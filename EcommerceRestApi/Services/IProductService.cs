using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
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
    }
}

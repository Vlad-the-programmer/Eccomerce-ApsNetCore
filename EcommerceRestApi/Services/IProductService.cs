using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;

namespace EcommerceRestApi.Services
{
    public interface IProductsService : IEntityBaseRepository<Product>
    {
        Task<Product> GetProductByIDAsync(int id);
        //Task<NewProductsDropDownsVm> GetNewProductsDropDownValues();

        Task AddNewProductAsync(NewProductViewModel data);

        Task UpdateProductAsync(int id, ProductUpdateVM data);
    }
}

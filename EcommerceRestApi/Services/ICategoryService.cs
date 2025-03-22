using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface ICategoryService : IEntityBaseRepository<Category>
    {
        Task<Category> GetCategoryByIDAsync(int id);

        Task AddNewCategoryAsync(CategoryViewModel data);

        Task UpdateCategoryAsync(int id, CategoryUpdateVM data);
    }
}

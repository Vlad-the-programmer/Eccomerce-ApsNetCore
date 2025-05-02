using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface ICategoryService : IEntityBaseRepository<Category>
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task<Category> GetCategoryByIDAsync(int id);

        Task AddNewCategoryAsync(CategoryDTO data);

        Task UpdateCategoryAsync(int id, CategoryUpdateVM data);
    }
}

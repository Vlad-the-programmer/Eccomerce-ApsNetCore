using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface ICategoryService : IEntityBaseRepository<Category>
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task<CategoryDTO> GetCategoryByIDAsync(int id);

        Task AddNewCategoryAsync(NewCategoryVM data);

        Task UpdateCategoryAsync(int id, CategoryUpdateVM data);
        Task<List<CategoryDTO>> GetAllCategoriesForAdmin();
        Task DeleteAsync(int id);
    }
}

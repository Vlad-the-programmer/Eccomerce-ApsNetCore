using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface ISubcategoryService : IEntityBaseRepository<Subcategory>
    {
        Task<List<SubcategoryDTO>> GetAllSubcategories();
        Task<SubcategoryDTO> GetSubcategoryByCodeAsync(string code);
        Task<SubcategoryDTO> GetSubcategoryByIdAsync(int id);

        Task AddNewSubCategoryAsync(NewSubcategoryVM data);

        Task UpdateSubCategoryAsync(int id, SubcategoryUpdateVM data);
        //Task DeleteSubCategoryAsync(string code);

    }
}

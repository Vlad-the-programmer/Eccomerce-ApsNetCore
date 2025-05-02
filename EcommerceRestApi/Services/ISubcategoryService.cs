using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface ISubcategoryService : IEntityBaseRepository<Subcategory>
    {
        Task<List<SubcategoryDTO>> GetAllSubcategories();
        Task<SubcategoryDTO> GetSubcategoryByCodeAsync(string code);

        Task AddNewSubCategoryAsync(SubcategoryDTO data);

        Task UpdateSubCategoryAsync(int id, SubcategoryDTO data);
    }
}

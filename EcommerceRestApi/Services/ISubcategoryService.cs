using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;
using EcommerceWebApp.Models;

namespace EcommerceRestApi.Services
{
    public interface ISubcategoryService : IEntityBaseRepository<Subcategory>
    {
        Task<List<SubcategoryViewModel>> GetAllSubcategories();
        Task<SubcategoryViewModel> GetSubcategoryByCodeAsync(string code);

        Task AddNewSubCategoryAsync(SubcategoryViewModel data);

        Task UpdateSubCategoryAsync(int id, SubcategoryViewModel data);
    }
}

using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services.Base;
using EcommerceWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class SubCategoryService : EntityBaseRepository<Subcategory>, ISubcategoryService
    {
        private readonly AppDbContext _context;

        public SubCategoryService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task AddNewSubCategoryAsync(SubcategoryViewModel data)
        {
            throw new NotImplementedException();
        }

        public async Task<List<SubcategoryViewModel>> GetAllSubcategories()
        {
            return (await _context.Subcategories.ToListAsync())
                        .Select(sc => new SubcategoryViewModel
                        {
                            Code = sc.Code,
                            CategoryId = sc.CategoryId,
                            About = sc.About,
                            Name = sc.Name,
                            Products = sc.Products.Select(p => NewProductViewModel.FromProductToVm(p)).ToList(),
                        }).ToList();
        }

        public async Task<SubcategoryViewModel> GetSubcategoryByCodeAsync(string code)
        {
            var subCategory = await _context.Subcategories.FirstAsync(sc => sc.Code == code);
            return new SubcategoryViewModel
            {
                Code = subCategory.Code,
                CategoryId = subCategory.CategoryId,
                About = subCategory.About,
                Name = subCategory.Name,
                Products = (IList<Helpers.Data.ViewModels.NewProductViewModel>)subCategory.Products,
            };

        }

        public Task UpdateSubCategoryAsync(int id, SubcategoryViewModel data)
        {
            throw new NotImplementedException();
        }
    }
}

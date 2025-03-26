using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;
using EcommerceRestApi.Helpers.Data.ViewModels;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;

namespace EcommerceRestApi.Services
{
    public class CategoryService : EntityBaseRepository<Category>, ICategoryService
    {

        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task AddNewCategoryAsync(CategoryViewModel data)
        {
            var newCategory = new Category()
            {
                Code = data.Code,
                About = data.About,
                Name = data.Name,
                IsActive = true,
                DateCreated = DateTime.Now,
            };

            var subCategory = _context.Subcategories.FirstOrDefault(sc => sc.Id == data.SubcategoryId);
            if (subCategory != null)
            {
                newCategory.Subcategories.Add(subCategory);
            }

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
        }

        public Task<List<CategoryViewModel>> GetAllCategories()
        {
            return _context.Categories.Select(c => new CategoryViewModel()
            {
                Code = c.Code,
                Name = c.Name,
                About = c.About,
                Id = c.Id,
                SubcategoryId = c.Subcategories.FirstOrDefault() != null ? c.Subcategories.First().Id : 0,
            }).ToListAsync();
        }

        public async Task<Category> GetCategoryByIDAsync(int id)
        {
            var category = await _context.Categories
                .Include(C => C.Subcategories)
                .Include(C => C.ProductCategories)
                .ThenInclude(SC => SC.Product)
            .FirstOrDefaultAsync(n => n.Id == id);

            return category;
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateVM data)
        {

            var dbCategory = await _context.Categories.FirstOrDefaultAsync(n => n.Id == id);
            if (dbCategory != null)
            {

                dbCategory.Name = data.Name;
                dbCategory.Code = data.Code;
                dbCategory.About = data.About;
                dbCategory.IsActive = data.IsActive;
                dbCategory.DateUpdated = DateTime.Now;

                var subCategory = _context.Subcategories.FirstOrDefault(sc => sc.Id == data.SubcategoryId);
                if (subCategory != null)
                {
                    dbCategory.Subcategories.First().About = subCategory.About;
                    dbCategory.Subcategories.First().Code = subCategory.Code;
                    dbCategory.Subcategories.First().Name = subCategory.Name;
                    dbCategory.Subcategories.First().CategoryId = dbCategory.Id;
                    dbCategory.Subcategories.First().IsActive = dbCategory.IsActive;
                    dbCategory.DateUpdated = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
        }

    }
    }

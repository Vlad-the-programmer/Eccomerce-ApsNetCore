using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Inventory_Management_Sustem.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class CategoryService : EntityBaseRepository<Category>, ICategoryService
    {

        private readonly AppDbContext _context;
        public CategoryService(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task AddNewCategoryAsync(NewCategoryVM data)
        {

            var code = $"CAT{_context.Categories.Count() + 1}";
            var newCategory = new Category()
            {
                Code = code,
                About = data.About,
                Name = data.Name,
                IsActive = true,
                DateCreated = DateTime.Now
            };

            var subCategory = _context.Subcategories.FirstOrDefault(sc => sc.Code == data.SubcategoryCode);
            if (subCategory != null)
            {
                newCategory.Subcategories.Add(subCategory);
            }

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            return await _context.Categories
                .Where(c => c.DateDeleted == null)
                .Select(c => new CategoryDTO()
                {
                    Code = c.Code,
                    Name = c.Name,
                    About = c.About,
                    Id = c.Id,
                    IsActive = c.IsActive,
                    DateDeleted = c.DateDeleted,
                    SubcategoryId = c.Subcategories.FirstOrDefault() != null ? c.Subcategories.First().Id : 0,
                    Subcategories = c.Subcategories.Select(sc => new SubcategoryDTO
                    {
                        Id = sc.Id,
                        Code = sc.Code,
                        CategoryId = sc.CategoryId,
                        About = sc.About,
                        Name = sc.Name,
                        IsActive = sc.IsActive,
                        DateDeleted = sc.DateDeleted,
                        Products = sc.Products.Select(p => new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Brand = p.Brand,
                            Code = p.Code,
                            Price = p.Price,
                            Stock = p.Stock,
                            Photo = p.Photo,
                            OtherPhotos = p.OtherPhotos,
                            About = p.About,
                            LongAbout = p.LongAbout,
                            RatingSum = p.RatingSum,
                            RatingVotes = p.RatingVotes,
                            SubcategoryCode = p.Subcategory != null ? p.Subcategory.Code : string.Empty,
                            CategoryCode = p.ProductCategories.FirstOrDefault() != null ?
                                  p.ProductCategories.First().Category.Code : string.Empty,
                            IsActive = p.IsActive,
                        }).ToList(),
                    }).ToList(),
                }).ToListAsync();
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesForAdmin()
        {
            return await _context.Categories
                .Select(c => new CategoryDTO()
                {
                    Code = c.Code,
                    Name = c.Name,
                    About = c.About,
                    Id = c.Id,
                    IsActive = c.IsActive,
                    DateDeleted = c.DateDeleted,
                    SubcategoryId = c.Subcategories.FirstOrDefault() != null ? c.Subcategories.First().Id : 0,
                    Subcategories = c.Subcategories.Select(sc => new SubcategoryDTO
                    {
                        Id = sc.Id,
                        Code = sc.Code,
                        CategoryId = sc.CategoryId,
                        About = sc.About,
                        Name = sc.Name,
                        IsActive = sc.IsActive,
                        DateDeleted = sc.DateDeleted,
                        Products = sc.Products.Select(p => new ProductDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Brand = p.Brand,
                            Code = p.Code,
                            Price = p.Price,
                            Stock = p.Stock,
                            Photo = p.Photo,
                            OtherPhotos = p.OtherPhotos,
                            About = p.About,
                            LongAbout = p.LongAbout,
                            RatingSum = p.RatingSum,
                            RatingVotes = p.RatingVotes,
                            SubcategoryCode = p.Subcategory != null ? p.Subcategory.Code : string.Empty,
                            CategoryCode = p.ProductCategories.FirstOrDefault() != null ?
                                  p.ProductCategories.First().Category.Code : string.Empty,
                            IsActive = p.IsActive,
                        }).ToList(),
                    }).ToList(),
                }).ToListAsync();
        }

        public async Task<CategoryDTO> GetCategoryByIDAsync(int id)
        {
            var category = await _context.Categories
             .Select(c => new CategoryDTO()
             {
                 Code = c.Code,
                 Name = c.Name,
                 About = c.About,
                 Id = c.Id,
                 IsActive = c.IsActive,
                 DateDeleted = c.DateDeleted,
                 SubcategoryId = c.Subcategories.FirstOrDefault() != null ? c.Subcategories.First().Id : 0,
                 Subcategories = c.Subcategories.Select(sc => new SubcategoryDTO
                 {
                     Id = sc.Id,
                     Code = sc.Code,
                     CategoryId = sc.CategoryId,
                     About = sc.About,
                     Name = sc.Name,
                     IsActive = sc.IsActive,
                     DateDeleted = sc.DateDeleted,
                     Products = sc.Products.Select(p => new ProductDto
                     {
                         Id = p.Id,
                         Name = p.Name,
                         Brand = p.Brand,
                         Code = p.Code,
                         Price = p.Price,
                         Stock = p.Stock,
                         Photo = p.Photo,
                         OtherPhotos = p.OtherPhotos,
                         About = p.About,
                         LongAbout = p.LongAbout,
                         RatingSum = p.RatingSum,
                         RatingVotes = p.RatingVotes,
                         SubcategoryCode = p.Subcategory != null ? p.Subcategory.Code : string.Empty,
                         CategoryCode = p.ProductCategories.FirstOrDefault() != null ?
                               p.ProductCategories.First().Category.Code : string.Empty,
                         IsActive = p.IsActive,
                     }).ToList(),
                 }).ToList(),
             }
                )
            .FirstOrDefaultAsync(n => n.Id == id);

            return category;
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateVM data)
        {

            var dbCategory = await _context.Categories.FirstOrDefaultAsync(n => n.Id == id);
            if (dbCategory != null)
            {
                var hasChanges = false;

                if (data.About != null && dbCategory.About != data.About)
                {
                    dbCategory.About = data.About;
                    hasChanges = true;
                }

                if (data.Code != null && dbCategory.Code != data.Code)
                {
                    dbCategory.Code = data.Code;
                    hasChanges = true;
                }

                if (data.Name != null && dbCategory.Name != data.Name)
                {
                    dbCategory.Name = data.Name;
                    hasChanges = true;
                }

                if (dbCategory.IsActive != data.IsActive)
                {
                    dbCategory.IsActive = data.IsActive;

                    if (data.IsActive)
                    {
                        dbCategory.DateDeleted = null;
                        _context.Entry(dbCategory).Property(x => x.DateDeleted).IsModified = true;
                    }

                    hasChanges = true;
                }
                else if (data.IsActive == true && dbCategory.IsActive == false)
                {
                    dbCategory.IsActive = data.IsActive;
                    _context.Entry(dbCategory).Property(x => x.IsActive).IsModified = true;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    dbCategory.DateUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Subcategories)
                .Include(c => c.ProductCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return;

            category.IsActive = false;
            category.DateDeleted = DateTime.Now;

            foreach (var sc in category.Subcategories)
            {
                sc.IsActive = false;
                sc.DateDeleted = DateTime.Now;
            }

            foreach (var pc in category.ProductCategories)
            {
                pc.IsActive = false;
                pc.DateDeleted = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

    }
}

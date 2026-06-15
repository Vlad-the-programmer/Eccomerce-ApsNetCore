using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Inventory_Management_Sustem.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class SubCategoryService : EntityBaseRepository<Subcategory>, ISubcategoryService
    {
        private readonly AppDbContext _context;
        private readonly ICategoryService _categoryService;


        public SubCategoryService(AppDbContext context, ICategoryService categoryService) : base(context)
        {
            _context = context;
            _categoryService = categoryService;
        }

        public async Task AddNewSubCategoryAsync(NewSubcategoryVM data)
        {

            var code = $"SUBCAT{_context.Subcategories.Count() + 1}";

            var dbCategory = await _context.Categories.FirstOrDefaultAsync(n => n.Code == data.CategoryCode);
            if (dbCategory != null)
            {
                var newSubCategory = new Subcategory()
                {
                    Code = code,
                    About = data.About,
                    Name = data.Name,
                    CategoryId = dbCategory.Id,
                    IsActive = true,
                    DateCreated = DateTime.Now,
                };

                await _context.Subcategories.AddAsync(newSubCategory);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<SubcategoryDTO>> GetAllSubcategories()
        {
            return await _context.Subcategories
                        .Select(sc => new SubcategoryDTO
                        {
                            Id = sc.Id,
                            Code = sc.Code,
                            CategoryId = sc.CategoryId,
                            About = sc.About,
                            Name = sc.Name,
                            IsActive = sc.IsActive,
                            DateDeleted = sc.DateDeleted,
                            Category = new CategoryDTO()
                            {
                                Code = sc.Category.Code,
                                Name = sc.Category.Name,
                                About = sc.Category.About,
                                Id = sc.Category.Id,
                                IsActive = sc.Category.IsActive,
                                DateDeleted = sc.Category.DateDeleted,
                                SubcategoryId = sc.Id
                            },
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
                        }).ToListAsync();
        }

        public async Task<SubcategoryDTO> GetSubcategoryByCodeAsync(string code)
        {
            return await _context.Subcategories
                                            .Select(sc => new SubcategoryDTO
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
                                            })
                                            .FirstAsync(sc => sc.Code == code);
        }

        public async Task<SubcategoryDTO?> GetSubcategoryByIdAsync(int id)
        {
            return await _context.Subcategories
                                    .Select(sc => new SubcategoryDTO
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
                                    })
                                    .FirstOrDefaultAsync(sc => sc.Id == id);
        }

        public async Task UpdateSubCategoryAsync(int id, SubcategoryUpdateVM data)
        {
            var dbCategory = await _context.Categories.FirstOrDefaultAsync(n => n.Code == data.CategoryCode);
            if (dbCategory == null) return;

            var subCategory = await _context.Subcategories.FirstOrDefaultAsync(sc => sc.Id == id);
            if (subCategory != null)
            {
                var hasChanges = false;

                if (data.About != null && subCategory.About != data.About)
                {
                    subCategory.About = data.About;
                    hasChanges = true;
                }

                if (data.Code != null && subCategory.Code != data.Code)
                {
                    subCategory.Code = data.Code;
                    hasChanges = true;
                }

                if (data.Name != null && subCategory.Name != data.Name)
                {
                    subCategory.Name = data.Name;
                    hasChanges = true;
                }

                if (subCategory.CategoryId != dbCategory.Id)
                {
                    subCategory.CategoryId = dbCategory.Id;
                    hasChanges = true;
                }

                if (subCategory.IsActive != data.IsActive)
                {
                    subCategory.IsActive = data.IsActive;

                    if (subCategory.IsActive)
                    {
                        subCategory.DateDeleted = null;
                    }

                    hasChanges = true;
                }
                else if (data.IsActive == true && subCategory.IsActive == false)
                {
                    subCategory.IsActive = data.IsActive;
                    _context.Entry(subCategory).Property(x => x.IsActive).IsModified = true;
                    hasChanges = true;
                }

                if (hasChanges)
                {
                    subCategory.DateUpdated = DateTime.Now;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}

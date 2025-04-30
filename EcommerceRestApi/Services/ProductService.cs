using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services.Base;
using Inventory_Management_Sustem.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class ProductsService : EntityBaseRepository<Product>, IProductsService
    {

        private readonly AppDbContext _context;
        public ProductsService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddNewProductAsync(NewProductViewModel data)
        {
            var newProduct = new Product()
            {
                Code = data.Code,
                Name = data.Name,
                Brand = data.Brand,
                Price = data.Price,
                OldPrice = data.Price,
                About = data.About,
                LongAbout = data.LongAbout,
                Photo = data.Photo,
                OtherPhotos = data.OtherPhotos,
                Stock = data.Stock,
                IsActive = data.IsActive,
                DateCreated = DateTime.Now,
            };

            var subCategory = _context.Subcategories.FirstOrDefault(C => C.Code == data.SubcategoryCode);
            if (subCategory != null)
            {
                newProduct.SubcategoryId = subCategory.Id;
            }

            var category = _context.Categories.FirstOrDefault(C => C.Code == data.CategoryCode);
            if (category != null)
            {
                newProduct.ProductCategories.Add(new ProductCategory()
                {
                    CategoryId = category.Id,
                    ProductId = newProduct.Id,
                    IsActive = true,
                    DateCreated = DateTime.Now
                });

            }

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

        }


        public async Task<ProductDto?> GetProductByIDAsync(int id)
        {
            var product = await _context.Products
                                 .Include(p => p.Reviews)
                                 .ThenInclude(r => r.Customer)
                                 .ThenInclude(c => c.User)
                                 .Include(p => p.ProductCategories)
                                 .ThenInclude(pc => pc.Category)
                                 .Include(p => p.Subcategory)
                                 .Where(p => p.Id == id)
                                 .FirstOrDefaultAsync();

            if (product == null) return null;

            var p = ProductDto.ToDto(product);

            return p;
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            return await _context.Products
                                 .Include(p => p.Reviews)
                                 .ThenInclude(r => r.Customer)
                                 .ThenInclude(c => c.User)
                                 .Include(p => p.ProductCategories)
                                 .ThenInclude(pc => pc.Category)
                                 .Include(p => p.Subcategory)
                                 .Select(p => ProductDto.ToDto(p, true))
                                 .ToListAsync();
        }

        public async Task UpdateProductAsync(int id, ProductUpdateVM data)
        {
            var dbproduct = await _context.Products
                .Include(p => p.ProductCategories) // important!
                .FirstOrDefaultAsync(n => n.Id == id);

            if (dbproduct != null)
            {
                // Update product fields
                dbproduct.Name = data.Name ?? dbproduct.Name;
                dbproduct.Code = data.Code ?? dbproduct.Code;
                dbproduct.Brand = data.Brand ?? dbproduct.Brand;
                dbproduct.Price = data.Price ?? dbproduct.Price;
                dbproduct.About = data.About ?? dbproduct.About;
                dbproduct.LongAbout = data.LongAbout ?? dbproduct.LongAbout;
                dbproduct.Photo = data.Photo ?? dbproduct.Photo;
                dbproduct.OtherPhotos = data.OtherPhotos ?? dbproduct.OtherPhotos;
                dbproduct.Stock = data.Stock ?? dbproduct.Stock;
                dbproduct.IsActive = data.IsActive;
                dbproduct.DateUpdated = DateTime.Now;

                // Update Subcategory
                var subCategory = await _context.Subcategories
                    .FirstOrDefaultAsync(C => C.Code == data.SubcategoryCode);
                if (subCategory != null)
                {
                    dbproduct.SubcategoryId = subCategory.Id;
                }

                // Update Category (remove all existing and add new one)
                var category = await _context.Categories
                    .FirstOrDefaultAsync(C => C.Code == data.CategoryCode);

                if (category != null)
                {
                    var existingLinks = await _context.ProductCategories
                        .Where(pc => pc.ProductId == dbproduct.Id)
                        .ToListAsync();

                    _context.ProductCategories.RemoveRange(existingLinks);

                    dbproduct.ProductCategories.Add(new ProductCategory
                    {
                        ProductId = dbproduct.Id,
                        CategoryId = category.Id,
                        IsActive = true,
                        DateCreated = DateTime.Now
                    });
                }

                await _context.SaveChangesAsync();
            }
        }

    }
}

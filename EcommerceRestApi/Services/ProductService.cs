using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;
using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels;

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
                Name = data.Name,
                Brand = data.Brand,
                Price = data.Price,
                About = data.About,
                LongAbout = data.LongAbout,
                Photo = data.Photo,
                OtherPhotos = data.OtherPhotos,
                Stock = data.Stock,
                SubcategoryId = data.SubcategoryId,
            };

            var category = _context.Categories.FirstOrDefault(C => C.Id == data.CategoryId);
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

        //public async Task<NewProductsDropDownsVm> GetNewProductsDropDownValues()
        //{
        //    var response = new NewProductsDropDownsVm()
        //    {
        //        Suppliers = await _context.Suppliers.OrderBy(n => n.FullName).ToListAsync(),
        //        Companies = await _context.Companies.OrderBy(n => n.Name).ToListAsync(),
        //        Brands = await _context.Brands.OrderBy(n => n.FullName).ToListAsync()

        //    };
        //    return response;
        //}

        public async Task<Product> GetProductByIDAsync(int id)
        {
            var productDetails = await _context.Products
                .Include(C => C.Reviews)
            .Include(B => B.Subcategory)
            .ThenInclude(SC => SC.Category)
            .FirstOrDefaultAsync(n => n.Id == id);

            return productDetails;
        }

        public async Task UpdateProductAsync(int id, ProductUpdateVM data)
        {

            var dbproduct = await _context.Products.FirstOrDefaultAsync(n => n.Id == id);
            if (dbproduct != null)
            {

                dbproduct.Name = data.Name ?? dbproduct.Name;
                dbproduct.Brand = data.Brand ?? dbproduct.Brand;
                dbproduct.Price = data.Price ?? dbproduct.Price;
                dbproduct.About = data.About ?? dbproduct.About;
                dbproduct.LongAbout = data.LongAbout ?? dbproduct.LongAbout;
                dbproduct.Photo = data.Photo ?? dbproduct.Photo;
                dbproduct.OtherPhotos = data.OtherPhotos ?? dbproduct.OtherPhotos;
                dbproduct.Stock = data.Stock ?? dbproduct.Stock;
                dbproduct.SubcategoryId = data.SubcategoryId ?? dbproduct.SubcategoryId;
                dbproduct.IsActive = data.IsActive;
                dbproduct.DateUpdated = DateTime.Now;


                dbproduct.ProductCategories.First().CategoryId = data.CategoryId ?? dbproduct.ProductCategories.First().CategoryId;
                dbproduct.ProductCategories.First().DateUpdated = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }
    }
}

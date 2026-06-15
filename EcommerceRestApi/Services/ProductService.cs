using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
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

        public async Task<List<ProductDto>> FilterProductsAsync(
          string searchString,
          string? searchProperty,
          string? sortProperty,
          decimal? fromPrice,
          decimal? ToPrice,
          string? categoryCode,
          string? subcategoryCode,
          int? minRating,
          bool sortAscending = false)
        {

            var filteredResult = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {

                switch (searchProperty)
                {
                    case nameof(Product.Name):
                        filteredResult = filteredResult.Where(item => item.Name.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "CategoryName":
                        filteredResult = filteredResult.Where(item => item.ProductCategories.First().Category.Name.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "SubcategoryName":
                        filteredResult = filteredResult.Where(item => item.Subcategory.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                        break;
                }

                if (searchProperty == null)
                {
                    filteredResult = filteredResult.Where(n =>
                                n.Name.ToLower().Contains(searchString.ToLower()) ||
                                n.LongAbout.ToLower().Contains(searchString.ToLower()) ||
                                n.About.ToLower().Contains(searchString.ToLower())
                        )
                        .AsQueryable();
                }
            }

            if (fromPrice.HasValue)
            {
                filteredResult = filteredResult.Where(item => item.Price >= fromPrice.Value);
            }

            if (ToPrice.HasValue)
            {
                filteredResult = filteredResult.Where(item => item.Price <= ToPrice.Value);
            }

            if (!string.IsNullOrEmpty(categoryCode))
                filteredResult = filteredResult.Where(p => p.ProductCategories.Any(pc => pc.Category.Code == categoryCode && pc.IsActive));

            if (!string.IsNullOrEmpty(subcategoryCode))
                filteredResult = filteredResult.Where(p => p.ProductCategories.Any(pc => pc.Category.Subcategories.Any(sc => sc.Code == subcategoryCode && sc.IsActive)));

            if (minRating.HasValue)
                filteredResult = filteredResult.Where(item => item.RatingSum >= minRating);


            if (!string.IsNullOrEmpty(sortProperty))
            {
                switch (sortProperty)
                {
                    case "CategoryName":
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.ProductCategories.First().Category.Name) : filteredResult.OrderByDescending(item => item.ProductCategories.First().Category.Name);
                        break;
                    case nameof(Product.Name):
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.Name) : filteredResult.OrderByDescending(item => item.Name);
                        break;
                    case "SubcategoryName":
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.ProductCategories.First().Category.Name) : filteredResult.OrderByDescending(item => item.ProductCategories.First().Category.Name);
                        break;
                    case nameof(Product.RatingSum):
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.RatingSum) : filteredResult.OrderByDescending(item => item.RatingSum);
                        break;
                }
            }
            else
            {
                filteredResult = filteredResult.OrderByDescending(p => p.RatingSum);
            }

            return filteredResult.Select(p => new ProductDto
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
                IsOutOfStock = p.Stock < 10 ? true : false,
                Reviews = p.Reviews.Where(r => r.IsActive).Select(r => new ReviewDto
                {
                    Id = r.Id,
                    IsActive = r.IsActive,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    ProductId = r.ProductId,
                    CustomerId = r.CustomerId,
                    UserName = r.Customer != null && r.Customer.User != null ?
                              r.Customer.User.FullName : string.Empty,
                    Customer = r.Customer != null ? new CustomerDto
                    {
                        Id = r.Customer.Id,
                        CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                        FullName = r.Customer.User != null ? r.Customer.User.FullName : string.Empty,
                        Email = r.Customer.User != null ? r.Customer.User.Email : string.Empty,
                        Nip = r.Customer.Nip,
                        Address = new AddressDto
                        {
                            HouseNumber = r.Customer.Addresses.FirstOrDefault().HouseNumber,
                            Street = r.Customer.Addresses.FirstOrDefault().Street,
                            City = r.Customer.Addresses.FirstOrDefault().City,
                            PostalCode = r.Customer.Addresses.FirstOrDefault().PostalCode,
                            FlatNumber = r.Customer.Addresses.FirstOrDefault().FlatNumber,
                            CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                            State = r.Customer.Addresses.FirstOrDefault().State,
                            CustomerId = r.Customer.Id
                        },
                        Points = r.Customer.Points,
                        IsActive = r.Customer.IsActive,
                        DateCreated = r.Customer.DateCreated
                    } : null!,
                    DateCreated = r.DateCreated,
                    DateUpdated = r.DateUpdated ?? DateTime.MinValue
                }).ToList()
            }).ToList();
        }

        public async Task AddNewProductAsync(NewProductViewModel data)
        {
            var newProduct = new Product()
            {
                Code = data.Code,
                Name = data.Name,
                Brand = data.Brand,
                Price = data.Price + (data.Price * AppConstants.TAXES_RATE),
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
                                 .Where(p => p.Id == id)
                                 .Select(p => new ProductDto
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
                                     IsOutOfStock = p.Stock < 10 ? true : false,
                                     Reviews = p.Reviews.Where(r => r.IsActive).Select(r => new ReviewDto
                                     {
                                         Id = r.Id,
                                         IsActive = r.IsActive,
                                         Rating = r.Rating,
                                         ReviewText = r.ReviewText,
                                         ProductId = r.ProductId,
                                         CustomerId = r.CustomerId,
                                         UserName = r.Customer != null && r.Customer.User != null ?
                                                   r.Customer.User.FullName : string.Empty,
                                         Customer = r.Customer != null ? new CustomerDto
                                         {
                                             Id = r.Customer.Id,
                                             CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                                             FullName = r.Customer.User != null ? r.Customer.User.FullName : string.Empty,
                                             Email = r.Customer.User != null ? r.Customer.User.Email : string.Empty,
                                             Nip = r.Customer.Nip,
                                             Address = new AddressDto
                                             {
                                                 HouseNumber = r.Customer.Addresses.FirstOrDefault().HouseNumber,
                                                 Street = r.Customer.Addresses.FirstOrDefault().Street,
                                                 City = r.Customer.Addresses.FirstOrDefault().City,
                                                 PostalCode = r.Customer.Addresses.FirstOrDefault().PostalCode,
                                                 FlatNumber = r.Customer.Addresses.FirstOrDefault().FlatNumber,
                                                 CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                                                 State = r.Customer.Addresses.FirstOrDefault().State,
                                                 CustomerId = r.Customer.Id
                                             },
                                             Points = r.Customer.Points,
                                             IsActive = r.Customer.IsActive,
                                             DateCreated = r.Customer.DateCreated
                                         } : null!,
                                         DateCreated = r.DateCreated,
                                         DateUpdated = r.DateUpdated ?? DateTime.MinValue
                                     }).ToList()
                                 })
                                 .FirstOrDefaultAsync();
            return product;
        }

        public async Task<List<ProductDto>> GetProducts()
        {
            return await _context.Products
                .Select(p => new ProductDto
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
                    IsOutOfStock = p.Stock < 10 ? true : false,
                    Reviews = p.Reviews.Where(r => r.IsActive).Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        IsActive = r.IsActive,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        ProductId = r.ProductId,
                        CustomerId = r.CustomerId,
                        UserName = r.Customer != null && r.Customer.User != null ?
              r.Customer.User.FullName : string.Empty,
                        Customer = r.Customer != null ? new CustomerDto
                        {
                            Id = r.Customer.Id,
                            CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                            FullName = r.Customer.User != null ? r.Customer.User.FullName : string.Empty,
                            Email = r.Customer.User != null ? r.Customer.User.Email : string.Empty,
                            Nip = r.Customer.Nip,
                            Address = new AddressDto
                            {
                                HouseNumber = r.Customer.Addresses.FirstOrDefault().HouseNumber,
                                Street = r.Customer.Addresses.FirstOrDefault().Street,
                                City = r.Customer.Addresses.FirstOrDefault().City,
                                PostalCode = r.Customer.Addresses.FirstOrDefault().PostalCode,
                                FlatNumber = r.Customer.Addresses.FirstOrDefault().FlatNumber,
                                CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                                State = r.Customer.Addresses.FirstOrDefault().State,
                                CustomerId = r.Customer.Id
                            },
                            Points = r.Customer.Points,
                            IsActive = r.Customer.IsActive,
                            DateCreated = r.Customer.DateCreated
                        } : null!,
                        DateCreated = r.DateCreated,
                        DateUpdated = r.DateUpdated ?? DateTime.MinValue
                    }).ToList()

                })
                .OrderByDescending(p => p.RatingSum)
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
                dbproduct.Price = (decimal)(data.Price == null
                                    ? dbproduct.Price
                                    : data.Price + (data.Price * AppConstants.TAXES_RATE));
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

        public List<SearchComboBoxDto> GetSearchComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Product.Name),
                    DisplayName = "Product Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "CategoryName",
                    DisplayName = "Category Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "SubcategoryName",
                    DisplayName = "Subcategory Name"
                }
            };
        }

        public List<SearchComboBoxDto> GetOrderByComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Product.Name),
                    DisplayName = "Product Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "SubcategoryName",
                    DisplayName = "Subcategory Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "CategoryName",
                    DisplayName = "Category Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Product.RatingSum),
                    DisplayName = "Rating sum"
                }
            };
        }

        public async Task<int> IncreaseStockAsync(int productId, int quantityToAdd)
        {
            if (quantityToAdd <= 0)
            {
                throw new ArgumentException("Quantity to add must be greater than 0");
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            product.Stock += quantityToAdd;
            product.DateUpdated = DateTime.Now;

            await _context.SaveChangesAsync();

            return product.Stock;
        }
    }
}

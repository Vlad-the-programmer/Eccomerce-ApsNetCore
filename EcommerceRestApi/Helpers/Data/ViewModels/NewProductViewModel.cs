using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using Inventory_Management_Sustem.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Product name")]
        [Required(ErrorMessage = "Product name is Requred")]
        public string Name { get; set; } = null!;

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Code is Requred")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Brand is Requred")]
        public string Brand { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is Requred")]
        public decimal Price { get; set; }

        [Display(Name = "About")]
        public string? About { get; set; } = default!;

        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is Requred")]
        public string LongAbout { get; set; }

        [Display(Name = "Photo")]
        [Required(ErrorMessage = "Photo is Requred")]
        public string? Photo { get; set; } = default!;

        [Display(Name = "Other photoes")]
        public string? OtherPhotos { get; set; } = default!;

        [Display(Name = "Stock")]
        [Required(ErrorMessage = "Stock is Requred")]
        public int Stock { get; set; }

        [Display(Name = "Is Active")]

        public bool IsActive { get; set; }


        [Display(Name = "SubCategory Code")]
        [Required(ErrorMessage = "Subcategory Code is Requred")]
        public string SubcategoryCode { get; set; }

        [Display(Name = "Category Code")]
        [Required(ErrorMessage = "Category code is Requred")]
        public string CategoryCode { get; set; }

        public IList<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();

        public static NewProductViewModel FromProductToVm(Product p)
        {
            return new NewProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Photo = p.Photo,
                OtherPhotos = p.OtherPhotos,
                CategoryCode = p.ProductCategories.FirstOrDefault()?.Category?.Code,
                SubcategoryCode = p.Subcategory?.Code,
                About = p.About,
                Brand = p.Brand,
                LongAbout = p.LongAbout,
                Price = p.Price,
                Stock = p.Stock,
                Reviews = p.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    IsActive = r.IsActive,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    ProductId = r.ProductId,
                    CustomerId = r.CustomerId,
                    UserName = r.Customer != null && r.Customer.User != null ? r.Customer.User.UserName : null,
                    Customer = r.Customer != null ? new CustomerDto
                    {
                        Id = r.Customer.Id,
                        CountryName = r.Customer.Addresses.FirstOrDefault()?.Country?.CountryName,
                        FullName = r.Customer.User?.FullName ?? string.Empty,
                        Email = r.Customer.User?.Email,
                        Nip = r.Customer.Nip,
                        Points = r.Customer.Points,
                        IsActive = r.Customer.IsActive,
                        DateCreated = r.Customer.DateCreated,
                        Address = r.Customer.Addresses.FirstOrDefault() != null ? new AddressDto
                        {
                            Id = r.Customer.Addresses.FirstOrDefault().Id,
                            CountryName = r.Customer.Addresses.FirstOrDefault().Country.CountryName,
                            CustomerId = r.Customer.Addresses.FirstOrDefault().CustomerId,
                            Street = r.Customer.Addresses.FirstOrDefault().Street,
                            HouseNumber = r.Customer.Addresses.FirstOrDefault().HouseNumber,
                            FlatNumber = r.Customer.Addresses.FirstOrDefault().FlatNumber,
                            City = r.Customer.Addresses.FirstOrDefault().City,
                            State = r.Customer.Addresses.FirstOrDefault().State,
                            PostalCode = r.Customer.Addresses.FirstOrDefault().PostalCode
                        } : null
                    } : null!,
                    Product = null!,
                    DateCreated = r.DateCreated,
                    DateUpdated = r.DateUpdated ?? DateTime.MinValue,
                }).ToList(),
            };
        }
    }
}

using EcommerceRestApi.Models;
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

        public IList<Review> Reviews { get; set; } = new List<Review>();

        public static NewProductViewModel FromProductToVm(Product p)
        {
            return new NewProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Photo = p.Photo,
                OtherPhotos = p.OtherPhotos,
                CategoryCode = p.ProductCategories.FirstOrDefault()?.Category?.Code,
                SubcategoryCode = p.Subcategory.Code,
                About = p.About,
                Brand = p.Brand,
                LongAbout = p.LongAbout,
                Price = p.Price,
                Stock = p.Stock,
            };
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class NewProductViewModel: BaseViewModel
    {
        public NewProductViewModel(): base("Create Product") { }
        public int Id { get; set;  }

        [Display(Name = "Product name")]
        [Required(ErrorMessage = "Product name is Requred")]
        public string Name { get; set; }

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Brand is Requred")]
        public string Brand { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is Requred")]
        public int Price { get; set; }

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

        [Display(Name = "Select Product SubCategory")]
        [Required(ErrorMessage = "Subcategory is Requred")]
        public int SubcategoryId { get; set; }

        [Display(Name = "Select Product Category")]
        [Required(ErrorMessage = "Category is Requred")]
        public int CategoryId { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}

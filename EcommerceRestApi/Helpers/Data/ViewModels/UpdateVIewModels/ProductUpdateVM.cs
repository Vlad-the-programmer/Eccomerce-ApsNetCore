using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels
{
    public class ProductUpdateVM: BaseUpdateViewModel
    {
        [Display(Name = "Product name")]
        public string? Name { get; set; }

        [Display(Name = "Brand")]
        public string? Brand { get; set; }

        [Display(Name = "Price")]
        public int? Price { get; set; }

        [Display(Name = "About")]
        public string? About { get; set; } = default!;

        [Display(Name = "Description")]
        public string? LongAbout { get; set; }

        [Display(Name = "Photo")]
        public string? Photo { get; set; } = default!;

        [Display(Name = "Other photoes")]
        public string? OtherPhotos { get; set; } = default!;

        [Display(Name = "Stock")]
        public int? Stock { get; set; }

        //[Display(Name = "Select Product SubCategory")]
        //public int? SubcategoryId { get; set; }

        //[Display(Name = "Select Product Category")]
        //public int? CategoryId { get; set; }

        [Display(Name = "SubCategory Code")]
        [Required(ErrorMessage = "Subcategory Code is Requred")]
        public string SubcategoryCode { get; set; }

        [Display(Name = "Category Code")]
        [Required(ErrorMessage = "Category code is Requred")]
        public string CategoryCode { get; set; }
    }
}

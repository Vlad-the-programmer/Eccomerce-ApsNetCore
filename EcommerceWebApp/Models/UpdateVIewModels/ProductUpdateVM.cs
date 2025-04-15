using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.UpdateViewModels
{
    public class ProductUpdateVM : BaseUpdateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Product name")]
        public string? Name { get; set; }

        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "Brand")]
        public string? Brand { get; set; }

        [Display(Name = "Price")]
        public decimal? Price { get; set; }

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

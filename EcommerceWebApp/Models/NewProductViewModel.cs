using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EcommerceWebApp.Models
{
    public class NewProductViewModel : BaseViewModel
    {
        public NewProductViewModel() : base("Create Product") { }
        public int Id { get; set; }

        [Display(Name = "Product name")]
        [Required(ErrorMessage = "Product name is Requred")]
        public string Name { get; set; }

        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is Requred")]
        public string Code { get; set; }

        [Display(Name = "Brand")]
        [Required(ErrorMessage = "Brand is Requred")]
        public string Brand { get; set; }

        [Display(Name = "Price")]
        [Required(ErrorMessage = "Price is Requred")]
        public decimal Price { get; set; }

        [Display(Name = "About")]
        public string? About { get; set; } = default!;

        [Display(Name = "Long About")]
        [Required(ErrorMessage = "Description is Requred")]
        public string LongAbout { get; set; }

        [Display(Name = "Photo")]
        [Required(ErrorMessage = "Photo is Requred")]
        public string Photo { get; set; }

        [Display(Name = "Other photoes")]
        public string? OtherPhotos { get; set; } = default!;

        [Display(Name = "Stock")]
        [Required(ErrorMessage = "Stock is Requred")]
        public int Stock { get; set; }

        //[Display(Name = "Select Product SubCategory")]
        //[Required(ErrorMessage = "Subcategory is Requred")]
        //public int SubcategoryId { get; set; }

        //[Display(Name = "Select Product Category")]
        //[Required(ErrorMessage = "Category is Requred")]
        //public int CategoryId { get; set; }

        [Display(Name = "SubCategory Code")]
        [Required(ErrorMessage = "Subcategory Code is Requred")]
        public string SubcategoryCode { get; set; }

        [Display(Name = "Category Code")]
        [Required(ErrorMessage = "Category code is Requred")]
        public string CategoryCode { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public IList<ReviewViewModel> Reviews { get; set; } = new List<ReviewViewModel>();

        [JsonIgnore]
        public ReviewViewModel ReviewForm { get; set; } = new ReviewViewModel();
    }
}

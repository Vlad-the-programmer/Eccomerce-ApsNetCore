using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class NewSubcategoryVM : BaseViewModel
    {
        public NewSubcategoryVM() : base("Create SubCategory") { }

        [Display(Name = "SubCategory name")]
        [Required(ErrorMessage = "Subcategory name is Requred")]
        public string Name { get; set; }

        [Display(Name = "SubCategory about")]
        public string About { get; set; } = default!;

        [Display(Name = "Category Code")]
        [Required(ErrorMessage = "Category Code is Requred")]
        public string CategoryCode { get; set; }
    }
}

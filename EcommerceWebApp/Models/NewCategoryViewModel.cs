using System.ComponentModel.DataAnnotations;


namespace EcommerceWebApp.Models
{
    public class NewCategoryViewModel : BaseViewModel
    {
        public NewCategoryViewModel() : base("Create Category") { }

        [Display(Name = "Category Code")]
        public string? Code { get; set; } = default!;

        [Display(Name = "SubCategory name")]
        [Required(ErrorMessage = "Subcategory name is Requred")]
        public string Name { get; set; }

        [Display(Name = "SubCategory about")]
        public string About { get; set; }

        [Display(Name = "SubCategory Code")]
        public string? SubcategoryCode { get; set; } = default!;
    }
}

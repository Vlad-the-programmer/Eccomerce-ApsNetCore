using EcommerceWebApp.Models.UpdateViewModels;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.UpdateVIewModels
{
    public class SubcategoryUpdateVM : BaseUpdateViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "SubCategory Code")]
        [StringLength(20, ErrorMessage = "Code must be between up to 20 characters.")]
        public string Code { get; set; } = default!;

        [Display(Name = "SubCategory name")]
        [Required(ErrorMessage = "Subcategory name is Requred")]
        public string Name { get; set; }

        [Display(Name = "SubCategory about")]
        public string About { get; set; } = default!;

        [Display(Name = "Category Code")]
        [Required(ErrorMessage = "Category Code is Requred")]
        public string CategoryCode { get; set; }
        public bool IsActive { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.UpdateViewModels
{
    public class CategoryUpdateVM : BaseUpdateViewModel
    {

        [StringLength(20, ErrorMessage = "Code must be between up to 20 characters.")]
        public string? Code { get; set; }

        [StringLength(500, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20 characters.")]
        public string? Name { get; set; }

        public string? About { get; set; } = default!;

        //public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        //public int SubcategoryId { get; set; }
        //public int CategoryId { get; set; }
    }
}

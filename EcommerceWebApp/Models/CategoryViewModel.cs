using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class CategoryViewModel: BaseViewModel
    {
        public CategoryViewModel() : base("Category") { }

        public int Id { get; set; }
        public string Code { get; set; }
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 20 characters.")]
        [Required]
        public string Name { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 20 characters.")]
        public string About { get; set; } = default!;

        //public List<SubcategoryViewModel> Subcategories { get; set; } = new List<SubcategoryViewModel>();

        //public List<ProductCategoryViewModel> ProductCategories { get; set; } = new List<ProductCategoryViewModel>();
        public int SubcategoryId { get; set; }
        public int CategoryId { get; set; }

    }
}

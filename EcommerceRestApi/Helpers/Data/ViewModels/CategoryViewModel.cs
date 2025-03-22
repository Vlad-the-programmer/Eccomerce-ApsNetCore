using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceRestApi.Models;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class CategoryViewModel: BaseViewModel
    {
        public int Id { get; set; }

        [StringLength(20, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 20 characters.")]
        public string? Code { get; set; } = default!;

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 20 characters.")]
        [Required]
        public string Name { get; set; }

        public string? About { get; set; } = default!;

        //public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();

        //public List<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
        public int SubcategoryId { get; set; }
        //public int CategoryId { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceRestApi.Helpers.Data.ViewModels;

namespace EcommerceWebApp.Models
{
    public class SubcategoryViewModel: BaseViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Code must be between 3 and 20 characters.")]
        public string? Code { get; set; } = default!;
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Subcategory name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string About { get; set; } = default!;

        public int CategoryId { get; set; }

        public CategoryViewModel Category { get; set; } = null!;

        public ICollection<NewProductViewModel> Products { get; set; } = new List<NewProductViewModel>();
    }
}

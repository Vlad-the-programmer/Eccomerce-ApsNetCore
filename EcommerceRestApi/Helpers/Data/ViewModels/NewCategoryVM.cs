using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class NewCategoryVM : BaseViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string About { get; set; } = default!;
        public string? SubcategoryCode { get; set; } = default!;
    }
}

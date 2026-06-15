using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class NewSubcategoryVM : BaseViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string About { get; set; } = default!;
        public string CategoryCode { get; set; }
    }
}

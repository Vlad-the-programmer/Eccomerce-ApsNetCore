using EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels
{
    public class SubcategoryUpdateVM : BaseUpdateViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? About { get; set; } = default!;
        public string? CategoryCode { get; set; }
        public bool IsActive { get; set; }

    }
}

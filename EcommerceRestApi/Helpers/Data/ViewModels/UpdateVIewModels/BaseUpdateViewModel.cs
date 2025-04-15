using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels
{
    public class BaseUpdateViewModel
    {
        public int Id { get; set; }
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
    }
}

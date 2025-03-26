using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.UpdateViewModels
{
    public class BaseUpdateViewModel
    {
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
    }
}

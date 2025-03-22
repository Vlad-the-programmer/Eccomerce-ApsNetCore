using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels.UpdateVIewModels
{
    public class BaseUpdateViewModel
    {
        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class RegisterViewModel : ApplicationUserViewModel
    {
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not Match")]
        public string ConfirmPassword { get; set; }

    }
}

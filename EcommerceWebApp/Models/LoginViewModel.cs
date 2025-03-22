using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class LoginViewModel: BaseViewModel
    {
        public LoginViewModel(): base("Login")
        {

        }
        [Display(Name = "EmailAddress")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

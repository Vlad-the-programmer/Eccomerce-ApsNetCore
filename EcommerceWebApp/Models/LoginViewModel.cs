using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class LoginViewModel
    {
            [Display(Name = "EmailAddress")]
            [Required(ErrorMessage = "Email is Required")]
            public string EmailAddress { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
    }
}

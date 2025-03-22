using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        [Display(Name = "EmailAddress")]
            [Required(ErrorMessage = "Email is Required")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            public bool RememberMe { get; set; }
    }
}

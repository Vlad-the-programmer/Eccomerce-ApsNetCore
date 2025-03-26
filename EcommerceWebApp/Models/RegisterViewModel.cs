using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class RegisterViewModel: ApplicationUserViewModel
    {
            public List<string> Countries { get; set; }

            public RegisterViewModel()
            {
                Countries = new List<string>();
            }
        public RegisterViewModel(List<string> countries) : base("Register")
            {
                Countries = countries;
            }


            [Display(Name = "Confirm Password")]
            [Required(ErrorMessage = "Confirm Password is Required")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Password do not Match")]
            public string ConfirmPassword { get; set; }

        }
}

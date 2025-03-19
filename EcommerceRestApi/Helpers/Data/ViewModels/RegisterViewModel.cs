using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is Required")]
        public string FirstName { get; set; }

        [Display(Name = "Lastname")]
        [Required(ErrorMessage = "Lastname is Required")]
        public string LastName { get; set; }

        //[Display(Name = "Username")]
        //[Required(ErrorMessage = "Username is Required")]
        //public string Username { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [Display(Name = "Phonenumber")]
        [Required(ErrorMessage = "Phonenumber is Required")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password do not Match")]
        public string ConfirmPassword { get; set; }
    }
}

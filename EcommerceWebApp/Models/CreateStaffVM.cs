using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class CreateStaffVM
    {
        [Display(Name = "Name")]
        public string FirstName { get; set; }

        [Display(Name = "Lastname")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string? UserName { get; set; }

        [Display(Name = "Phonenumber")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

    }
}

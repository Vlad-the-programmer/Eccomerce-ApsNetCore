using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class ApplicationUserViewModel : BaseViewModel
    {
        public ApplicationUserViewModel() { }
        public ApplicationUserViewModel(string title) : base(title) { }

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
        public string? Nip { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? CountryName { get; set; }
        public string? Street { get; set; }
        public string? HouseNumber { get; set; }
        public string? FlatNumber { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
    }
}

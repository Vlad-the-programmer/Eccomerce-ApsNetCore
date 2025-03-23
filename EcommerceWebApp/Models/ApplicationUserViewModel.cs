using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models
{
    public class ApplicationUserViewModel:BaseViewModel
    {
        public ApplicationUserViewModel() { }
        public ApplicationUserViewModel(string title): base(title) { }

        [Display(Name = "Name")]
        //[Required(ErrorMessage = "Name is Required")]
        public string FirstName { get; set; }

        [Display(Name = "Lastname")]
        //[Required(ErrorMessage = "Lastname is Required")]
        public string LastName { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Display(Name = "Phonenumber")]
        //[Required(ErrorMessage = "Phonenumber is Required")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Nip")]
        //[DataType(DataType.Password)]
        //[StringLength(10, ErrorMessage = "Nip should be 10 digit long")]
        public string Nip { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string Role { get; set; }

        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }


        public int? CountryId { get; set; }

        public string? CountryName { get; set; }


        //[StringLength(100, ErrorMessage = "Street should be no more than 100 characters long.")]
        public string? Street { get; set; }

        //[StringLength(20, ErrorMessage = "Housenumber should be no more than 20 characters long.")]
        public string? HouseNumber { get; set; }

        //[StringLength(20, ErrorMessage = "Flatnumber should be no more than 20 characters long.")]
        public string? FlatNumber { get; set; }

        //[StringLength(50, ErrorMessage = "City should be no more than 50 characters long.")]
        public string? City { get; set; }

        //[StringLength(50, ErrorMessage = "State should be no more than 50 characters long.")]
        public string? State { get; set; }

        //[StringLength(10, ErrorMessage = "Postalcode should be no more than 10 characters long.")]
        public string? PostalCode { get; set; }
    }
}

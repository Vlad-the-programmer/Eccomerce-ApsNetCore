
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.UpdateViewModels
{
    public class UserUpdateVM : BaseUpdateViewModel
    {
        public UserUpdateVM()
        {
        }

        /////// <summary>
        /////// The ID of the selected country.
        /////// </summary>
        //[Display(Name = "Id")]
        //[Required(ErrorMessage = "Id is Required")]
        //public string Id { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        [Display(Name = "Name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        [Display(Name = "Lastname")]
        public string? LastName { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        /// <summary>
        /// The username of the user (optional).
        /// </summary>
        [Display(Name = "Username")]
        public string? Username { get; set; }

        /// <summary>
        /// The phone number of the user.
        /// </summary>
        [Display(Name = "Phonenumber")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        ///// <summary>
        ///// The password for the user account.
        ///// </summary>
        //[Required]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        ///// <summary>
        ///// The confirmation password for the user account.
        ///// </summary>
        //[Display(Name = "Confirm Password")]
        //[Required(ErrorMessage = "Confirm Password is Required")]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "Password do not Match")]
        //public string ConfirmPassword { get; set; }

        /// <summary>
        /// The NIP (Tax Identification Number) of the user (optional).
        /// </summary>
        [Display(Name = "Nip")]
        [StringLength(10, ErrorMessage = "Nip should be 10 digit long")]
        public string? Nip { get; set; } = string.Empty;

        /// <summary>
        /// The role of the user (e.g., Admin, User).
        /// </summary>
        [Display(Name = "Role")]
        public string? Role { get; set; } = null!;

        [Display(Name = "IsAdmin")]
        public bool IsAdmin { get; set; }


        ///// <summary>
        ///// The ID of the selected country.
        ///// </summary>
        //public int? CountryId { get; set; }

        /// <summary>
        /// The Points User get for making an each  order.
        /// </summary>
        public int? Points { get; set; }

        /// <summary>
        /// The name of the selected country.
        /// </summary>
        [StringLength(100, ErrorMessage = "Country name should be no more than 100 characters long.")]
        public string? CountryName { get; set; }


        /// <summary>
        /// The street address of the user.
        /// </summary>
        [StringLength(100, ErrorMessage = "Street should be no more than 100 characters long.")]
        public string? Street { get; set; }

        /// <summary>
        /// The house number of the user.
        /// </summary>
        [StringLength(20, ErrorMessage = "Housenumber should be no more than 20 characters long.")]
        public string? HouseNumber { get; set; }

        /// <summary>
        /// The flat number of the user (optional).
        /// </summary>
        [StringLength(20, ErrorMessage = "Flatnumber should be no more than 20 characters long.")]
        public string? FlatNumber { get; set; }

        /// <summary>
        /// The city of the user.
        /// </summary>
        [StringLength(50, ErrorMessage = "City should be no more than 50 characters long.")]
        public string? City { get; set; }

        /// <summary>
        /// The state or region of the user.
        /// </summary>
        [StringLength(50, ErrorMessage = "State should be no more than 50 characters long.")]
        public string? State { get; set; }

        /// <summary>
        /// The postal code of the user.
        /// </summary>
        [StringLength(10, ErrorMessage = "Postalcode should be no more than 10 characters long.")]
        [DataType(DataType.PostalCode)]
        public string? PostalCode { get; set; }
    }
}

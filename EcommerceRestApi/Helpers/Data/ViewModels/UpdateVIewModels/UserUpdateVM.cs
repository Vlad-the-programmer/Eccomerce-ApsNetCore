using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Helpers.Data.ViewModels.UpdateViewModels
{
    public class UserUpdateVM
    {
        ///// <summary>
        ///// The ID of the selected country.
        ///// </summary>
        [Display(Name = "Id")]
        [Required(ErrorMessage = "Id is Required")]
        public string Id { get; set; }

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
        /// The name of the selected country.
        /// </summary>
        [StringLength(100, ErrorMessage = "Country name should be no more than 100 characters long.")]
        [Unicode(false)]
        public string? CountryName { get; set; }


        /// <summary>
        /// The street address of the user.
        /// </summary>
        [StringLength(100, ErrorMessage = "Street should be no more than 100 characters long.")]
        [Unicode(false)]
        public string? Street { get; set; }

        /// <summary>
        /// The house number of the user.
        /// </summary>
        [StringLength(20, ErrorMessage = "Housenumber should be no more than 20 characters long.")]
        [Unicode(false)]
        public string? HouseNumber { get; set; }

        /// <summary>
        /// The flat number of the user (optional).
        /// </summary>
        [StringLength(20, ErrorMessage = "Flatnumber should be no more than 20 characters long.")]
        [Unicode(false)]
        public string? FlatNumber { get; set; }

        /// <summary>
        /// The city of the user.
        /// </summary>
        [StringLength(50, ErrorMessage = "City should be no more than 50 characters long.")]
        [Unicode(false)]
        public string? City { get; set; }

        /// <summary>
        /// The state or region of the user.
        /// </summary>
        [StringLength(50, ErrorMessage = "State should be no more than 50 characters long.")]
        [Unicode(false)]
        public string? State { get; set; }

        /// <summary>
        /// The postal code of the user.
        /// </summary>
        [StringLength(10, ErrorMessage = "Postalcode should be no more than 10 characters long.")]
        [DataType(DataType.PostalCode)]
        [Unicode(false)]
        public string? PostalCode { get; set; }
    }
}

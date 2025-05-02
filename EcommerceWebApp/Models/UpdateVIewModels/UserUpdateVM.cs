
using System.ComponentModel.DataAnnotations;

namespace EcommerceWebApp.Models.UpdateViewModels
{
    public class UserUpdateVM
    {
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
        public string? Nip { get; set; } = string.Empty;

        /// <summary>
        /// The name of the selected country.
        /// </summary>
        public string? CountryName { get; set; }


        /// <summary>
        /// The street address of the user.
        /// </summary>
        public string? Street { get; set; }

        /// <summary>
        /// The house number of the user.
        /// </summary>
        public string? HouseNumber { get; set; }

        /// <summary>
        /// The flat number of the user (optional).
        /// </summary>
        public string? FlatNumber { get; set; }

        /// <summary>
        /// The city of the user.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// The state or region of the user.
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// The postal code of the user.
        /// </summary>
        public string? PostalCode { get; set; }
    }
}

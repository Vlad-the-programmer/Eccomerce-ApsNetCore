using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models.Common;

[Index("Username", Name = "UQ__Users__536C85E4347D1877", IsUnique = true)]
[Index("PhoneNumber", Name = "UQ__Users__85FB4E38C7241820", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D10534494A092D", IsUnique = true)]
public partial class User : EntityBase
{

    [Required(ErrorMessage = "Username is required.")]
    [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(50, ErrorMessage = "Password cannot exceed 50 characters.")]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "First Name is required.")]
    [StringLength(50, ErrorMessage = "First Name cannot exceed 50 characters.")]
    [Unicode(false)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required.")]
    [StringLength(50, ErrorMessage = "Last Name cannot exceed 50 characters.")]
    [Unicode(false)]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required.")]
    [StringLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Role is required.")]
    [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters.")]
    [Unicode(false)]
    public string Role { get; set; }

    [Required(ErrorMessage = "Phone Number is required.")]
    [StringLength(50, ErrorMessage = "Phone Number cannot exceed 50 characters.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    [Unicode(false)]
    public string PhoneNumber { get; set; }

    public bool IsAdmin { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}

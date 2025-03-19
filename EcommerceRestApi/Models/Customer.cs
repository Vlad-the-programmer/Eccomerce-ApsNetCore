using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

public partial class Customer : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "User ID is required.")]
    public string UserId { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Points cannot be negative.")]
    public int? Points { get; set; }

    [StringLength(50, ErrorMessage = "NIP should be no more than 50 characters long.")]
    [Unicode(false)]
    public string? Nip { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    [InverseProperty("Customer")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Customer")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("UserId")]
    [InverseProperty("Customers")]
    public virtual ApplicationUser User { get; set; } = null!;
}

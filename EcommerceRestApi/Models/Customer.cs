using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("UserId", Name = "IX_Customers_UserId")]
public partial class Customer : EntityBase
{
    public string UserId { get; set; } = null!;

    public int? Points { get; set; }

    [StringLength(50)]
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

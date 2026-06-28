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

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Return> Returns { get; set; } = new List<Return>();
    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();

    [ForeignKey("UserId")]
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("SubcategoryId", Name = "IX_Products_SubcategoryId")]
public partial class Product : EntityBase
{
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string Brand { get; set; } = null!;

    [StringLength(30)]
    public string Code { get; set; } = null!;

    public int Price { get; set; }

    public int? OldPrice { get; set; }

    [StringLength(500)]
    public string? About { get; set; }

    [StringLength(2000)]
    public string LongAbout { get; set; } = null!;

    public int? RatingSum { get; set; }

    public int? RatingVotes { get; set; }

    [StringLength(255)]
    public string? Photo { get; set; }

    [StringLength(1000)]
    public string? OtherPhotos { get; set; }

    public int Stock { get; set; }

    public int SubcategoryId { get; set; }
    // Many-to-One: A Product belongs to one Subcategory
    [ForeignKey("SubcategoryId")]
    [InverseProperty("Products")]
    public virtual Subcategory Subcategory { get; set; } = null!;

    // Many-to-Many: A Product can belong to multiple Categories (via ProductCategory)
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    [InverseProperty("Product")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [InverseProperty("Product")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();


    [InverseProperty("Product")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Product")]
    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[PrimaryKey("ProductId", "CategoryId")]
[Index("CategoryId", Name = "IX_ProductCategories_CategoryId")]
public partial class ProductCategory
{
    [Key]
    public int ProductId { get; set; }

    [Key]
    public int CategoryId { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateUpdated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateDeleted { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ProductCategories")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductCategories")]
    public virtual Product Product { get; set; } = null!;
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("CategoryId", Name = "IX_Subcategories_CategoryId")]
public partial class Subcategory : EntityBase
{

    [StringLength(20)]
    public string Code { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string About { get; set; } = null!;

    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Subcategories")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Subcategory")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

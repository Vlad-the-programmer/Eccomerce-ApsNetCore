using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

public partial class Category : EntityBase
{
    public string Code { get; set; } = null!;

    [StringLength(20)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string About { get; set; } = null!;


    [InverseProperty("Category")]
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();

    [InverseProperty("Category")]
    public virtual ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
}

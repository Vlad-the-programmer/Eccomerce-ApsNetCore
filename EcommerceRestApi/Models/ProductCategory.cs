using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[PrimaryKey("ProductId", "CategoryId")]
public partial class ProductCategory : EntityBase
{
    [Key]
    public int ProductId { get; set; }

    [Key]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ProductCategories")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductCategories")]
    public virtual Product Product { get; set; } = null!;
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("CustomerId", Name = "IX_Reviews_CustomerId")]
[Index("ProductId", Name = "IX_Reviews_ProductId")]
public partial class Review : EntityBase
{
    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public int Rating { get; set; }

    [Column(TypeName = "text")]
    public string ReviewText { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Reviews")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("Reviews")]
    public virtual Product Product { get; set; } = null!;
}

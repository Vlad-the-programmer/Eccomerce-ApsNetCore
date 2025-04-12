using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("ProductId", Name = "IX_ShoppingCartItems_ProductId")]
public partial class ShoppingCartItem : EntityBase
{
    public int ProductId { get; set; }

    public int Amount { get; set; }

    [StringLength(20)]
    public string ShoppingCartId { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ShoppingCartItems")]
    public virtual Product Product { get; set; } = null!;
}

using EcommerceRestApi.Helpers.ModelsUtils;
using EcommerceRestApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("ProductId", Name = "IX_ShoppingCartItems_ProductId")]
public partial class ShoppingCartItem : EntityBase
{
    public int ProductId { get; set; }

    public int Amount { get; set; }

    [StringLength(50)]
    public string ShoppingCartId { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ShoppingCartItems")]
    public virtual Product Product { get; set; } = null!;

    public static explicit operator ShoppingCartItem(ShoppingCartItemDTO cartItemVM)
    {
        var cartItem = new ShoppingCartItem().CopyProperties(cartItemVM);
        return cartItem;
    }
}

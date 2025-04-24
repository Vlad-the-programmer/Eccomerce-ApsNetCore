using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

public partial class OrderItem : EntityBase
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderItems")]
    public virtual Product Product { get; set; } = null!;

    public static OrderItem CartItemToOrderItem(ShoppingCartItem cartItem, int orderId)
    {
        return new OrderItem
        {
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Amount,
            UnitPrice = cartItem.Product.Price,
            DateCreated = DateTime.Now,
            IsActive = cartItem.IsActive,
            OrderId = orderId,
        };
    }
}

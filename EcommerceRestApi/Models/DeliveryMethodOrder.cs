using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[PrimaryKey("DeliveryMethodId", "OrderId")]
[Index("OrderId", Name = "IX_DeliveryMethodOrders_OrderId")]
public partial class DeliveryMethodOrder
{
    [Key]
    public int DeliveryMethodId { get; set; }

    [Key]
    public int OrderId { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateUpdated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateDeleted { get; set; }

    [ForeignKey("DeliveryMethodId")]
    [InverseProperty("DeliveryMethodOrders")]
    public virtual DeliveryMethod DeliveryMethod { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("DeliveryMethodOrders")]
    public virtual Order Order { get; set; } = null!;
}

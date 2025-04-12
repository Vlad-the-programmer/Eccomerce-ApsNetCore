using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("DeliveryMethodId", Name = "IX_Shipments_DeliveryMethodId")]
[Index("OrderId", Name = "IX_Shipments_OrderId")]
public partial class Shipment : EntityBase
{

    public int OrderId { get; set; }

    public int DeliveryMethodId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ShipmentDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EstimatedArrivalDate { get; set; }

    [ForeignKey("DeliveryMethodId")]
    [InverseProperty("Shipments")]
    public virtual DeliveryMethod DeliveryMethod { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("Shipments")]
    public virtual Order Order { get; set; } = null!;
}

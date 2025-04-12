using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

public partial class DeliveryMethod : EntityBase
{
    [StringLength(50)]
    [Unicode(false)]
    public string MethodName { get; set; } = null!;

    [InverseProperty("DeliveryMethod")]
    public virtual ICollection<DeliveryMethodOrder> DeliveryMethodOrders { get; set; } = new List<DeliveryMethodOrder>();

    [InverseProperty("DeliveryMethod")]
    public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
}

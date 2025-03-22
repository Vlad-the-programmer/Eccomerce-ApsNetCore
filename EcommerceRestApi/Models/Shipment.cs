using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EcommerceRestApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

public partial class Shipment : EntityBase
{

    [Required(ErrorMessage = "Order ID is required.")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Delivery Method ID is required.")]
    public int DeliveryMethodId { get; set; }

    [Required(ErrorMessage = "Shipment date is required.")]
    [Column(TypeName = "datetime")]
    public DateTime ShipmentDate { get; set; }

    [Required(ErrorMessage = "Estimated arrival date is required.")]
    [Column(TypeName = "datetime")]
    [DateGreaterThan("ShipmentDate", ErrorMessage = "Estimated arrival date must be after shipment date.")]
    public DateTime EstimatedArrivalDate { get; set; }

    [ForeignKey("DeliveryMethodId")]
    [InverseProperty("Shipments")]
    public virtual DeliveryMethod DeliveryMethod { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("Shipments")]
    public virtual Order Order { get; set; } = null!;
}

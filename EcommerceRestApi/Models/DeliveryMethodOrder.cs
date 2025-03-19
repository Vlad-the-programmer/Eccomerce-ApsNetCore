using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[PrimaryKey("DeliveryMethodId", "OrderId")]
public partial class DeliveryMethodOrder : EntityBase
{
    [Key]
    public int DeliveryMethodId { get; set; }

    [Key]
    public int OrderId { get; set; }


    [ForeignKey("DeliveryMethodId")]
    [InverseProperty("DeliveryMethodOrders")]
    public virtual DeliveryMethod DeliveryMethod { get; set; } = null!;

    [ForeignKey("OrderId")]
    [InverseProperty("DeliveryMethodOrders")]
    public virtual Order Order { get; set; } = null!;
}

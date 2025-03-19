using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[PrimaryKey("OrderId", "ProductId")]
public partial class OrderItem : EntityBase
{
    [Key]
    [Required(ErrorMessage = "Order ID is required.")]
    public int OrderId { get; set; }

    [Key]
    [Required(ErrorMessage = "Product ID is required.")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Unit Price is required.")]
    [Column(TypeName = "money")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0.")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("OrderItems")]
    public virtual Product Product { get; set; } = null!;
}

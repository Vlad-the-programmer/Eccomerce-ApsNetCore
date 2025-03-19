using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

public partial class Payment : EntityBase
{
    [Key]
    public int Id { get; set; }

    public int? OrderId { get; set; } = null!;

    public int? PaymentMethodId { get; set; } = null;

    [Required(ErrorMessage = "Amount is required.")]
    [Column(TypeName = "money")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Payment Date is required.")]
    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    [InverseProperty("Payment")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("OrderId")]
    [InverseProperty("Payments")]
    public virtual Order? Order { get; set; }

    [ForeignKey("PaymentMethodId")]
    [InverseProperty("Payments")]
    public virtual PaymentMethod? PaymentMethod { get; set; }
}

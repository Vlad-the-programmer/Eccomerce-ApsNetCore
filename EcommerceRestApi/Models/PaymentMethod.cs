using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

public partial class PaymentMethod : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Payment Type is required.")]
    [StringLength(50, ErrorMessage = "Payment Type cannot exceed 50 characters.")]
    [Unicode(false)]
    public string PaymentType { get; set; } = null!;

    [StringLength(100, ErrorMessage = "Details cannot exceed 100 characters.")]
    [Unicode(false)]
    public string? Details { get; set; }

    [InverseProperty("PaymentMethod")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

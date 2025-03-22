using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[Table("InvoiceItem")]
public partial class InvoiceItem : EntityBase
{

    [Required(ErrorMessage = "InvoiceId is required.")]
    public int? InvoiceId { get; set; }

    [Required(ErrorMessage = "ProductId is required.")]
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "BasePricePerUnit is required.")]
    [Column(TypeName = "decimal(18, 0)")]
    [Range(0.01, double.MaxValue, ErrorMessage = "BasePricePerUnit must be greater than zero.")]
    public decimal BasePricePerUnit { get; set; }

    [Required(ErrorMessage = "TaxRate is required.")]
    [Range(0, 100, ErrorMessage = "TaxRate must be between 0 and 100%.")]
    public double TaxRate { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public double Quantity { get; set; }

    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100%.")]
    public double Discount { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceItems")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("InvoiceItems")]
    public virtual Product? Product { get; set; }
}

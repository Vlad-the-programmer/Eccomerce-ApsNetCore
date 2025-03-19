using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Models;

[Table("Invoice")]
public partial class Invoice : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "CustomerId is required.")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Invoice Number is required.")]
    [StringLength(16, ErrorMessage = "Invoice number should be no more than 16 characters long.")]
    [Unicode(false)]
    public string Number { get; set; }

    [Required(ErrorMessage = "Date of issue is required.")]
    [Column(TypeName = "datetime")]
    public DateTime DateOfIssue { get; set; }

    public int? PaymentId { get; set; } = null;

    public bool IsPaid { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Invoices")]
    public virtual Customer? Customer { get; set; }

    [InverseProperty("Invoice")]
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    [ForeignKey("PaymentId")]
    [InverseProperty("Invoices")]
    public virtual Payment? Payment { get; set; }
}

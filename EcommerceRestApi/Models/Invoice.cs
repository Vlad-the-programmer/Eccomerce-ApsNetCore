using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Table("Invoice")]
[Index("CustomerId", Name = "IX_Invoice_CustomerId")]
[Index("PaymentId", Name = "IX_Invoice_PaymentId")]
public partial class Invoice : EntityBase
{
    public int CustomerId { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string Number { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DateOfIssue { get; set; }

    public int? PaymentId { get; set; }

    public bool IsPaid { get; set; }

    public int OrderId { get; set; }
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; } = null!;
    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    [ForeignKey("PaymentId")]
    public virtual Payment? Payment { get; set; }
}

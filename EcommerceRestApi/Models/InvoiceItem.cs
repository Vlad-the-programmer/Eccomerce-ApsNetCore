using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Table("InvoiceItem")]
[Index("InvoiceId", Name = "IX_InvoiceItem_InvoiceId")]
[Index("ProductId", Name = "IX_InvoiceItem_ProductId")]
public partial class InvoiceItem : EntityBase
{
    public int InvoiceId { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "decimal(18, 0)")]
    public decimal BasePricePerUnit { get; set; }

    public double TaxRate { get; set; }

    public double Quantity { get; set; }

    public double Discount { get; set; }

    [ForeignKey("InvoiceId")]
    [InverseProperty("InvoiceItems")]
    public virtual Invoice Invoice { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("InvoiceItems")]
    public virtual Product Product { get; set; } = null!;
}

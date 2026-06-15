using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

[Index("OrderId", Name = "IX_Payments_OrderId")]
[Index("PaymentMethodId", Name = "IX_Payments_PaymentMethodId")]
public partial class Payment : EntityBase
{

    public int? OrderId { get; set; }

    public int? PaymentMethodId { get; set; }

    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime PaymentDate { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("OrderId")]
    public virtual Order? Order { get; set; }

    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod? PaymentMethod { get; set; }
}

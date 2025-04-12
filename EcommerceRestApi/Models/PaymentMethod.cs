using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models;

public partial class PaymentMethod : EntityBase
{

    [StringLength(50)]
    [Unicode(false)]
    public string PaymentType { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Details { get; set; }

    [InverseProperty("PaymentMethod")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

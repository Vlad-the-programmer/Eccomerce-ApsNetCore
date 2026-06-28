using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class Return : EntityBase
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int RefundId { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
        [ForeignKey(nameof(RefundId))]
        public virtual Refund Refund { get; set; }

    }
}

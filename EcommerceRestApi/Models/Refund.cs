using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class Refund : EntityBase
    {
        public string Code { get; set; }
        public int CustomerId { get; set; }
        public int? PaymentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Status { get; set; } = "Pending";
        public DateTime? ProcessedDate { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public virtual Payment? Payment { get; set; }
        public virtual ICollection<RefundItem> RefundItems { get; set; } = new List<RefundItem>();
        public virtual ICollection<RefundStatusHistory> RefundStatusHistory { get; set; } = new List<RefundStatusHistory>();

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; }
        public virtual Return Return { get; set; }
    }
}

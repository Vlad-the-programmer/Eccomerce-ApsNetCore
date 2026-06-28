using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class RefundStatusHistory : EntityBase
    {
        public int RefundId { get; set; }

        public string RefundCode { get; set; }

        public string Status { get; set; }

        public string ChangedBy { get; set; } // Could be AdminId or UserId

        [ForeignKey(nameof(RefundId))]
        public virtual Refund Refund { get; set; }
    }
}

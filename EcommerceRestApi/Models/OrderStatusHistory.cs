using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    public class OrderStatusHistory : EntityBase
    {
        public int OrderId { get; set; }

        public string Status { get; set; }

        public string ChangedBy { get; set; } // Could be AdminId or UserId

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; }
    }
}

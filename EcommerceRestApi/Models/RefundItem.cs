using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Models
{
    [Table("RefundItems")]
    [Index(nameof(RefundId), Name = "IX_RefundItems_RefundId")]
    [Index(nameof(OrderItemId), Name = "IX_RefundItems_OrderItemId")]
    [Index(nameof(DateCreated), Name = "IX_RefundItems_DateCreated")]
    [Index(nameof(RefundId), nameof(OrderItemId), Name = "IX_RefundItems_Refund_OrderItem", IsUnique = true)]
    public class RefundItem : EntityBase
    {

        [Required]
        public int RefundId { get; set; }

        [Required]
        public int OrderItemId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Refund amount must be greater than 0")]
        public decimal RefundAmount { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        [ForeignKey(nameof(RefundId))]
        public virtual Refund? Refund { get; set; }

        [ForeignKey(nameof(OrderItemId))]
        public virtual OrderItem? OrderItem { get; set; }
    }
}

namespace EcommerceRestApi.Models
{
    public class Coupon : EntityBase
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public decimal? MaxDiscountAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int? PerUserLimit { get; set; }

        public virtual ICollection<OrderCoupon> OrderCoupons { get; set; } = new List<OrderCoupon>();
    }
}

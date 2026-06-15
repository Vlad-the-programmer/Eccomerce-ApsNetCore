namespace EcommerceRestApi.Models
{
    public class OrderCoupon : EntityBase
    {
        public int OrderId { get; set; }
        public int CouponId { get; set; }
        public decimal DiscountApplied { get; set; }
        public DateTime AppliedAt { get; set; }

        public Order Order { get; set; } = null!;
        public Coupon Coupon { get; set; } = null!;
    }
}

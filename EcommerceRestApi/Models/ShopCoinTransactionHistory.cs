namespace EcommerceRestApi.Models
{
    public class ShopCoinTransactionHistory : EntityBase
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int Coins { get; set; }

        public string Type { get; set; } = null!; // "EARN_ORDER", "SPEND_ORDER", "BONUS", "REFUND"

        public string? Description { get; set; }

        public int? OrderId { get; set; }
        public Order? Order { get; set; }
    }
}

namespace EcommerceRestApi.Models
{
    public class ShopCoinSettings : EntityBase
    {
        public decimal EarnRate { get; set; }
        public decimal SpendRate { get; set; }
        public decimal MaxSpendPercentage { get; set; }
    }

}

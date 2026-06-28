namespace EcommerceWebApp.Models.Dtos
{
    public class ShopCoinSettingsDto
    {
        public decimal EarnRate { get; set; }
        public decimal SpendRate { get; set; }
        public decimal MaxSpendPercentage { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}

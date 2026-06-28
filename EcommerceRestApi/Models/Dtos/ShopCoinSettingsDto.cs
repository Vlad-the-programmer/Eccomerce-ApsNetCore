using System.ComponentModel.DataAnnotations;

namespace EcommerceRestApi.Models.Dtos
{
    public class ShopCoinSettingsDto
    {
        public decimal EarnRate { get; set; }
        public decimal SpendRate { get; set; }

        [Range(0, 1)]
        public decimal MaxSpendPercentage { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}

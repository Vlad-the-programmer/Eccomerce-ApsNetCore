using EcommerceWebApp.Models.Dtos.Analitics;

namespace EcommerceWebApp.Models.Analitics
{
    public class AnalyticsViewModel
    {
        public DashboardSummaryDto? Dashboard { get; set; }
        public RevenueAnalyticsDto? Revenue { get; set; }
        public OrdersAnalyticsDto? Orders { get; set; }
        public RefundAnalyticsDto? Refunds { get; set; }
        public ReturnAnalyticsDto? Returns { get; set; }
        public List<TopProductDto>? TopProducts { get; set; } = new List<TopProductDto>();
        public CustomerAnalyticsDto? Customers { get; set; }
    }
}

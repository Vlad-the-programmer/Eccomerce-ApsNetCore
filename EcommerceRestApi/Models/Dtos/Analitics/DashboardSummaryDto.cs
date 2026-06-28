namespace EcommerceRestApi.Models.Dtos.Analitics
{
    public class DashboardSummaryDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalRefunds { get; set; }
        public int TotalReturns { get; set; }

        public decimal RevenueToday { get; set; }
        public int OrdersToday { get; set; }
    }
}

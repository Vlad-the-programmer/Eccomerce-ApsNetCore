namespace EcommerceRestApi.Models.Dtos.Analitics
{
    public class RevenueAnalyticsDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }

        public List<ChartPointDto> RevenueByDate { get; set; } = new();
    }
}

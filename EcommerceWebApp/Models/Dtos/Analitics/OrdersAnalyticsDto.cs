namespace EcommerceWebApp.Models.Dtos.Analitics
{
    public class OrdersAnalyticsDto
    {
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }

        public List<ChartPointDto> OrdersByDate { get; set; } = new();
    }
}

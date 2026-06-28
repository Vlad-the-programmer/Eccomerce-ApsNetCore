namespace EcommerceWebApp.Models.Dtos.Analitics
{
    public class RefundAnalyticsDto
    {
        public int TotalRefunds { get; set; }
        public decimal TotalRefundAmount { get; set; }

        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }
}

namespace EcommerceRestApi.Models.Dtos.Analitics
{
    public class ReturnAnalyticsDto
    {
        public int TotalReturns { get; set; }

        public int Pending { get; set; }
        public int Processing { get; set; }
        public int Delivered { get; set; }
        public int Cancelled { get; set; }
    }
}

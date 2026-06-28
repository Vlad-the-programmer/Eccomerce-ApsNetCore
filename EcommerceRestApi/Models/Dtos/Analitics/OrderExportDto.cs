namespace EcommerceRestApi.Models.Dtos.Analitics
{
    public class OrderExportDto
    {
        public string OrderId { get; set; }              // A
        public DateTime OrderDate { get; set; }          // B
        public string CustomerName { get; set; }         // C
        public string Region { get; set; }               // D
        public string ProductName { get; set; }          // E
        public string Category { get; set; }             // F
        public int Quantity { get; set; }                // G
        public decimal UnitPrice { get; set; }           // H
        public decimal DiscountPercent { get; set; }     // I
        public decimal TotalRevenue { get; set; }        // J
        public decimal Profit { get; set; }              // K
        public string ShipMode { get; set; }             // L
    }

}

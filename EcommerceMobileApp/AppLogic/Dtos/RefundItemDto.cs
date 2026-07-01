namespace EcommerceMobileApp.AppLogic.Dtos
{
    public class RefundItemDto
    {
        public int Quantity { get; set; }
        public decimal RefundAmount { get; set; }
        public string? Reason { get; set; }
        public string ProductName { get; set; }
        public string ProductBrand { get; set; }
        public string ProductPhoto { get; set; }
    }
}

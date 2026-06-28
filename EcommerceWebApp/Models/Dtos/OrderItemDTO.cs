namespace EcommerceWebApp.Models.Dtos
{
    public class OrderItemDTO
    {
        public int? Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; }
        public string ProductBrand { get; set; }
        public string? ProductPhoto { get; set; }
    }
}

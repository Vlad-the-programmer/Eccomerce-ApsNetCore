namespace EcommerceRestApi.Models.Dtos
{
    public class InventoryItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
        public int Reserved { get; set; }
        public int Available => Stock - Reserved;
        public decimal UnitCost { get; set; }
        public decimal InventoryValue => Available * UnitCost;
    }

}

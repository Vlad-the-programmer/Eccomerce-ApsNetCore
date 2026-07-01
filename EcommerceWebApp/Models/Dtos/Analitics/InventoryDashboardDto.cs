namespace EcommerceWebApp.Models.Dtos.Analitics
{
    public class InventoryDashboardDto
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public decimal TotalInventoryValue { get; set; }

        public List<InventoryItemDto> Items { get; set; } = new();
        public List<LowStockItemDto> LowStockItems { get; set; } = new();
    }
}

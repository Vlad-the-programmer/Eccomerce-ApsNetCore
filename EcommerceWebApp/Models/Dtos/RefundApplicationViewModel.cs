namespace EcommerceWebApp.Models.ViewModels
{
    public class RefundApplicationViewModel
    {
        public string OrderCode { get; set; }

        public List<RefundOrderItemViewModel> OrderItems { get; set; } = new List<RefundOrderItemViewModel>();
    }

    public class RefundOrderItemViewModel
    {
        public int OrderItemId { get; set; }
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductBrand { get; set; }
        public string? ProductPhoto { get; set; }
        public decimal UnitPrice { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public decimal TotalPrice => UnitPrice * Quantity;

        public int RefundQuantity { get; set; } = 0;
        public string? Reason { get; set; }
        public bool IsSelected { get; set; }
    }
}
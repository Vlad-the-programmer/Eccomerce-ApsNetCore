namespace EcommerceWebApp.Models.Dtos
{
    public class CreateRefundDto
    {
        public string OrderCode { get; set; }
        public List<CreateRefundItemDto> OrderItems { get; set; } = new List<CreateRefundItemDto>();
    }

    public class CreateRefundItemDto
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public string? Reason { get; set; }
    }
}

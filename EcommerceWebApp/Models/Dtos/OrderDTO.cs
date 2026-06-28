namespace EcommerceWebApp.Models.Dtos
{
    public class OrderDTO
    {
        public string Code { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public CustomerDTO Customer { get; set; } = new CustomerDTO();

        public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
        public IList<OrderStatusHistoryDto> StatusHistory { get; set; } = new List<OrderStatusHistoryDto>();

        public string DeliveryMethod { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderStatus { get; set; }
        public bool IsPaid { get; set; }
    }
}

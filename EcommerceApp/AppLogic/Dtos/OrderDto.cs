namespace EcommerceApp.AppLogic.Dtos
{
    public class OrderDto
    {
        public string Code { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderCustomerDTO Customer { get; set; } = new OrderCustomerDTO();

        public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

        public string DeliveryMethod { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderStatus { get; set; }
        public bool IsPaid { get; set; }

    }
}

using EcommerceWebApp.Models.Dtos;

namespace EcommerceWebApp.Models
{
    public class OrderViewModel : BaseViewModel
    {
        public OrderViewModel() : base("Order") { }

        public CustomerDTO Customer { get; set; } = new CustomerDTO();

        //public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

        public int DeliveryMethod { get; set; }

        public int PaymentMethod { get; set; }

        public int OrderStatus { get; set; }
    }

}

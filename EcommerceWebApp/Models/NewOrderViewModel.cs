using EcommerceWebApp.Models.Dtos;

namespace EcommerceWebApp.Models
{
    public class NewOrderViewModel : BaseViewModel
    {
        public NewOrderViewModel() : base("Order") { }

        public decimal TotalAmount { get; set; }
        public decimal TaxRate { get; set; }

        public CustomerDTO Customer { get; set; } = new CustomerDTO();

        public int DeliveryMethod { get; set; }

        public int PaymentMethod { get; set; }

        public int OrderStatus { get; set; }
    }

}

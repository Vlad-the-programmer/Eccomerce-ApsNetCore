using EcommerceWebApp.Helpers.Enums;

namespace EcommerceWebApp.Models
{
    public class OrderViewModel : BaseViewModel
    {
        public OrderViewModel() : base("Order") { }

        public string? Code { get; set; }

        public int? CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public ApplicationUserViewModel Customer { get; set; } = new ApplicationUserViewModel();

        public IList<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public DeliveryMethods DeliveryMethod { get; set; }

        public PaymentMethods PaymentMethod { get; set; }

        public OrderStatuses OrderStatus { get; set; }
    }

}

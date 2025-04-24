using EcommerceWebApp.Helpers.Enums;
using System.ComponentModel.DataAnnotations;

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

        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(20)]
        public string? HouseNumber { get; set; }

        [StringLength(20)]
        public string? FlatNumber { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }
        public string? CountryName { get; set; }

        public DeliveryMethods DeliveryMethod { get; set; }

        public PaymentMethods PaymentMethod { get; set; }

        public OrderStatuses OrderStatus { get; set; }
    }

}

using EcommerceWebApp.Helpers.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWebApp.Models
{
    public class OrderViewModel : BaseViewModel
    {
        public OrderViewModel() : base("Order") { }

        [Required(ErrorMessage = "Order code is required.")]
        [StringLength(50, ErrorMessage = "Order code cannot exceed 50 characters.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Customer ID is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Total amount is required.")]
        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        public decimal TotalAmount { get; set; }

        //[Required(ErrorMessage = "Status is required.")]
        //[StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        //public string Status { get; set; } = null!;

        public virtual ApplicationUserViewModel Customer { get; set; } = null!;

        //public IList<DeliveryMethodOrder> DeliveryMethodOrders { get; set; } = new List<DeliveryMethodOrder>();

        public IList<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        //public IList<Payment> Payments { get; set; } = new List<Payment>();

        //public IList<Shipment> Shipments { get; set; } = new List<Shipment>();

        public string Address { get; set; }
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

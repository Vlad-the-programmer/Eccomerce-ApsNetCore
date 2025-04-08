using EcommerceRestApi.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceRestApi.Helpers.Enums;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class OrderViewModel: BaseViewModel
    {
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

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        [Unicode(false)]
        public string Status { get; set; } = null!;

        public virtual Customer Customer { get; set; } = null!;

        //public IList<DeliveryMethodOrder> DeliveryMethodOrders { get; set; } = new List<DeliveryMethodOrder>();

        public IList<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        //public IList<Payment> Payments { get; set; } = new List<Payment>();

        //public IList<Shipment> Shipments { get; set; } = new List<Shipment>();

        public DeliveryMethods DeliveryMethod { get; set; }

        public PaymentMethods PaymentMethod { get; set; }

        public OrderStatuses OrderStatus { get; set; }
    }

}

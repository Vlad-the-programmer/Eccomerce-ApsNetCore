using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        //[Required(ErrorMessage = "Order code is required.")]
        //[StringLength(50, ErrorMessage = "Order code cannot exceed 50 characters.")]
        //public string? Code { get; set; }

        //[Required(ErrorMessage = "Customer ID is required.")]
        public int? CustomerId { get; set; }

        //[Required(ErrorMessage = "Order date is required.")]
        //[Column(TypeName = "datetime")]
        //public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        public decimal? TotalAmount { get; set; }

        public CreateOrderCustomerDto Customer { get; set; } = new CreateOrderCustomerDto();
        public IList<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        public int DeliveryMethod { get; set; }

        public int PaymentMethod { get; set; }

        public int OrderStatus { get; set; }

        public static OrderViewModel OrderToVm(Order order, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            var address = order.Customer.Addresses.FirstOrDefault();

            var orderVm = new OrderViewModel
            {
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                OrderStatus = OrderProcessingFuncs.GetEnumValueForOrderStatus(order.Status),
                PaymentMethod = OrderProcessingFuncs.GetEnumValueForPaymentMethod(order.Payments.FirstOrDefault()?.PaymentMethod?.PaymentType),
                DeliveryMethod = OrderProcessingFuncs.GetEnumValueForDeliveryMethod(order.DeliveryMethodOrders.FirstOrDefault()?.DeliveryMethod.MethodName),
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product.Price,
                    OrderId = oi.OrderId,
                    ProductName = oi.Product.Name,
                    ProductBrand = oi.Product.Brand,
                }).ToList(),
                Customer = new CreateOrderCustomerDto()
            };

            orderVm.Customer = CreateOrderCustomerDto.ToDto(order.Customer, userManager);
            return orderVm;
        }
    }
}

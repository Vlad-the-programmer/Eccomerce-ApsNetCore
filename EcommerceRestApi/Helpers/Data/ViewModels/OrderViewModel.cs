using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        //[Required(ErrorMessage = "Order code is required.")]
        [StringLength(50, ErrorMessage = "Order code cannot exceed 50 characters.")]
        public string? Code { get; set; }

        //[Required(ErrorMessage = "Customer ID is required.")]
        public int? CustomerId { get; set; }

        [Required(ErrorMessage = "Order date is required.")]
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "money")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        public decimal? TotalAmount { get; set; }

        public CustomerViewModel? Customer { get; set; }
        public IList<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
        public DeliveryMethods DeliveryMethod { get; set; }

        public PaymentMethods PaymentMethod { get; set; }

        public OrderStatuses OrderStatus { get; set; }

        public static OrderViewModel OrderToVm(Order order, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            var address = order.Customer.Addresses.FirstOrDefault();

            var orderVm = new OrderViewModel
            {
                Code = order.Code,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
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
                Customer = new CustomerViewModel()
            };
            //orderVm.Customer.Street = address?.Street;
            //orderVm.Customer.City = address?.City;
            //orderVm.Customer.FlatNumber = address?.FlatNumber;
            //orderVm.Customer.HouseNumber = address?.HouseNumber;
            //orderVm.Customer.State = address?.State;
            //orderVm.Customer.PostalCode = address?.PostalCode;
            //orderVm.Customer.CountryName = address?.CountryId != null
            //                               ? context.Countries.FirstOrDefault(
            //                                            c => c.Id == address.CountryId)
            //                                                ?.CountryName : string.Empty;
            //orderVm.Customer.FirstName = order.Customer.User.FirstName;
            //orderVm.Customer.LastName = order.Customer.User.LastName;
            //orderVm.Customer.UserName = order.Customer.User.UserName;
            //orderVm.Customer.Role = order.Customer.User.Role;
            //orderVm.Customer.IsActive = order.Customer.IsActive;
            //orderVm.Customer.IsAdmin = order.Customer.IsA;
            orderVm.Customer = CustomerViewModel.ToVM(order.Customer, userManager);
            return orderVm;
        }
    }
}

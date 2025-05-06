using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class NewOrderViewModel : BaseViewModel
    {
        public CreateOrderCustomerDto Customer { get; set; } = new CreateOrderCustomerDto();
        public decimal TotalAmount { get; set; }
        public decimal TaxRate { get; set; }

        public int DeliveryMethod { get; set; }

        public int PaymentMethod { get; set; }

        public int OrderStatus { get; set; }

        public static NewOrderViewModel OrderToVm(Order order, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            var address = order.Customer.Addresses.FirstOrDefault();

            var orderVm = new NewOrderViewModel
            {
                OrderStatus = OrderProcessingFuncs.GetEnumValueForOrderStatus(order.Status),
                PaymentMethod = OrderProcessingFuncs.GetEnumValueForPaymentMethod(order.Payments.FirstOrDefault()?.PaymentMethod?.PaymentType),
                DeliveryMethod = OrderProcessingFuncs.GetEnumValueForDeliveryMethod(order.DeliveryMethodOrders.FirstOrDefault()?.DeliveryMethod.MethodName),
                Customer = new CreateOrderCustomerDto()
            };

            orderVm.Customer = CreateOrderCustomerDto.ToDto(order.Customer, userManager);
            return orderVm;
        }
    }
}

using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace EcommerceRestApi.Helpers.Data.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public CreateOrderCustomerDto Customer { get; set; } = new CreateOrderCustomerDto();
        public int DeliveryMethod { get; set; }

        public int PaymentMethod { get; set; }

        public int OrderStatus { get; set; }

        public static OrderViewModel OrderToVm(Order order, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            var address = order.Customer.Addresses.FirstOrDefault();

            var orderVm = new OrderViewModel
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

using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models.Context;
using Microsoft.AspNetCore.Identity;

namespace EcommerceRestApi.Models.Dtos
{
    public class OrderDto
    {
        public string Code { get; set; }

        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public CustomerViewModel Customer { get; set; } = new CustomerViewModel();

        public IList<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public string DeliveryMethod { get; set; }

        public string PaymentMethod { get; set; }

        public string OrderStatus { get; set; }
        public bool IsPaid { get; set; }

        public static OrderDto OrderToDto(Order order, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            var address = order.Customer.Addresses.FirstOrDefault();

            var orderDto = new OrderDto
            {
                Code = order.Code,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.Status,
                PaymentMethod = order.Payments.FirstOrDefault()?.PaymentMethod?.PaymentType ?? string.Empty,
                DeliveryMethod = order.DeliveryMethodOrders.FirstOrDefault()?.DeliveryMethod.MethodName ?? string.Empty,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product.Price,
                    OrderId = oi.OrderId,
                    ProductName = oi.Product.Name,
                    ProductBrand = oi.Product.Brand,
                }).ToList(),
                Customer = new CustomerViewModel(),
                IsPaid = order.IsPaid
            };

            orderDto.Customer = CustomerViewModel.ToVM(order.Customer, userManager);
            return orderDto;
        }
    }
}

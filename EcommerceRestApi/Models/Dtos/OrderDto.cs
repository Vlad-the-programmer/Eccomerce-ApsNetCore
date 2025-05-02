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

        public OrderCustomerDTO Customer { get; set; } = new OrderCustomerDTO();

        public IList<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();

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
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product.Price,
                    OrderId = oi.OrderId,
                    ProductName = oi.Product.Name,
                    ProductBrand = oi.Product.Brand,
                }).ToList(),
                Customer = new OrderCustomerDTO(),
                IsPaid = order.IsPaid
            };

            orderDto.Customer = OrderCustomerDTO.ToVM(order.Customer, userManager);
            return orderDto;
        }
    }
}

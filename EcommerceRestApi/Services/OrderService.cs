using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services.Base;
using EcommerceRestApi.Helpers.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using EcommerceRestApi.Helpers.Data.Functions;

namespace EcommerceRestApi.Services
{
    public class OrderService : EntityBaseRepository<Order>, IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<OrderViewModel?> GetOrderByCodeAsync(string code)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Customer)
                .Include(o => o.DeliveryMethodOrders)
                .Include(o => o.Payments)
                .Include(o => o.Shipments)
                .FirstOrDefaultAsync(o => o.Code == code);

            if (order == null)
            {
                return null;
            }

            return new OrderViewModel
            {
                Code = order.Code,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = OrderProcessingFuncs.GetEnumValueForOrderStatus(order.Status),
                PaymentMethod = OrderProcessingFuncs.GetEnumValueForPaymentMethod(order.Payments.FirstOrDefault()?.PaymentMethod?.PaymentType),
                DeliveryMethod = OrderProcessingFuncs.GetEnumValueForDeliveryMethod(order.DeliveryMethodOrders.FirstOrDefault()?.DeliveryMethod.MethodName),
                OrderItems = (IList<OrderItem>)order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderId = oi.OrderId,
                }).ToList()
            };
        }

        public async Task UpdateOrderAsync(string code, OrderViewModel data)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Code == code);
            if (order == null)
                throw new KeyNotFoundException($"Order with code '{code}' not found.");

            order.TotalAmount = data.TotalAmount;
            order.Status = OrderProcessingFuncs.GetStringValue(data.OrderStatus);
            order.OrderDate = data.OrderDate;
            order.CustomerId = data.CustomerId;
            order.DateUpdated = DateTime.Now;


            order.DeliveryMethodOrders.First().DeliveryMethodId = _context.DeliveryMethods.First(
                            m => m.MethodName == OrderProcessingFuncs.GetStringValue(data.DeliveryMethod)).Id;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(string code)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Code == code);
            order.DateDeleted = DateTime.Now;
            order.IsActive = false;

            await _context.SaveChangesAsync();
        }

        public async Task AddNewOrderAsync(OrderViewModel data)
        {

            var order = new Order
            {
                Code = data.Code,
                CustomerId = data.CustomerId,
                OrderDate = data.OrderDate,
                TotalAmount = data.TotalAmount,
                OrderItems = new List<OrderItem>()
            };

            order.Status = OrderProcessingFuncs.GetStringValue(data.OrderStatus);
            order.Payments.Add(new Payment { 
                Amount = data.TotalAmount,
                OrderId = order.Id,
                IsActive = true,
                PaymentDate = DateTime.Now,
                PaymentMethodId = _context.PaymentMethods.First(m => m.PaymentType == OrderProcessingFuncs.GetStringValue(data.PaymentMethod)).Id,
                DateCreated = DateTime.Now,
            });

            order.DateCreated = DateTime.Now;

            order.DeliveryMethodOrders.Add(new DeliveryMethodOrder {
                OrderId = order.Id,
                DeliveryMethodId = _context.DeliveryMethods.First(m => m.MethodName == OrderProcessingFuncs.GetStringValue(data.DeliveryMethod)).Id,   
                IsActive=true,
                DateCreated = DateTime.Now
            });

            foreach (var item in data.OrderItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DateCreated = DateTime.Now,
                });
            }

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync()
        {
            return await _context.Orders.Include(o => o.Customer)
                                        .Include(o => o.OrderItems)
                                        .Select(o => new OrderViewModel
                                        {
                                            Code = o.Code,
                                            CustomerId = o.CustomerId,
                                            OrderDate = o.OrderDate,
                                            TotalAmount = o.TotalAmount,
                                            OrderStatus = OrderProcessingFuncs.GetEnumValueForOrderStatus(o.Status),
                                            PaymentMethod = OrderProcessingFuncs.GetEnumValueForPaymentMethod(o.Payments.FirstOrDefault().PaymentMethod.PaymentType),
                                            DeliveryMethod = OrderProcessingFuncs.GetEnumValueForDeliveryMethod(o.DeliveryMethodOrders.FirstOrDefault().DeliveryMethod.MethodName),
                                            OrderItems = (IList<OrderItem>)o.OrderItems.Select(oi => new OrderItemViewModel
                                            {
                                                ProductId = oi.ProductId,
                                                Quantity = oi.Quantity,
                                                UnitPrice = oi.UnitPrice,
                                                OrderId = oi.OrderId,
                                            }).ToList()
                                        }).ToListAsync();
        }
    }
}

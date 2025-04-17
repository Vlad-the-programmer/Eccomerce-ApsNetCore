using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services.Base;
using Microsoft.EntityFrameworkCore;

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
                .Include(item => item.Customer)
                                    .ThenInclude(item => item.User)
                                    .Include(item => item.Customer.Addresses)
                                    .Include(item => item.Shipments)
                                    .ThenInclude(item => item.DeliveryMethod)
                                    .ThenInclude(item => item.DeliveryMethodOrders)
                                    .Include(item => item.Payments)
                                    .Include(item => item.OrderItems)
                                    .ThenInclude(item => item.Product)
                                    .ThenInclude(item => item.ProductCategories)
                                    .FirstOrDefaultAsync(o => o.Code == code);

            if (order == null)
            {
                return null;
            }

            var address = order.Customer.Addresses.FirstOrDefault();
            return OrderViewModel.OrderToVm(order, _context);
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
            order.Payments.Add(new Payment
            {
                Amount = data.TotalAmount,
                OrderId = order.Id,
                IsActive = true,
                PaymentDate = DateTime.Now,
                PaymentMethodId = _context.PaymentMethods.First(m => m.PaymentType == OrderProcessingFuncs.GetStringValue(data.PaymentMethod)).Id,
                DateCreated = DateTime.Now,
            });

            order.DateCreated = DateTime.Now;

            order.DeliveryMethodOrders.Add(new DeliveryMethodOrder
            {
                OrderId = order.Id,
                DeliveryMethodId = _context.DeliveryMethods.First(m => m.MethodName == OrderProcessingFuncs.GetStringValue(data.DeliveryMethod)).Id,
                IsActive = true,
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

            Address newAddress = new Address
            {
                Street = data.Street,
                FlatNumber = data.FlatNumber,
                HouseNumber = data.HouseNumber,
                State = data.State,
                PostalCode = data.PostalCode,
                City = data.City,
                CountryId = data.CountryId ?? _context.Countries.FirstOrDefault(c => c.CountryName == data.CountryName)?.Id,
                CustomerId = data.CustomerId,
                DateCreated = DateTime.Now,
                IsActive = true
            };

            var oldAddress = order.Customer.Addresses.FirstOrDefault();
            if (oldAddress != null)
            {
                oldAddress = newAddress;
            }
            else
            {
                _context.Addresses.Add(newAddress);
                order.Customer.Addresses.Add(newAddress);
            }

            await _context.Orders.AddAsync(order);
            await InvoicePaymentHelperFuncs.GenerateInvoice(order, _context);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync()
        {
            return await _context.Orders
                                    .Include(item => item.Customer)
                                    .ThenInclude(item => item.User)
                                    .Include(item => item.Customer.Addresses)
                                    .Include(item => item.Shipments)
                                    .ThenInclude(item => item.DeliveryMethod)
                                    .ThenInclude(item => item.DeliveryMethodOrders)
                                    .Include(item => item.Payments)
                                    .Include(item => item.OrderItems)
                                    .ThenInclude(item => item.Product)
                                    .ThenInclude(item => item.ProductCategories)
                                        .Select(o =>
                                            OrderViewModel.OrderToVm(o, _context)
                                        ).ToListAsync();
        }
    }
}

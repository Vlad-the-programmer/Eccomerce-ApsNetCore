using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Services.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class OrderService : EntityBaseRepository<Order>, IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrderService(AppDbContext context,
                            IHttpContextAccessor httpContextAccessor,
                            UserManager<ApplicationUser> userManager) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<OrderViewModel?> GetOrderByCodeAsync(string code)
        {
            var order = await _context.Orders
                                    .Include(item => item.Customer)
                                            .ThenInclude(item => item.User)
                                    .Include(c => c.Customer)
                                            .ThenInclude(c => c.Invoices)
                                    .Include(c => c.Customer)
                                            .ThenInclude(c => c.Addresses)
                                                .ThenInclude(a => a.Country)
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

            return OrderViewModel.OrderToVm(order, _context, _userManager);
        }

        public async Task UpdateOrderAsync(string code, OrderViewModel data)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Code == code);
            if (order == null)
                throw new KeyNotFoundException($"Order with code '{code}' not found.");

            order.TotalAmount = data.TotalAmount ?? decimal.Zero;
            order.Status = OrderProcessingFuncs.GetStringValue(data.OrderStatus);
            order.OrderDate = data.OrderDate;
            order.CustomerId = data.CustomerId ?? 0;
            order.DateUpdated = DateTime.Now;


            order.DeliveryMethodOrders.First().DeliveryMethodId = _context.DeliveryMethods.First(
                            m => m.MethodName == OrderProcessingFuncs.GetStringValue(data.DeliveryMethod)).Id;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(string code)
        {
            var order = await _context.Orders
                .Include(c => c.Customer)
                    .ThenInclude(c => c.Invoices)
                .Include(o => o.Shipments)
                    .ThenInclude(s => s.DeliveryMethod)
                        .ThenInclude(dm => dm.DeliveryMethodOrders)
                .Include(o => o.Payments)
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryMethodOrders)
                .FirstOrDefaultAsync(o => o.Code == code);

            if (order == null)
                return;

            order.DateDeleted = DateTime.Now;
            order.IsActive = false;
            order.Status = OrderProcessingFuncs.GetStringValue(OrderStatuses.Cancelled);

            if (order.Shipments.Any())
            {
                var shipment = order.Shipments.First();
                shipment.IsActive = false;
                shipment.DateUpdated = DateTime.Now;
                shipment.DateDeleted = DateTime.Now;
            }

            if (order.DeliveryMethodOrders.Any())
            {
                var delivery = order.DeliveryMethodOrders.First();
                delivery.IsActive = false;
                delivery.DateDeleted = DateTime.Now;
                delivery.DateUpdated = DateTime.Now;
            }

            if (order.Payments.Any())
            {
                var payment = order.Payments.First();
                payment.IsActive = false;
                payment.DateDeleted = DateTime.Now;
                payment.DateUpdated = DateTime.Now;
            }

            foreach (var item in order.OrderItems)
            {
                item.IsActive = false;
            }

            order.DateUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
        }


        public async Task AddNewOrderAsync(OrderViewModel data)
        {

            var order = new Order
            {
                Code = data.Code ?? Guid.NewGuid().ToString(),
                CustomerId = data.CustomerId ?? 0,
                OrderDate = DateTime.Now,
                IsActive = true,
                OrderItems = new List<OrderItem>()
            };

            order = await new ShoppingCart(_context, _httpContextAccessor.HttpContext.Session)
                                                    .ConvertToOrder(order);

            order.Status = OrderProcessingFuncs.GetStringValue(data.OrderStatus);

            order.DateCreated = DateTime.Now;

            var deliveryMethodId = _context.DeliveryMethods.First(
                m => m.MethodName == OrderProcessingFuncs.GetStringValue(
                                            data.DeliveryMethod)).Id;

            order.DeliveryMethodOrders.Add(new DeliveryMethodOrder
            {
                OrderId = order.Id,
                DeliveryMethodId = deliveryMethodId,
                IsActive = true,
                DateCreated = DateTime.Now
            });

            Address newAddress = new Address
            {
                Street = data.Customer?.Street,
                FlatNumber = data.Customer?.FlatNumber,
                HouseNumber = data.Customer?.HouseNumber,
                State = data.Customer?.State,
                PostalCode = data.Customer?.PostalCode,
                City = data.Customer?.City,
                CountryId = _context.Countries.FirstOrDefault(
                                        c => c.CountryName == data.Customer.CountryName)?.Id,
                CustomerId = data.CustomerId ?? 0,
                DateCreated = DateTime.Now,
                IsActive = true,
            };

            var oldAddress = _context.Customers
                                        .Include(c => c.Addresses)
                                            .ThenInclude(a => a.Country)
                                        .FirstOrDefault(
                                                c => c.Id == data.CustomerId)
                                        ?.Addresses
                                        .FirstOrDefault();

            if (oldAddress != null)
            {
                oldAddress.Street = newAddress.Street ?? oldAddress.Street;
                oldAddress.FlatNumber = newAddress.FlatNumber ?? oldAddress.FlatNumber;
                oldAddress.HouseNumber = newAddress.HouseNumber ?? oldAddress.HouseNumber;
                oldAddress.State = newAddress.State ?? oldAddress.State;
                oldAddress.PostalCode = newAddress.PostalCode ?? oldAddress.PostalCode;
                oldAddress.City = newAddress.City ?? oldAddress.City;
                oldAddress.CountryId = newAddress.CountryId ?? oldAddress.CountryId;
                oldAddress.DateUpdated = DateTime.Now;
                oldAddress.IsActive = newAddress.IsActive != oldAddress.IsActive
                                            ? newAddress.IsActive
                                            : oldAddress.IsActive;
            }
            else
            {
                _context.Addresses.Add(newAddress);
            }


            var estimatedArrivalDateShippment = order.OrderDate;

            switch (data.DeliveryMethod)
            {
                case DeliveryMethods.Delivery:
                    estimatedArrivalDateShippment = estimatedArrivalDateShippment.AddDays(2);
                    break;
                case DeliveryMethods.Courier:
                    estimatedArrivalDateShippment = estimatedArrivalDateShippment.AddDays(2.5);
                    break;
                case DeliveryMethods.ParcelLocker:
                    estimatedArrivalDateShippment = estimatedArrivalDateShippment.AddDays(1.5);
                    break;
                case DeliveryMethods.TakeAway:
                    estimatedArrivalDateShippment = estimatedArrivalDateShippment.AddDays(1);
                    break;
            }

            order.Shipments.Add(new Shipment
            {
                DeliveryMethodId = deliveryMethodId,
                ShipmentDate = order.OrderDate.AddDays(2),
                EstimatedArrivalDate = estimatedArrivalDateShippment,
                OrderId = order.Id,
                IsActive = true,
                DateCreated = DateTime.Now
            });

            order.Payments.Add(new Payment
            {
                Amount = order.TotalAmount,
                OrderId = order.Id,
                IsActive = true,
                PaymentDate = DateTime.Now,
                PaymentMethodId = _context.PaymentMethods.First(m =>
                                       m.PaymentType == OrderProcessingFuncs.GetStringValue(
                                                                     data.PaymentMethod)).Id,
                DateCreated = DateTime.Now,
            });

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            await InvoicePaymentHelperFuncs.GenerateInvoice(order, _context);
        }

        public async Task<IEnumerable<OrderViewModel>> GetOrdersAsync()
        {
            var orders = await _context.Orders
                             .Include(o => o.Customer)
                                 .ThenInclude(c => c.Addresses)
                                    .ThenInclude(a => a.Country)
                             .Include(o => o.Customer)
                                 .ThenInclude(c => c.User)
                             .Include(o => o.Customer)
                                 .ThenInclude(o => o.Invoices)
                             .Include(o => o.Shipments)
                                 .ThenInclude(s => s.DeliveryMethod)
                                     .ThenInclude(dm => dm.DeliveryMethodOrders)
                             .Include(o => o.Payments)
                             .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Product)
                                     .ThenInclude(p => p.ProductCategories)
                             .ToListAsync();

            var orderVMs = orders.Select(o => OrderViewModel.OrderToVm(o, _context, _userManager)).ToList();
            return orderVMs;

        }
    }
}

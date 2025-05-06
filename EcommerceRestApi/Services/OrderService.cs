using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class OrderService : EntityBaseRepository<Order>, IOrderService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShoppingCart _cart;

        public OrderService(AppDbContext context,
                            UserManager<ApplicationUser> userManager,
                            ShoppingCart cart) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _cart = cart;
        }

        public async Task<OrderDto?> GetOrderByCodeAsync(string code)
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

            return OrderDto.OrderToDto(order, _context, _userManager);
        }

        public async Task UpdateOrderAsync(string code, NewOrderViewModel data)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Code == code);
            if (order == null)
                throw new KeyNotFoundException($"Order with code '{code}' not found.");

            //order.TotalAmount = data.TotalAmount ?? decimal.Zero;
            order.Status = OrderProcessingFuncs.GetStringValue((OrderStatuses)data.OrderStatus);
            //order.CustomerId = data.CustomerId ?? 0;
            order.DateUpdated = DateTime.Now;


            order.DeliveryMethodOrders.First().DeliveryMethodId = _context.DeliveryMethods.First(
                            m => m.MethodName == OrderProcessingFuncs.GetStringValue((DeliveryMethods)data.DeliveryMethod)).Id;

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

            var pointsForOrder = (int)(order.TotalAmount * AppConstants.POINTS_PER_DOLLAR);
            order.Customer.Points -= pointsForOrder;
            order.Customer.DateUpdated = DateTime.Now;

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


        public async Task<OrderDto> AddNewOrderAsync(NewOrderViewModel data)
        {
            var customer = await _context.Customers
                                    .Include(c => c.Addresses)
                                        .ThenInclude(a => a.Country)
                                    .Include(c => c.User)
                                    .FirstOrDefaultAsync(customer =>
                                            customer.Id == data.Customer.CustomerId);

            var order = new Order
            {
                Code = Guid.NewGuid().ToString(),
                CustomerId = data.Customer?.CustomerId ?? 0,
                OrderDate = DateTime.Now,
                IsActive = true,
                OrderItems = new List<OrderItem>()
            };

            order.TotalAmount = data.TotalAmount;
            order = await _cart.ConvertToOrder(order);

            if (customer != null)
            {
                customer.Nip = data.Customer?.Nip ?? customer.Nip;

                var pointsForOrder = (int)(order.TotalAmount * AppConstants.POINTS_PER_DOLLAR);
                customer.Points += pointsForOrder;
                customer.DateUpdated = DateTime.Now;
            }

            order.Status = OrderProcessingFuncs.GetStringValue((OrderStatuses)data.OrderStatus);

            order.DateCreated = DateTime.Now;

            var deliveryMethodId = _context.DeliveryMethods.First(
                m => m.MethodName == OrderProcessingFuncs.GetStringValue(
                                           (DeliveryMethods)data.DeliveryMethod)).Id;

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
                CustomerId = data.Customer?.CustomerId ?? 0,
                DateCreated = DateTime.Now,
                IsActive = true,
            };

            var oldAddress = _context.Customers
                                        .Include(c => c.Addresses)
                                            .ThenInclude(a => a.Country)
                                        .FirstOrDefault(
                                                c => c.Id == data.Customer.CustomerId)
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

            switch ((DeliveryMethods)data.DeliveryMethod)
            {
                case DeliveryMethods.StandardDelivery:
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


            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            var invoice = await InvoicePaymentHelperFuncs.GenerateInvoice(order, _context);
            await OrderProcessingFuncs.CreateShippment(order.Id,
                                                        estimatedArrivalDateShippment,
                                                        deliveryMethodId,
                                                        _context);

            var paymentMethod = await _context.PaymentMethods
                                                        .FirstOrDefaultAsync(pm =>
                                               pm.PaymentType == OrderProcessingFuncs.GetStringValue(
                                                   (PaymentMethods)data.PaymentMethod));
            if (paymentMethod != null)
            {
                await InvoicePaymentHelperFuncs.CreatePayment(order,
                                                              paymentMethod.Id,
                                                              invoice.Id,
                                                              _context);
            }

            await _cart.ClearCart(); // Clear cart after order submit
            return OrderDto.OrderToDto(order, _context, _userManager);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
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

            var orderVMs = orders.Select(o => OrderDto.OrderToDto(o, _context, _userManager)).ToList();
            return orderVMs;

        }
    }
}

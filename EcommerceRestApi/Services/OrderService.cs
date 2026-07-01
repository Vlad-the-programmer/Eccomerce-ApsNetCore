using EcommerceRestApi.AppGlobals;
using EcommerceRestApi.Helpers.Cart;
using EcommerceRestApi.Helpers.Data.Functions;
using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using EcommerceRestApi.Services.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceRestApi.Services
{
    public class OrderService : EntityBaseRepository<Order>, IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShoppingCart _cart;
        private readonly ICouponService _couponService;
        private readonly IShopCoinsService _coinsService;
        private readonly INotificationService _notificationService;

        public OrderService(AppDbContext context,
                            IHttpContextAccessor httpContextAccessor,
                            UserManager<ApplicationUser> userManager,
                            ShoppingCart cart,
                            ICouponService couponService,
                            IShopCoinsService coinsService,
                            INotificationService notificationService) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _cart = cart;
            _couponService = couponService;
            _coinsService = coinsService;
            _notificationService = notificationService;
        }

        private ISession Session => _httpContextAccessor.HttpContext?.Session;

        public async Task<OrderDto?> GetOrderByCodeAsync(string code)
        {
            var order = await _context.Orders
           .Select(order => new OrderDto
           {
               Code = order.Code,
               CustomerId = order.CustomerId,
               OrderDate = order.OrderDate,
               TotalAmount = order.TotalAmount,
               OrderStatus = order.Status,
               PaymentMethod = order.Payments.FirstOrDefault().PaymentMethod.PaymentType ?? string.Empty,
               DeliveryMethod = order.DeliveryMethodOrders.FirstOrDefault().DeliveryMethod.MethodName ?? string.Empty,
               OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
               {
                   Id = oi.Id,
                   ProductId = oi.ProductId,
                   Quantity = oi.Quantity,
                   UnitPrice = oi.Product.Price,
                   OrderId = oi.OrderId,
                   ProductName = oi.Product.Name,
                   ProductBrand = oi.Product.Brand,
                   ProductPhoto = oi.Product.Photo ?? string.Empty
               }).ToList(),
               Customer = new OrderCustomerDTO
               {
                   FlatNumber = order.Customer.Addresses.FirstOrDefault().FlatNumber ?? string.Empty,
                   HouseNumber = order.Customer.Addresses.FirstOrDefault().HouseNumber ?? string.Empty,
                   City = order.Customer.Addresses.FirstOrDefault().City ?? string.Empty,
                   PostalCode = order.Customer.Addresses.FirstOrDefault().PostalCode ?? string.Empty,
                   State = order.Customer.Addresses.FirstOrDefault().State ?? string.Empty,
                   Street = order.Customer.Addresses.FirstOrDefault().Street ?? string.Empty,
                   CountryName = order.Customer.Addresses.FirstOrDefault().Country.CountryName ?? string.Empty,
                   Email = order.Customer.User.Email ?? string.Empty,
                   FirstName = order.Customer.User.FirstName ?? string.Empty,
                   LastName = order.Customer.User.LastName ?? string.Empty,
                   PhoneNumber = order.Customer.User.PhoneNumber ?? string.Empty,
                   Nip = order.Customer.Nip ?? string.Empty,
                   IsActive = order.Customer.IsActive,
                   IsAdmin = order.Customer.User.IsAdmin,
                   IsAuthenticated = order.Customer.User.IsAuthenticated,
                   Role = order.Customer.User.Role,
                   UserName = order.Customer.User.UserName,
               },
               IsPaid = order.IsPaid,
               StatusHistory = order.StatusHistory.Select(s => new OrderStatusHistoryDto
               {
                   Status = s.Status,
                   DateCreated = s.DateCreated
               }).ToList()
           })
           .FirstOrDefaultAsync(o => o.Code == code);

            if (order == null)
            {
                return null;
            }

            return order;
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

            if (order.Customer != null)
            {
                await _coinsService.RefundCoinsForOrder(order.Id);
            }
        }

        public async Task<OrderDto> AddNewOrderAsync(NewOrderViewModel data)
        {
            if (data.SelectedItemsIds == null || !data.SelectedItemsIds.Any())
            {
                throw new ArgumentException("No items selected for the order.");
            }

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
            order = await _cart.ConvertToOrder(order, data.SelectedItemsIds);

            var appliedCouponCode = Session.GetString("AppliedCouponCode");

            if (!string.IsNullOrEmpty(appliedCouponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code == appliedCouponCode);
                var tuple = await _couponService.ApplyCouponAsync(appliedCouponCode);
                var couponDiscountAmount = tuple.Discount;

                if (coupon != null && tuple.Success)
                {
                    coupon.UsedCount++;

                    order.OrderCoupons.Add(new OrderCoupon
                    {
                        CouponId = coupon.Id,
                        OrderId = order.Id,
                        DiscountApplied = couponDiscountAmount,
                        AppliedAt = DateTime.Now,
                        IsActive = true,
                        DateCreated = DateTime.Now
                    });
                }

                order.TotalAmount -= couponDiscountAmount;

                Session.Remove("AppliedCouponCode");
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

            var message = $"Your order with code {order.Code} has been submitted with status: {order.Status}.";
            await _notificationService.AddNotificationForCustomerAsync(order.CustomerId, message);


            if (customer != null)
            {
                await _coinsService.SpendCoinsForOrder(order.Id);
            }

            var newOrder = await _context.Orders
                    .Include(o => o.OrderCoupons)
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

            foreach (var item in newOrder.OrderItems)
            {
                item.Product.Stock -= item.Quantity;
            }

            await _context.SaveChangesAsync();

            var invoice = await InvoicePaymentHelperFuncs.GenerateInvoice(
                newOrder,
                _context,
                _notificationService
            );
            await OrderProcessingFuncs.CreateShippment(order.Id,
                                                        estimatedArrivalDateShippment,
                                                        deliveryMethodId,
                                                        _context);

            var paymentMethod = await _context.PaymentMethods
                                                        .FirstOrDefaultAsync(pm =>
                                               pm.PaymentType == OrderProcessingFuncs.GetStringValue(
                                                   (PaymentMethods)data.PaymentMethod));
            if (paymentMethod != null && invoice != null)
            {
                await InvoicePaymentHelperFuncs.CreatePayment(newOrder,
                                                              paymentMethod.Id,
                                                              invoice.Id,
                                                              _context,
                                                              _notificationService);
            }

            if (customer != null)
            {
                await _coinsService.RewardCoinsForOrder(order.Id);
            }

            await _cart.ClearCart(data.SelectedItemsIds); // Clear cart items baught after order submit

            return OrderDto.OrderToDto(order, _context, _userManager);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var orders = await _context.Orders
           .Select(order => new OrderDto
           {
               Code = order.Code,
               CustomerId = order.CustomerId,
               OrderDate = order.OrderDate,
               TotalAmount = order.TotalAmount,
               OrderStatus = order.Status,
               PaymentMethod = order.Payments.FirstOrDefault().PaymentMethod.PaymentType ?? string.Empty,
               DeliveryMethod = order.DeliveryMethodOrders.FirstOrDefault().DeliveryMethod.MethodName ?? string.Empty,
               OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
               {
                   ProductId = oi.ProductId,
                   Quantity = oi.Quantity,
                   UnitPrice = oi.Product.Price,
                   OrderId = oi.OrderId,
                   ProductName = oi.Product.Name,
                   ProductBrand = oi.Product.Brand,
               }).ToList(),
               Customer = new OrderCustomerDTO
               {
                   FlatNumber = order.Customer.Addresses.FirstOrDefault().FlatNumber ?? string.Empty,
                   HouseNumber = order.Customer.Addresses.FirstOrDefault().HouseNumber ?? string.Empty,
                   City = order.Customer.Addresses.FirstOrDefault().City ?? string.Empty,
                   PostalCode = order.Customer.Addresses.FirstOrDefault().PostalCode ?? string.Empty,
                   State = order.Customer.Addresses.FirstOrDefault().State ?? string.Empty,
                   Street = order.Customer.Addresses.FirstOrDefault().Street ?? string.Empty,
                   CountryName = order.Customer.Addresses.FirstOrDefault().Country.CountryName ?? string.Empty,
                   Email = order.Customer.User.Email ?? string.Empty,
                   FirstName = order.Customer.User.FirstName ?? string.Empty,
                   LastName = order.Customer.User.LastName ?? string.Empty,
                   PhoneNumber = order.Customer.User.PhoneNumber ?? string.Empty,
                   Nip = order.Customer.Nip ?? string.Empty,
                   IsActive = order.Customer.IsActive,
                   IsAdmin = order.Customer.User.IsAdmin,
                   IsAuthenticated = order.Customer.User.IsAuthenticated,
                   Role = order.Customer.User.Role,
                   UserName = order.Customer.User.UserName,
               },
               IsPaid = order.IsPaid
           })
           .OrderByDescending(o => o.OrderDate)
           .ToListAsync();

            return orders;
        }

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(int customerId)
        {
            var orders = await _context.Orders
            .Where(o => o.CustomerId == customerId)
           .Select(order => new OrderDto
           {
               Code = order.Code,
               CustomerId = order.CustomerId,
               OrderDate = order.OrderDate,
               TotalAmount = order.TotalAmount,
               OrderStatus = order.Status,
               PaymentMethod = order.Payments.FirstOrDefault().PaymentMethod.PaymentType ?? string.Empty,
               DeliveryMethod = order.DeliveryMethodOrders.FirstOrDefault().DeliveryMethod.MethodName ?? string.Empty,
               OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
               {
                   ProductId = oi.ProductId,
                   Quantity = oi.Quantity,
                   UnitPrice = oi.Product.Price,
                   OrderId = oi.OrderId,
                   ProductName = oi.Product.Name,
                   ProductBrand = oi.Product.Brand,
               }).ToList(),
               Customer = new OrderCustomerDTO
               {
                   FlatNumber = order.Customer.Addresses.FirstOrDefault().FlatNumber ?? string.Empty,
                   HouseNumber = order.Customer.Addresses.FirstOrDefault().HouseNumber ?? string.Empty,
                   City = order.Customer.Addresses.FirstOrDefault().City ?? string.Empty,
                   PostalCode = order.Customer.Addresses.FirstOrDefault().PostalCode ?? string.Empty,
                   State = order.Customer.Addresses.FirstOrDefault().State ?? string.Empty,
                   Street = order.Customer.Addresses.FirstOrDefault().Street ?? string.Empty,
                   CountryName = order.Customer.Addresses.FirstOrDefault().Country.CountryName ?? string.Empty,
                   Email = order.Customer.User.Email ?? string.Empty,
                   FirstName = order.Customer.User.FirstName ?? string.Empty,
                   LastName = order.Customer.User.LastName ?? string.Empty,
                   PhoneNumber = order.Customer.User.PhoneNumber ?? string.Empty,
                   Nip = order.Customer.Nip ?? string.Empty,
                   IsActive = order.Customer.IsActive,
                   IsAdmin = order.Customer.User.IsAdmin,
                   IsAuthenticated = order.Customer.User.IsAuthenticated,
                   Role = order.Customer.User.Role,
                   UserName = order.Customer.User.UserName,
               },
               IsPaid = order.IsPaid,
               StatusHistory = order.StatusHistory.Select(s => new OrderStatusHistoryDto
               {
                   Status = s.Status,
                   DateCreated = s.DateCreated
               }).ToList()
           })
           .OrderByDescending(o => o.OrderDate)
           .ToListAsync();

            return orders;
        }

        public async Task ChangeOrderStatusAsync(ChangeOrderStatusDto dto, string? currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId)) return;

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Code == dto.OrderCode);

            if (order == null)
                throw new Exception("Order not found");

            string nextStatus = order.Status switch
            {
                "Pending" => "Processing",
                "Paid" => "Processing",
                "Processing" => "ReadyToShip",
                "ReadyToShip" => "Shipped",
                "Shipped" => "InDelivery",
                "InDelivery" => "Delivered",
                _ => null
            };

            if (dto.Status == "Cancelled")
            {
                order.Status = "Cancelled";
            }
            else if (nextStatus == null || dto.Status != nextStatus)
            {
                throw new Exception($"Invalid status transition from {order.Status} to {dto.Status}");
            }
            else
            {
                order.Status = dto.Status;
            }

            if (dto.Status == "Delivered")
            {
                order.IsPaid = true;
            }

            await _context.SaveChangesAsync();

            await _context.OrderStatusHistory.AddAsync(new OrderStatusHistory()
            {
                Status = dto.Status,
                ChangedBy = currentUserId,
                DateCreated = DateTime.Now,
                IsActive = true,
                OrderId = order.Id
            });

            await _context.SaveChangesAsync();

            var message = $"Your order with code {order.Code} has been updated to status: {order.Status}.";
            await _notificationService.AddNotificationForCustomerAsync(order.CustomerId, message);
        }

        public async Task<List<OrderDto>> FilterOrdersAsync(
           string searchString, string? searchProperty, string? sortProperty, DateTime? fromDate, DateTime? toDate, bool sortAscending = false)
        {
            var allOrders = _context.Orders.Select(order => new OrderDto
            {
                Code = order.Code,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.Status,
                PaymentMethod = order.Payments.FirstOrDefault().PaymentMethod.PaymentType,
                DeliveryMethod = order.DeliveryMethodOrders.FirstOrDefault().DeliveryMethod.MethodName ?? string.Empty,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.Product.Price,
                    OrderId = oi.OrderId,
                    ProductName = oi.Product.Name,
                    ProductBrand = oi.Product.Brand,
                }).ToList(),
                Customer = new OrderCustomerDTO
                {
                    FlatNumber = order.Customer.Addresses.FirstOrDefault().FlatNumber ?? string.Empty,
                    HouseNumber = order.Customer.Addresses.FirstOrDefault().HouseNumber ?? string.Empty,
                    City = order.Customer.Addresses.FirstOrDefault().City ?? string.Empty,
                    PostalCode = order.Customer.Addresses.FirstOrDefault().PostalCode ?? string.Empty,
                    State = order.Customer.Addresses.FirstOrDefault().State ?? string.Empty,
                    Street = order.Customer.Addresses.FirstOrDefault().Street ?? string.Empty,
                    CountryName = order.Customer.Addresses.FirstOrDefault().Country.CountryName ?? string.Empty,
                    Email = order.Customer.User.Email ?? string.Empty,
                    FirstName = order.Customer.User.FirstName ?? string.Empty,
                    LastName = order.Customer.User.LastName ?? string.Empty,
                    PhoneNumber = order.Customer.User.PhoneNumber ?? string.Empty,
                    Nip = order.Customer.Nip ?? string.Empty,
                    IsActive = order.Customer.IsActive,
                    IsAdmin = order.Customer.User.IsAdmin,
                    IsAuthenticated = order.Customer.User.IsAuthenticated,
                    Role = order.Customer.User.Role,
                    UserName = order.Customer.User.UserName,
                },
                IsPaid = order.IsPaid
            }).AsQueryable();

            var filteredResult = allOrders.Where(o => !string.IsNullOrEmpty(o.Customer.FirstName) && !string.IsNullOrEmpty(o.Customer.FirstName));

            if (!string.IsNullOrEmpty(searchString))
            {

                switch (searchProperty)
                {
                    case nameof(Order.Status):
                        filteredResult = filteredResult.Where(item => item.OrderStatus.ToLower().Contains(searchString.ToLower()));
                        break;
                    case nameof(Order.Code):
                        filteredResult = filteredResult.Where(item => item.Code.Contains(searchString));
                        break;
                    case "FirstName":
                        filteredResult = filteredResult.Where(item => item.Customer.FirstName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "LastName":
                        filteredResult = filteredResult.Where(item => item.Customer.LastName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "FullName":
                        filteredResult = filteredResult.Where(item =>
                            item.Customer.FirstName.ToLower().Contains(searchString.ToLower()) ||
                            item.Customer.LastName.ToLower().Contains(searchString.ToLower()) ||
                            (item.Customer.FirstName.ToLower() + " " + item.Customer.LastName.ToLower()).Contains(searchString.ToLower())
                        );
                        break;
                }

                if (searchProperty == null)
                {
                    filteredResult = filteredResult.Where(n =>
                                n.Code.ToLower().Contains(searchString.ToLower()) ||
                                n.Customer.FirstName.ToLower().Contains(searchString.ToLower()) ||
                                n.Customer.LastName.ToLower().Contains(searchString.ToLower()) ||
                                (n.Customer.FirstName.ToLower() + " " + n.Customer.LastName.ToLower()).Contains(searchString.ToLower())
                        )
                        .AsQueryable();
                }
            }

            if (fromDate.HasValue)
            {
                filteredResult = filteredResult.Where(o => o.OrderDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                filteredResult = filteredResult.Where(o => o.OrderDate.Date <= toDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(sortProperty))
            {
                switch (sortProperty)
                {
                    case nameof(Order.Status):
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.OrderStatus) : filteredResult.OrderByDescending(item => item.OrderStatus);
                        break;
                    case "FirstName":
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.Customer.FirstName) : filteredResult.OrderByDescending(item => item.Customer.FirstName);
                        break;
                    case nameof(Order.Code):
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.Code) : filteredResult.OrderByDescending(item => item.Code);
                        break;
                    case "LastName":
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => item.Customer.LastName) : filteredResult.OrderByDescending(item => item.Customer.LastName);
                        break;
                    case "FullName":
                        filteredResult = sortAscending ? filteredResult.OrderBy(item => $"{item.Customer.FirstName} {item.Customer.LastName}".ToLower()) : filteredResult.OrderByDescending(item => $"{item.Customer.FirstName} {item.Customer.LastName}".ToLower());
                        break;
                    case "OrderDate":
                        filteredResult = sortAscending ? filteredResult.OrderBy(o => o.OrderDate) : filteredResult.OrderByDescending(o => o.OrderDate);
                        break;
                }
            }
            else
            {
                filteredResult = filteredResult.OrderByDescending(o => o.OrderDate);
            }

            return filteredResult.Select(order => new OrderDto
            {
                Code = order.Code,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentMethod = order.PaymentMethod,
                DeliveryMethod = order.DeliveryMethod ?? string.Empty,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    OrderId = oi.OrderId,
                    ProductName = oi.ProductName,
                    ProductBrand = oi.ProductBrand,
                }).ToList(),
                Customer = new OrderCustomerDTO
                {
                    FlatNumber = order.Customer.FlatNumber ?? string.Empty,
                    HouseNumber = order.Customer.HouseNumber ?? string.Empty,
                    City = order.Customer.City ?? string.Empty,
                    PostalCode = order.Customer.PostalCode ?? string.Empty,
                    State = order.Customer.State ?? string.Empty,
                    Street = order.Customer.Street ?? string.Empty,
                    CountryName = order.Customer.CountryName ?? string.Empty,
                    Email = order.Customer.Email ?? string.Empty,
                    FirstName = order.Customer.FirstName ?? string.Empty,
                    LastName = order.Customer.LastName ?? string.Empty,
                    PhoneNumber = order.Customer.PhoneNumber ?? string.Empty,
                    Nip = order.Customer.Nip ?? string.Empty,
                    IsActive = order.Customer.IsActive,
                    IsAdmin = order.Customer.IsAdmin,
                    IsAuthenticated = order.Customer.IsAuthenticated,
                    Role = order.Customer.Role,
                    UserName = order.Customer.UserName,
                },
                IsPaid = order.IsPaid
            }).ToList();
        }

        public List<SearchComboBoxDto> GetSearchComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Order.Status),
                    DisplayName = "Status"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(ApplicationUser.LastName),
                    DisplayName = "Last Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(ApplicationUser.FirstName),
                    DisplayName = "FirstName"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Order.Code),
                    DisplayName = "Code"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "FullName",
                    DisplayName = "Full Name"
                }
            };
        }

        public List<SearchComboBoxDto> GetOrderByComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Order.Status),
                    DisplayName = "Status"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(ApplicationUser.LastName),
                    DisplayName = "Last Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(ApplicationUser.FirstName),
                    DisplayName = "First Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Order.Code),
                    DisplayName = "Code"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = "FullName",
                    DisplayName = "Full Name"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Order.OrderDate),
                    DisplayName = "Order Date"
                }
            };
        }

        public async Task<NewOrderViewModel> CreateOrderCreateTemplate(string shoppingCartId, ClaimsPrincipal User)
        {
            var model = new NewOrderViewModel();
            model.Customer = new CreateOrderCustomerDto();

            var customer = await _context.Customers
                                            .Include(c => c.User)
                                            .Include(c => c.Addresses)
                                                .ThenInclude(a => a.Country)
                                            .Where(c => c.UserId == _userManager.GetUserId(User))
                                            .FirstOrDefaultAsync();
            if (customer != null)
            {
                model.Customer = CreateOrderCustomerDto.ToDto(customer, _userManager);
            }

            model.TotalAmount += await _cart.GetTotal();

            model.TaxRate = AppConstants.TAXES_RATE;

            return model;
        }
    }
}

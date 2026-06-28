using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EcommerceRestApi.Services
{
    public class RefundService : IRefundService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RefundService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IReturnService _returnService;

        public RefundService(AppDbContext context,
            ILogger<RefundService> logger,
            INotificationService notificationService,
            IReturnService returnService)
        {
            _context = context;
            _logger = logger;
            _notificationService = notificationService;
            _returnService = returnService;
        }

        public async Task ApplyForRefund(string orderCode, CreateRefundDto dto, string? userId)
        {
            if (string.IsNullOrEmpty(orderCode))
                throw new ArgumentException("Order code is required");

            if (dto.OrderItems == null || dto.OrderItems.Count <= 0)
                throw new ArgumentException("At least one order item must be selected for refund");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Code == orderCode && o.IsActive);

                if (order == null)
                    throw new Exception($"Order with code {orderCode} not found");

                if (order.Status != OrderStatuses.Delivered.ToString())
                    throw new Exception($"Order must be delivered to request a refund. Current status: {order.Status}");

                if (!order.IsPaid)
                    throw new Exception("Order must be paid to request a refund");

                var requestedItemIds = dto.OrderItems.Select(oi => oi.OrderItemId).ToList();
                var orderItemIds = order.OrderItems.Select(oi => oi.Id).ToList();

                var invalidItems = requestedItemIds.Where(id => !orderItemIds.Contains(id)).ToList();
                if (invalidItems.Any())
                    throw new Exception($"Invalid order items: {string.Join(", ", invalidItems)}");

                var orderItems = await _context.OrderItems
                    .Include(oi => oi.Product)
                    .Where(oi => requestedItemIds.Contains(oi.Id))
                    .ToListAsync();

                var existingRefunds = await _context.RefundItem
                    .Where(ri => requestedItemIds.Contains(ri.OrderItemId) && ri.Refund.IsActive)
                    .Select(ri => ri.OrderItemId)
                    .ToListAsync();

                if (existingRefunds.Any())
                    throw new Exception($"Items already have pending/processed refunds: {string.Join(", ", existingRefunds)}");

                var refund = new Refund
                {
                    Code = GenerateRefundCode(),
                    CustomerId = order.CustomerId,
                    Amount = 0, // Will be calculated
                    Status = RefundStatuses.Pending,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Refund.Add(refund);
                await _context.SaveChangesAsync();

                decimal totalRefundAmount = 0;

                foreach (var requestedItem in dto.OrderItems)
                {
                    var orderItem = orderItems.FirstOrDefault(oi => oi.Id == requestedItem.OrderItemId);
                    if (orderItem == null)
                        continue;

                    if (requestedItem.Quantity <= 0)
                        throw new Exception($"Invalid quantity for item {orderItem.Product.Name}. Quantity must be greater than 0");

                    if (requestedItem.Quantity > orderItem.Quantity)
                        throw new Exception($"Requested quantity ({requestedItem.Quantity}) exceeds available quantity ({orderItem.Quantity}) for {orderItem.Product.Name}");

                    var refundAmount = requestedItem.Quantity * orderItem.UnitPrice;

                    var refundItem = new RefundItem
                    {
                        RefundId = refund.Id,
                        OrderItemId = orderItem.Id,
                        Quantity = requestedItem.Quantity,
                        RefundAmount = refundAmount,
                        Reason = !string.IsNullOrEmpty(requestedItem.Reason) ? requestedItem.Reason : "Customer requested refund",
                        DateCreated = DateTime.UtcNow,
                        IsActive = true
                    };

                    _context.RefundItem.Add(refundItem);
                    totalRefundAmount += refundAmount;
                }

                refund.Amount = totalRefundAmount;
                await _context.SaveChangesAsync();

                var message = $"Your refund {refund.Code} has been submitted for {dto.OrderItems.Count} item(s) totaling {totalRefundAmount.ToString("C")}.";
                await _notificationService.AddNotificationForCustomerAsync(refund.CustomerId, message);

                await _context.RefundStatusHistory.AddAsync(new RefundStatusHistory()
                {
                    Status = refund.Status,
                    RefundId = refund.Id,
                    RefundCode = refund.Code,
                    IsActive = true,
                    DateCreated = DateTime.Now,
                    ChangedBy = userId ?? string.Empty
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation($"Refund {refund.Code} created for order {orderCode} with {dto.OrderItems.Count} items totaling {totalRefundAmount}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error creating refund for order {orderCode}");
                throw;
            }
        }

        public async Task CancelRefund(string code, bool currentUserIsStuffOrAdmin, string? userId)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Refund code is required");

            var refund = await _context.Refund
                .Include(r => r.RefundItems)
                .FirstOrDefaultAsync(r => r.Code == code && r.IsActive);

            if (refund == null)
                throw new Exception($"Refund with code {code} not found");

            if (refund.Status == RefundStatuses.Completed || refund.Status == RefundStatuses.Processed)
                throw new Exception($"Cannot cancel refund with status: {refund.Status}");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (currentUserIsStuffOrAdmin)
                {
                    refund.Status = RefundStatuses.Rejected;
                }
                else
                {
                    refund.Status = RefundStatuses.Cancelled;
                }
                refund.IsActive = false;
                refund.DateDeleted = DateTime.UtcNow;

                foreach (var item in refund.RefundItems)
                {
                    item.IsActive = false;
                    item.DateDeleted = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                var message = $"Your refund {refund.Code} has been cancelled.";
                await _notificationService.AddNotificationForCustomerAsync(refund.CustomerId, message);

                await _context.RefundStatusHistory.AddAsync(new RefundStatusHistory()
                {
                    Status = refund.Status,
                    RefundId = refund.Id,
                    RefundCode = refund.Code,
                    IsActive = true,
                    DateCreated = DateTime.Now,
                    ChangedBy = userId ?? string.Empty
                });

                await _context.SaveChangesAsync();

                await _returnService.CancelReturn(refund.Code);

                await transaction.CommitAsync();


                _logger.LogInformation($"Refund {code} has been cancelled");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error cancelling refund {code}");
                throw;
            }
        }

        public async Task ChangeRefundStatus(ChangeRefundStatusDto dto, string changedBy)
        {
            if (dto == null)
                throw new ArgumentException("ChangeRefundStatusDto is required");

            if (string.IsNullOrEmpty(dto.RefundCode))
                throw new ArgumentException("Refund code is required");

            if (string.IsNullOrEmpty(dto.Status))
                throw new ArgumentException("Status is required");

            var refund = await _context.Refund
                .Include(r => r.RefundItems)
                .ThenInclude(ri => ri.OrderItem)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(r => r.Code == dto.RefundCode && r.IsActive);

            if (refund == null)
                throw new Exception($"Refund with code {dto.RefundCode} not found");

            if (!IsValidStatusTransition(refund.Status, dto.Status))
                throw new Exception($"Invalid status transition from {refund.Status} to {dto.Status}");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                refund.Status = dto.Status;
                refund.ProcessedDate = DateTime.UtcNow;

                if (dto.Status == RefundStatuses.Processed || dto.Status == RefundStatuses.Completed)
                {
                    await CreatePaymentForRefund(refund.Code);

                    foreach (var item in refund.RefundItems)
                    {
                        item.IsActive = false;
                        item.DateDeleted = DateTime.UtcNow;
                    }

                    await _returnService.ChangeReturnStatus(new ChangeReturnStatusDto
                    {
                        RefundCode = refund.Code,
                        Status = ReturnStatuses.Approved.ToString()
                    });
                }

                if (dto.Status == RefundStatuses.Rejected)
                {
                    foreach (var item in refund.RefundItems)
                    {
                        item.IsActive = false;
                        item.DateDeleted = DateTime.UtcNow;
                    }

                    await _returnService.ChangeReturnStatus(new ChangeReturnStatusDto
                    {
                        RefundCode = refund.Code,
                        Status = ReturnStatuses.Rejected.ToString()
                    });
                }

                await _context.SaveChangesAsync();

                var message = $"Your refund {refund.Code} status has been changed to {dto.Status}.";
                await _notificationService.AddNotificationForCustomerAsync(refund.CustomerId, message);

                await _context.RefundStatusHistory.AddAsync(new RefundStatusHistory()
                {
                    Status = dto.Status,
                    RefundId = refund.Id,
                    RefundCode = refund.Code,
                    IsActive = true,
                    DateCreated = DateTime.Now,
                    ChangedBy = changedBy
                });

                await _context.SaveChangesAsync();

                if (dto.Status == RefundStatuses.Approved)
                    await _returnService.CreateReturn(refund.Code);

                await transaction.CommitAsync();

                _logger.LogInformation($"Refund {refund.Code} status changed to {dto.Status} by {changedBy}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error changing refund {refund.Code} status to {dto.Status}");
                throw;
            }
        }

        public async Task CreatePaymentForRefund(string refundCode)
        {
            if (string.IsNullOrEmpty(refundCode))
                throw new ArgumentException("Refund code is required");

            var refund = await _context.Refund
                .Include(r => r.RefundItems)
                .ThenInclude(ri => ri.OrderItem)
                .ThenInclude(oi => oi.Order)
                .FirstOrDefaultAsync(r => r.Code == refundCode && r.IsActive);

            if (refund == null)
                throw new Exception($"Refund with code {refundCode} not found");

            if (refund.PaymentId.HasValue)
            {
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.Id == refund.PaymentId.Value);

                if (existingPayment != null)
                    throw new Exception($"Payment already exists for refund {refundCode}");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var firstOrderItem = refund.RefundItems.FirstOrDefault()?.OrderItem;
                var orderId = firstOrderItem?.OrderId;

                if (!orderId.HasValue)
                    throw new Exception("Could not determine the original order for the refund");

                var order = await _context.Orders
                    .Include(o => o.Payments)
                    .FirstOrDefaultAsync(o => o.Id == orderId.Value);

                int? paymentMethodId = null;
                if (order?.Payments != null && order.Payments.Any())
                {
                    paymentMethodId = order.Payments.FirstOrDefault()?.PaymentMethodId;
                }

                var payment = new Payment
                {
                    OrderId = orderId,
                    PaymentMethodId = paymentMethodId,
                    Amount = refund.Amount,
                    PaymentDate = DateTime.UtcNow,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                refund.PaymentId = payment.Id;
                await _context.SaveChangesAsync();

                await _returnService.ChangeReturnStatus(new ChangeReturnStatusDto
                {
                    RefundCode = refund.Code,
                    Status = ReturnStatuses.Refunded.ToString()
                });

                var message = $"Payment is on it's way for refund {refundCode} with amount {refund.Amount}.";
                await _notificationService.AddNotificationForCustomerAsync(refund.CustomerId, message);

                await transaction.CommitAsync();

                _logger.LogInformation($"Payment created for refund {refundCode} with amount {refund.Amount}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error creating payment for refund {refundCode}");
                throw;
            }
        }

        public async Task<List<RefundDto>> FilterRefundsAsync(
            string? searchString,
            string? searchProperty,
            string? sortProperty,
            bool active,
            bool sortAscending)
        {
            var query = _context.Refund
                .Include(r => r.Customer)
                    .ThenInclude(r => r.User)
                .Include(r => r.RefundItems)
                    .ThenInclude(ri => ri.OrderItem)
                        .ThenInclude(oi => oi.Product)
                .AsQueryable();

            // =========================
            // ACTIVE FILTER (STATUS LOGIC)
            // =========================
            if (active)
            {
                query = query.Where(r =>
                    r.Status == "Pending" ||
                    r.Status == "Approved");
            }

            // =========================
            // SEARCH
            // =========================
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.ToLower();

                query = searchProperty switch
                {
                    nameof(Refund.Code) =>
                        query.Where(r => r.Code.ToLower().Contains(searchString)),

                    nameof(Refund.Status) =>
                        query.Where(r => r.Status.ToLower().Contains(searchString)),

                    "Customer Fullname" =>
                        query.Where(r => r.Customer.User.FullName.ToLower().Contains(searchString.ToLower())),

                    _ =>
                        query.Where(r =>
                            r.Code.ToLower().Contains(searchString) ||
                            r.Status.ToLower().Contains(searchString))
                };
            }

            // =========================
            // SORTING
            // =========================
            query = sortProperty switch
            {
                nameof(Refund.Code) =>
                    sortAscending
                        ? query.OrderBy(r => r.Code)
                        : query.OrderByDescending(r => r.Code),

                nameof(Refund.Status) =>
                    sortAscending
                        ? query.OrderBy(r => r.Status)
                        : query.OrderByDescending(r => r.Status),

                nameof(Refund.DateCreated) =>
                    sortAscending
                        ? query.OrderBy(r => r.DateCreated)
                        : query.OrderByDescending(r => r.DateCreated),

                _ =>
                    query.OrderByDescending(r => r.DateCreated)
            };

            // =========================
            // PROJECT TO DTO
            // =========================
            return await query
                .Select(r => new RefundDto
                {
                    Code = r.Code,
                    CustomerId = r.CustomerId,
                    PaymentId = r.PaymentId,
                    Amount = r.Amount,
                    Status = r.Status,
                    ProcessedDate = r.ProcessedDate,
                    DateCreated = r.DateCreated,
                    IsActive = r.IsActive,

                    RefundItems = r.RefundItems.Select(ri => new RefundItemDto
                    {
                        Quantity = ri.Quantity,
                        RefundAmount = ri.RefundAmount,
                        Reason = ri.Reason,
                        ProductName = ri.OrderItem.Product.Name,
                        ProductBrand = ri.OrderItem.Product.Brand,
                        ProductPhoto = ri.OrderItem.Product.Photo
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<RefundDto>> GetActiveRefunds(int customerId)
        {

            var refunds = await _context.Refund
                .Where(r => r.CustomerId == customerId)
                .Select(refund => new RefundDto
                {
                    Code = refund.Code,
                    CustomerId = refund.CustomerId,
                    PaymentId = refund.PaymentId,
                    Amount = refund.Amount,
                    Status = refund.Status,
                    ProcessedDate = refund.ProcessedDate,
                    DateCreated = refund.DateCreated,
                    IsActive = refund.IsActive,
                    RefundItems = refund.RefundItems.Select(ri => new RefundItemDto
                    {
                        Quantity = ri.Quantity,
                        RefundAmount = ri.RefundAmount,
                        Reason = ri.Reason,
                        ProductName = ri.OrderItem.Product.Name ?? string.Empty,
                        ProductBrand = ri.OrderItem.Product.Brand ?? string.Empty,
                        ProductPhoto = ri.OrderItem.Product.Photo ?? string.Empty
                    }).ToList(),
                    RefundStatusHistory = refund.RefundStatusHistory.Select(rh => new RefundStatusHistoryDto
                    {
                        Status = rh.Status,
                        RefundCode = rh.RefundCode,
                        DateCreated = rh.DateCreated
                    }).ToList(),
                })
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return refunds;
        }

        public async Task<List<RefundDto>> GetAllRefunds()
        {
            var refunds = await _context.Refund
                .Select(refund => new RefundDto
                {
                    Code = refund.Code,
                    CustomerId = refund.CustomerId,
                    PaymentId = refund.PaymentId,
                    Amount = refund.Amount,
                    Status = refund.Status,
                    ProcessedDate = refund.ProcessedDate,
                    DateCreated = refund.DateCreated,
                    IsActive = refund.IsActive,
                    RefundItems = refund.RefundItems.Select(ri => new RefundItemDto
                    {
                        Quantity = ri.Quantity,
                        RefundAmount = ri.RefundAmount,
                        Reason = ri.Reason,
                        ProductName = ri.OrderItem.Product.Name ?? string.Empty,
                        ProductBrand = ri.OrderItem.Product.Brand ?? string.Empty,
                        ProductPhoto = ri.OrderItem.Product.Photo ?? string.Empty
                    }).ToList()
                })
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return refunds;
        }

        public async Task<List<RefundDto>> GetAllRefundsForOrder(string orderCode)
        {
            var refunds = await _context.Return
                .Where(r => r.Order.Code == orderCode)
                .Select(r => r.Refund)
                .Where(rf => rf != null)
                .Select(refund => new RefundDto
                {
                    Code = refund.Code,
                    CustomerId = refund.CustomerId,
                    PaymentId = refund.PaymentId,
                    Amount = refund.Amount,
                    Status = refund.Status,
                    ProcessedDate = refund.ProcessedDate,
                    DateCreated = refund.DateCreated,
                    IsActive = refund.IsActive,
                    RefundItems = refund.RefundItems.Select(ri => new RefundItemDto
                    {
                        Quantity = ri.Quantity,
                        RefundAmount = ri.RefundAmount,
                        Reason = ri.Reason,
                        ProductName = ri.OrderItem.Product.Name ?? "",
                        ProductBrand = ri.OrderItem.Product.Brand ?? "",
                        ProductPhoto = ri.OrderItem.Product.Photo ?? ""
                    }).ToList()
                })
                .OrderByDescending(r => r.DateCreated)
                .ToListAsync();

            return refunds;
        }

        public async Task<RefundDto> GetRefundByCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentException("Refund code is required");

            var refund = await _context.Refund
                .Include(r => r.RefundItems)
                .ThenInclude(ri => ri.OrderItem)
                .ThenInclude(oi => oi.Product)
                .Include(r => r.Payment)
                .Include(r => r.RefundStatusHistory)
                .FirstOrDefaultAsync(r => r.Code == code);

            if (refund == null)
                throw new Exception($"Refund with code {code} not found");

            return MapToRefundDto(refund);
        }

        public List<SearchComboBoxDto> GetSearchComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                //new SearchComboBoxDto()
                //{
                //    PropertyTitle = "Customer Fullname",
                //    DisplayName = "Customer Fullname"
                //},
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Refund.Status),
                    DisplayName = "Status",
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Refund.Code),
                    DisplayName = "Refund Code"
                },
            };
        }

        public List<SearchComboBoxDto> GetOrderByComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Refund.Status),
                    DisplayName = "Status",
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Refund.Code),
                    DisplayName = "Refund Code"
                },
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Refund.DateCreated),
                    DisplayName = "Date created"
                }
            };
        }


        #region Helper Methods

        private string GenerateRefundCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = $"REF-{DateTime.Now:yyyyMMdd}-{RandomNumberGenerator.GetInt32(10000, 99999)}";

            var exists = _context.Refund.Any(r => r.Code == code);
            if (exists)
                return GenerateRefundCode();

            return code;
        }

        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            var validTransitions = new Dictionary<string, List<string>>
            {
                { RefundStatuses.Pending, new List<string> { RefundStatuses.Approved, RefundStatuses.Rejected } },
                { RefundStatuses.Approved, new List<string> { RefundStatuses.Processed, RefundStatuses.Rejected } },
                { RefundStatuses.Processed, new List<string> { RefundStatuses.Completed, RefundStatuses.Failed } },
                { RefundStatuses.Completed, new List<string>() },
                { RefundStatuses.Rejected, new List<string>() },
                { RefundStatuses.Cancelled, new List<string>() },
                { RefundStatuses.Failed, new List<string>() }
            };

            if (!validTransitions.ContainsKey(currentStatus))
                return false;

            return validTransitions[currentStatus].Contains(newStatus);
        }

        private RefundDto MapToRefundDto(Refund refund)
        {
            var dto = new RefundDto
            {
                Code = refund.Code,
                CustomerId = refund.CustomerId,
                PaymentId = refund.PaymentId,
                Amount = refund.Amount,
                Status = refund.Status,
                ProcessedDate = refund.ProcessedDate,
                DateCreated = refund.DateCreated,
                IsActive = refund.IsActive,
                RefundItems = refund.RefundItems.Select(ri => new RefundItemDto
                {
                    Quantity = ri.Quantity,
                    RefundAmount = ri.RefundAmount,
                    Reason = ri.Reason,
                    ProductName = ri.OrderItem?.Product?.Name ?? string.Empty,
                    ProductBrand = ri.OrderItem?.Product?.Brand ?? string.Empty,
                    ProductPhoto = ri.OrderItem?.Product?.Photo ?? string.Empty
                }).ToList(),
                RefundStatusHistory = refund.RefundStatusHistory.Select(rh => new RefundStatusHistoryDto
                {
                    Status = rh.Status,
                    RefundCode = rh.RefundCode,
                    DateCreated = rh.DateCreated
                }).ToList(),
            };

            return dto;
        }

        #endregion
    }
}
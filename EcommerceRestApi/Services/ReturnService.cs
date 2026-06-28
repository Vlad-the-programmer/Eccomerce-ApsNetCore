using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using Microsoft.EntityFrameworkCore;

namespace EcommerceRestApi.Services
{
    public class ReturnService : IReturnService
    {
        private readonly AppDbContext _context;

        public ReturnService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CancelReturn(string refundCode)
        {
            if (string.IsNullOrEmpty(refundCode))
                throw new ArgumentException("Return code is required");

            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var returnEntity = await _context.Return
                    .Include(r => r.Refund)
                    .FirstOrDefaultAsync(r => r.Refund.Code == refundCode && r.IsActive);

                if (returnEntity == null)
                    throw new Exception("Return not found");

                if (returnEntity.Status == "Delivered")
                    throw new Exception("Cannot cancel delivered return");

                returnEntity.Status = ReturnStatuses.Cancelled.ToString();
                returnEntity.IsActive = false;
                returnEntity.DateUpdated = DateTime.UtcNow;
                returnEntity.DateDeleted = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                //await transaction.CommitAsync();
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task ChangeReturnStatus(ChangeReturnStatusDto dto)
        {
            if (dto == null)
                throw new ArgumentException("DTO is required");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var returnEntity = await _context.Return
                    .Include(r => r.Refund)
                    .FirstOrDefaultAsync(r => r.Refund.Code == dto.RefundCode && r.IsActive);

                if (returnEntity == null)
                    throw new Exception("Return not found");

                if (!IsValidReturnStatusTransition(returnEntity.Status, dto.Status))
                    throw new Exception($"Invalid transition from {returnEntity.Status} to {dto.Status}");

                returnEntity.Status = dto.Status;
                returnEntity.DateUpdated = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CreateReturn(string refundCode)
        {
            if (string.IsNullOrEmpty(refundCode))
                throw new ArgumentException("Refund code is required");

            //using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var refund = await _context.Refund
                    .Include(r => r.RefundItems)
                        .ThenInclude(ri => ri.OrderItem)
                    .FirstOrDefaultAsync(r => r.Code == refundCode && r.IsActive);

                if (refund == null)
                    throw new Exception($"Refund {refundCode} not found");

                if (refund.Status != "Approved" && refund.Status != "Processed")
                    throw new Exception($"Cannot create return for refund with status {refund.Status}");

                var orderId = refund.RefundItems
                    .FirstOrDefault()?.OrderItem?.OrderId;

                if (!orderId.HasValue)
                    throw new Exception("Order not found for refund");

                var existingReturn = await _context.Return
                    .AnyAsync(r => r.RefundId == refund.Id && r.IsActive);

                if (existingReturn)
                    throw new Exception("Return already exists for this refund");

                var returnEntity = new Return
                {
                    RefundId = refund.Id,
                    CustomerId = refund.CustomerId,
                    OrderId = orderId.Value,
                    Reason = "Return created after refund approval",
                    RefundAmount = refund.Amount,
                    Status = RefundStatuses.Pending,
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Return.Add(returnEntity);
                await _context.SaveChangesAsync();

                //await transaction.CommitAsync();
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ReturnDto>> FilterReturnsAsync(
            string? searchString,
            string? searchProperty,
            string? sortProperty,
            bool sortAscending)
        {
            var query = _context.Refund
                .Include(rf => rf.Return)
                .Include(r => r.Customer)
                    .ThenInclude(r => r.User)
                .Include(r => r.RefundItems)
                    .ThenInclude(ri => ri.OrderItem)
                        .ThenInclude(oi => oi.Product)
                .AsQueryable();

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

                    nameof(Return.Status) =>
                        query.Where(r => r.Return.Status.ToLower().Contains(searchString)),

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
                .Select(rf => rf.Return)
                .Select(r => new ReturnDto
                {
                    Reason = r.Reason,
                    RefundCode = r.Refund.Code,
                    RefundAmount = r.RefundAmount,
                    Status = r.Status,
                    IsActive = r.IsActive,
                    DateCreated = r.DateCreated,
                })
                    .ToListAsync();
        }

        public async Task<List<ReturnDto>> GetAllReturnsAsync()
        {
            return await _context.Return
                    .Select(r => new ReturnDto
                    {
                        Reason = r.Reason,
                        RefundCode = r.Refund.Code,
                        RefundAmount = r.RefundAmount,
                        Status = r.Status,
                        IsActive = r.IsActive,
                        DateCreated = r.DateCreated,
                    })
                    .ToListAsync();
        }

        public async Task<List<ReturnDto>> GetCustomerReturnsAsync(int customerId)
        {
            Console.WriteLine($"Customer id {customerId}");

            return await _context.Return
                    .Where(r => r.CustomerId == customerId)
                    .Select(r => new ReturnDto
                    {
                        Reason = r.Reason,
                        RefundCode = r.Refund.Code,
                        RefundAmount = r.RefundAmount,
                        Status = r.Status,
                        IsActive = r.IsActive,
                        DateCreated = r.DateCreated,
                    })
                    .ToListAsync();
        }

        public List<SearchComboBoxDto> GetSearchComboBoxDtos()
        {
            return new List<SearchComboBoxDto>()
            {
                new SearchComboBoxDto()
                {
                    PropertyTitle = nameof(Return.Status),
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
                    PropertyTitle = nameof(Return.Status),
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

        public async Task<ReturnDto?> GetReturnByRefundCode(string refundCode)
        {
            return await _context.Return
                .Where(r => r.IsActive && r.Refund != null && r.Refund.Code == refundCode)
                .Select(r => new ReturnDto
                {
                    Reason = r.Reason,
                    RefundCode = r.Refund.Code,
                    RefundAmount = r.RefundAmount,
                    Status = r.Status,
                    IsActive = r.IsActive,
                    DateCreated = r.DateCreated,
                })
                .FirstOrDefaultAsync();
        }

        private bool IsValidReturnStatusTransition(string current, string next)
        {
            var transitions = new Dictionary<string, List<string>>
            {
                { "Pending", new List<string> { "Processing", "Cancelled" } },
                { "Processing", new List<string> { "Shipped", "Cancelled" } },
                { "Shipped", new List<string> { "Delivered" } },
                { "Delivered", new List<string>() { ReturnStatuses.Approved.ToString() } },
                { "Cancelled", new List<string>() },
                { "Rejected", new List<string>() }
            };

            return transitions.ContainsKey(current) && transitions[current].Contains(next);
        }
    }
}

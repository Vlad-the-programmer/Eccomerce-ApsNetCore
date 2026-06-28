using EcommerceRestApi.Helpers.Enums;
using EcommerceRestApi.Models.Context;
using EcommerceRestApi.Models.Dtos.Analitics;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace EcommerceRestApi.Services
{
    public class AnanliticsService : IAnaliticsService
    {
        private readonly AppDbContext _context;

        public AnanliticsService(AppDbContext context)
        {
            _context = context;
        }

        // =============================
        // 📊 DASHBOARD SUMMARY
        // =============================
        public async Task<DashboardSummaryDto> GetDashboardSummary(DateTime? from = null, DateTime? to = null)
        {
            var orders = _context.Orders.AsQueryable();
            var refunds = _context.Refund.AsQueryable();
            var returns = _context.Return.AsQueryable();

            if (from.HasValue)
            {
                orders = orders.Where(x => x.DateCreated >= from);
                refunds = refunds.Where(x => x.DateCreated >= from);
                returns = returns.Where(x => x.DateCreated >= from);
            }

            if (to.HasValue)
            {
                orders = orders.Where(x => x.DateCreated <= to);
                refunds = refunds.Where(x => x.DateCreated <= to);
                returns = returns.Where(x => x.DateCreated <= to);
            }

            var today = DateTime.UtcNow.Date;

            return new DashboardSummaryDto
            {
                TotalRevenue = await orders
                    .Include(o => o.Returns)
                    .Where(o => o.Status == OrderStatuses.Delivered.ToString() &&
                        !o.Returns.Any(r => r.Status == ReturnStatuses.Refunded.ToString()))
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                TotalOrders = await orders.CountAsync(),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalRefunds = await refunds.CountAsync(),
                TotalReturns = await returns.CountAsync(),

                RevenueToday = await _context.Orders
                    .Include(o => o.Returns)
                     .Where(o => o.Status == OrderStatuses.Delivered.ToString() &&
                        !o.Returns.Any(r => r.Status == ReturnStatuses.Refunded.ToString()))
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                OrdersToday = await _context.Orders
                    .CountAsync(o => o.DateCreated >= today)
            };
        }

        // =============================
        // 💰 REVENUE
        // =============================
        public async Task<RevenueAnalyticsDto> GetRevenueAnalytics(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders
                .Include(o => o.Returns)
                .Where(o => o.Status == OrderStatuses.Delivered.ToString() &&
                        !o.Returns.Any(r => r.Status == ReturnStatuses.Refunded.ToString()));

            if (from.HasValue)
                query = query.Where(x => x.DateCreated >= from);

            if (to.HasValue)
                query = query.Where(x => x.DateCreated <= to);

            var total = await query.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var monthly = await query
                .Where(x => x.DateCreated >= DateTime.UtcNow.AddMonths(-1))
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

            var weekly = await query
                .Where(x => x.DateCreated >= DateTime.UtcNow.AddDays(-7))
                .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

            var chartResult = await query
                .GroupBy(x => x.DateCreated.Date)
                .Select(g => new
                {
                    Label = g.Key,
                    Value = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            var chart = chartResult.Select(x => new ChartPointDto
            {
                Label = x.Label.ToString("yyyy-MM-dd"),
                Value = x.Value
            }).ToList();

            return new RevenueAnalyticsDto
            {
                TotalRevenue = total,
                MonthlyRevenue = monthly,
                WeeklyRevenue = weekly,
                RevenueByDate = chart
            };
        }

        // =============================
        // 📦 ORDERS
        // =============================
        public async Task<OrdersAnalyticsDto> GetOrdersAnalytics(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders.AsQueryable();

            if (from.HasValue)
                query = query.Where(x => x.DateCreated >= from);

            if (to.HasValue)
                query = query.Where(x => x.DateCreated <= to);

            var total = await query.CountAsync();
            var completed = await query.CountAsync(x => x.Status == OrderStatuses.Delivered.ToString());
            var cancelled = await query.CountAsync(x => x.Status == OrderStatuses.Cancelled.ToString());

            var chartResult = await query
                .GroupBy(x => x.DateCreated.Date)
                .Select(g => new
                {
                    Label = g.Key,
                    Value = g.Sum(x => x.TotalAmount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            var chart = chartResult.Select(x => new ChartPointDto
            {
                Label = x.Label.ToString("yyyy-MM-dd"),
                Value = x.Value
            }).ToList();

            return new OrdersAnalyticsDto
            {
                TotalOrders = total,
                CompletedOrders = completed,
                CancelledOrders = cancelled,
                OrdersByDate = chart
            };
        }

        // =============================
        // 💸 REFUNDS
        // =============================
        public async Task<RefundAnalyticsDto> GetRefundAnalytics(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Refund.AsQueryable();

            if (from.HasValue)
                query = query.Where(x => x.DateCreated >= from);

            if (to.HasValue)
                query = query.Where(x => x.DateCreated <= to);

            return new RefundAnalyticsDto
            {
                TotalRefunds = await query.CountAsync(),
                TotalRefundAmount = await query.SumAsync(x => (decimal?)x.Amount) ?? 0,

                Pending = await query.CountAsync(x => x.Status == "Pending"),
                Approved = await query.CountAsync(x => x.Status == "Approved"),
                Rejected = await query.CountAsync(x => x.Status == "Rejected")
            };
        }

        // =============================
        // 📦 RETURNS
        // =============================
        public async Task<ReturnAnalyticsDto> GetReturnAnalytics(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Return.AsQueryable();

            if (from.HasValue)
                query = query.Where(x => x.DateCreated >= from);

            if (to.HasValue)
                query = query.Where(x => x.DateCreated <= to);

            return new ReturnAnalyticsDto
            {
                TotalReturns = await query.CountAsync(),

                Pending = await query.CountAsync(x => x.Status == "Pending"),
                Processing = await query.CountAsync(x => x.Status == "Processing"),
                Delivered = await query.CountAsync(x => x.Status == "Delivered"),
                Cancelled = await query.CountAsync(x => x.Status == "Cancelled")
            };
        }

        // =============================
        // 🏆 TOP PRODUCTS
        // =============================
        public async Task<List<TopProductDto>> GetTopProducts(int count = 5)
        {
            return await _context.OrderItems
                .Include(o => o.Order)
                    .ThenInclude(o => o.Returns)
                .Where(o => o.Order.Status == OrderStatuses.Delivered.ToString() &&
                        !o.Order.Returns.Any(r => r.Status == ReturnStatuses.Refunded.ToString()))
                .GroupBy(oi => oi.Product.Name)
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(count)
                .ToListAsync();
        }

        // =============================
        // 👤 CUSTOMERS
        // =============================
        public async Task<CustomerAnalyticsDto> GetCustomerAnalytics()
        {
            var total = await _context.Customers.CountAsync();

            var monthAgo = DateTime.UtcNow.AddMonths(-1);

            var newCustomers = await _context.Customers
                .CountAsync(x => x.DateCreated >= monthAgo);

            return new CustomerAnalyticsDto
            {
                TotalCustomers = total,
                NewCustomersThisMonth = newCustomers
            };
        }

        public async Task<byte[]> ExportOrdersData(DateTime? from = null, DateTime? to = null)
        {
            var query = _context.Orders.AsQueryable();

            if (from.HasValue)
                query = query.Where(o => o.OrderDate >= from);

            if (to.HasValue)
                query = query.Where(o => o.OrderDate <= to);


            var data = await query
                .SelectMany(o => o.OrderItems.Select(item => new OrderExportDto
                {
                    OrderId = o.Code,
                    OrderDate = o.OrderDate,
                    CustomerName = o.Customer.User.FullName,
                    Region = o.Customer.Addresses
                        .Where(a => a.IsActive)
                        .Select(a => a.State)
                        .FirstOrDefault() ?? "N/A",

                    ProductName = item.Product.Name,
                    Category = item.Product.ProductCategories
                        .Select(pc => pc.Category.Name)
                        .FirstOrDefault(),

                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,

                    DiscountPercent = (o.OrderCoupons
                        .Select(c => (decimal?)c.DiscountApplied)
                        .FirstOrDefault() ?? 0) * 100,

                    TotalRevenue =
                        (item.Quantity * item.UnitPrice)
                        - o.OrderCoupons.Select(c => (decimal?)c.DiscountApplied).FirstOrDefault() ?? 0
                        - o.ShopCoinTransactionHistory
                            .Where(s => s.Type == ShopCoinTransactionType.SpendOrder.ToString())
                            .Select(s => (decimal?)s.Coins)
                            .FirstOrDefault() ?? 0,

                    Profit =
                        ((item.Quantity * item.UnitPrice)
                        - o.OrderCoupons.Select(c => (decimal?)c.DiscountApplied).FirstOrDefault() ?? 0
                        - o.ShopCoinTransactionHistory
                            .Where(s => s.Type == ShopCoinTransactionType.SpendOrder.ToString())
                            .Select(s => (decimal?)s.Coins)
                            .FirstOrDefault() ?? 0)
                        - item.Product.Price,

                    ShipMode = o.DeliveryMethodOrders
                        .Select(d => d.DeliveryMethod.MethodName)
                        .FirstOrDefault()
                }))
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Orders");

            string[] headers = {
                "Order ID", "Order Date", "Customer Name", "Region",
                "Product Name", "Category", "Quantity", "Unit Price",
                "Discount %", "Total Revenue", "Profit", "Ship Mode"
            };

            for (int i = 0; i < headers.Length; i++)
                ws.Cells[1, i + 1].Value = headers[i];

            int row = 2;

            foreach (var dto in data)
            {
                ws.Cells[row, 1].Value = dto.OrderId;
                ws.Cells[row, 2].Value = dto.OrderDate.ToString("yyyy-MM-dd");
                ws.Cells[row, 3].Value = dto.CustomerName;
                ws.Cells[row, 4].Value = dto.Region;
                ws.Cells[row, 5].Value = dto.ProductName;
                ws.Cells[row, 6].Value = dto.Category;
                ws.Cells[row, 7].Value = dto.Quantity;
                ws.Cells[row, 8].Value = dto.UnitPrice;
                ws.Cells[row, 9].Value = dto.DiscountPercent;
                ws.Cells[row, 10].Value = dto.TotalRevenue;
                ws.Cells[row, 11].Value = dto.Profit;
                ws.Cells[row, 12].Value = dto.ShipMode;

                row++;
            }

            return package.GetAsByteArray();
        }
    }
}
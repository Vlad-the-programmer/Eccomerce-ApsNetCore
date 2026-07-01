using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.Analitics;

namespace EcommerceRestApi.Services
{
    public interface IAnaliticsService
    {
        // 🔹 Dashboard summary
        Task<DashboardSummaryDto> GetDashboardSummary(DateTime? from = null, DateTime? to = null);

        // 🔹 Revenue analytics
        Task<RevenueAnalyticsDto> GetRevenueAnalytics(DateTime? from = null, DateTime? to = null);

        // 🔹 Orders analytics
        Task<OrdersAnalyticsDto> GetOrdersAnalytics(DateTime? from = null, DateTime? to = null);

        // 🔹 Refund analytics
        Task<RefundAnalyticsDto> GetRefundAnalytics(DateTime? from = null, DateTime? to = null);

        // 🔹 Returns analytics
        Task<ReturnAnalyticsDto> GetReturnAnalytics(DateTime? from = null, DateTime? to = null);

        // 🔹 Top products
        Task<List<TopProductDto>> GetTopProducts(int count = 5);

        // 🔹 Customers analytics
        Task<CustomerAnalyticsDto> GetCustomerAnalytics();
        Task<byte[]> ExportOrdersData(DateTime? from = null, DateTime? to = null);
        Task<InventoryDashboardDto> GetInventoryDashboard();
    }
}

using EcommerceRestApi.Helpers.Data.ResponseModels;
using EcommerceRestApi.Helpers.Data.Roles;
using EcommerceRestApi.Models.Dtos.Analitics;
using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    public class AnaliticsController : ControllerBase
    {
        private readonly IAnaliticsService _analiticsService;

        public AnaliticsController(IAnaliticsService analiticsService)
        {
            _analiticsService = analiticsService;
        }

        // =============================
        // 📊 DASHBOARD SUMMARY
        // =============================
        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboard(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _analiticsService.GetDashboardSummary(from, to);
                return Ok(result);

            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }
        }

        // =============================
        // 💰 REVENUE
        // =============================
        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueAnalyticsDto>> GetRevenue(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _analiticsService.GetRevenueAnalytics(from, to);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = $"{ex.Message} {ex.InnerException?.Message}" });
            }
        }

        // =============================
        // 📦 ORDERS
        // =============================
        [HttpGet("orders")]
        public async Task<ActionResult<OrdersAnalyticsDto>> GetOrders(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _analiticsService.GetOrdersAnalytics(from, to);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }

        }

        // =============================
        // 💸 REFUNDS
        // =============================
        [HttpGet("refunds")]
        public async Task<ActionResult<RefundAnalyticsDto>> GetRefunds(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _analiticsService.GetRefundAnalytics(from, to);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }

        }

        // =============================
        // 📦 RETURNS
        // =============================
        [HttpGet("returns")]
        public async Task<ActionResult<ReturnAnalyticsDto>> GetReturns(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var result = await _analiticsService.GetReturnAnalytics(from, to);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }

        }

        // =============================
        // 🏆 TOP PRODUCTS
        // =============================
        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopProductDto>>> GetTopProducts(
            [FromQuery] int count = 5)
        {
            try
            {
                var result = await _analiticsService.GetTopProducts(count);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }

        }

        // =============================
        // 👤 CUSTOMERS
        // =============================
        [HttpGet("customers")]
        public async Task<ActionResult<CustomerAnalyticsDto>> GetCustomers()
        {
            try
            {
                var result = await _analiticsService.GetCustomerAnalytics();
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new ResponseModel { Message = ex.Message });
            }

        }

        [HttpGet("export-orders")]
        public async Task<IActionResult> ExportOrders(DateTime? from, DateTime? to)
        {

            var bytes = await _analiticsService.ExportOrdersData(from, to);

            var fromStr = from?.ToString("yyyyMMdd") ?? "all";
            var toStr = to?.ToString("yyyyMMdd") ?? "now";
            Console.WriteLine($"Bytes: {bytes.ToString()}");
            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Orders_{fromStr}_{toStr}.xlsx"
            );
        }

        [HttpGet("inventory-dashboard")]
        public async Task<IActionResult> GetInventoryDashboard()
        {
            var dto = await _analiticsService.GetInventoryDashboard();

            return Ok(dto);
        }
    }
}
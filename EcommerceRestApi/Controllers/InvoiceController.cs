using EcommerceRestApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("order/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceByOrderCode(string orderCode)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByOrderCodeAsync(orderCode);
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("pdf/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GeneratePdf(string orderCode)
        {
            try
            {
                var pdfBytes = await _invoiceService.GeneratePdfInvoiceAsync(orderCode);
                return File(pdfBytes, "application/pdf", $"Invoice_{orderCode}.pdf");
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
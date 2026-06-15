// Controllers/InvoiceController.cs
using EcommerceWebApp.ApiServices;
using EcommerceWebApp.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebApp.Controllers
{
    [Route("invoice")]
    public class InvoiceController : Controller
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly IApiService _apiService;

        public InvoiceController(ILogger<InvoiceController> logger, IApiService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet("generate/{orderCode}")]
        public async Task<IActionResult> Generate(string orderCode)
        {
            ViewBag.OrderCode = orderCode;
            return View("Generate");
        }

        [HttpGet("generate-pdf")]
        public async Task<IActionResult> GeneratePdf([FromQuery] string orderCode)
        {
            try
            {
                var response = await _apiService.GetFileAsync($"{GlobalConstants.InvoiceEndpoint}/pdf/{orderCode}");
                return File(response, "application/pdf", $"Invoice_{orderCode}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Generate", new { orderCode });
            }
        }

        [HttpGet("view/{orderCode}")]
        public async Task<IActionResult> Details(string orderCode)
        {
            try
            {
                var invoice = await _apiService.GetDataAsync($"{GlobalConstants.InvoiceEndpoint}/order/{orderCode}");
                var invoiceDto = System.Text.Json.JsonSerializer.Deserialize<InvoiceDto>(invoice, GlobalConstants.JsonSerializerOptions);
                return View(invoiceDto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Generate", new { orderCode });
            }
        }
    }
}
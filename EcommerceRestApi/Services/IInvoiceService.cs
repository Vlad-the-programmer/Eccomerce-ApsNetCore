using EcommerceRestApi.Models.Dtos;

namespace EcommerceRestApi.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GetInvoiceByOrderCodeAsync(string orderCode);
        Task<byte[]> GeneratePdfInvoiceAsync(string orderCode);
    }
}

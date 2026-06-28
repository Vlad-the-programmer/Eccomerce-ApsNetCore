using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;

namespace EcommerceRestApi.Services
{
    public interface IRefundService
    {
        Task<List<RefundDto>> GetActiveRefunds(int customerId);
        Task<List<RefundDto>> GetAllRefunds();
        Task<RefundDto> GetRefundByCode(string code);
        Task ApplyForRefund(string orderCode, CreateRefundDto dto, string? userId);
        Task ChangeRefundStatus(ChangeRefundStatusDto dto, string changedBy);
        Task CancelRefund(string code, bool currentUserIsStuffOrAdmin, string? userId);
        Task CreatePaymentForRefund(string refundCode);
        Task<List<RefundDto>> GetAllRefundsForOrder(string orderCode);
        Task<List<RefundDto>> FilterRefundsAsync(string searchString, string? searchProperty, string? sortProperty, bool active, bool sortAscending);
        List<SearchComboBoxDto> GetSearchComboBoxDtos();
        List<SearchComboBoxDto> GetOrderByComboBoxDtos();

    }
}

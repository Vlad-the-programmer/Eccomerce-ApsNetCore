using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;

namespace EcommerceRestApi.Services
{
    public interface IReturnService
    {
        Task<List<ReturnDto>> GetCustomerReturnsAsync(int customerId);
        Task<List<ReturnDto>> GetAllReturnsAsync();
        Task<ReturnDto?> GetReturnByRefundCode(string refundCode);
        Task CreateReturn(string refundCode);
        Task ChangeReturnStatus(ChangeReturnStatusDto dto);
        Task CancelReturn(string refundCode);
        Task<List<ReturnDto>> FilterReturnsAsync(string searchString, string? searchProperty,
            string? sortProperty, bool sortAscending);
        List<SearchComboBoxDto> GetSearchComboBoxDtos();
        List<SearchComboBoxDto> GetOrderByComboBoxDtos();
    }
}

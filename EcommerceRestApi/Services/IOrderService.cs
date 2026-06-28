using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Models.Dtos.FilteringDtos;
using EcommerceRestApi.Services.Base;
using System.Security.Claims;

namespace EcommerceRestApi.Services
{
    public interface IOrderService : IEntityBaseRepository<Order>
    {
        Task<OrderDto> GetOrderByCodeAsync(string code);

        Task<OrderDto> AddNewOrderAsync(NewOrderViewModel data);

        Task UpdateOrderAsync(string code, NewOrderViewModel data);
        Task DeleteOrderAsync(string code);
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task ChangeOrderStatusAsync(ChangeOrderStatusDto dto, string? currentUserId);
        Task<List<OrderDto>> FilterOrdersAsync(string searchString, string? searchProperty,
            string? sortProperty, DateTime? fromDate, DateTime? toDate, bool sortAscending);
        Task<NewOrderViewModel> CreateOrderCreateTemplate(string shoppingCartId, ClaimsPrincipal User);
        List<SearchComboBoxDto> GetSearchComboBoxDtos();
        List<SearchComboBoxDto> GetOrderByComboBoxDtos();
    }
}

using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Models.Dtos;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface IOrderService : IEntityBaseRepository<Order>
    {
        Task<OrderDto> GetOrderByCodeAsync(string code);

        Task<OrderDto> AddNewOrderAsync(OrderViewModel data);

        Task UpdateOrderAsync(string code, OrderViewModel data);
        Task DeleteOrderAsync(string code);
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
    }
}

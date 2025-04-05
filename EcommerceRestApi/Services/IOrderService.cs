using EcommerceRestApi.Helpers.Data.ViewModels;
using EcommerceRestApi.Models;
using EcommerceRestApi.Services.Base;

namespace EcommerceRestApi.Services
{
    public interface IOrderService : IEntityBaseRepository<Order>
    {
        Task<OrderViewModel> GetOrderByCodeAsync(string code);

        Task AddNewOrderAsync(OrderViewModel data);

        Task UpdateOrderAsync(string code, OrderViewModel data);
        Task DeleteOrderAsync(string code);
        Task<IEnumerable<OrderViewModel>> GetOrdersAsync();
    }
}

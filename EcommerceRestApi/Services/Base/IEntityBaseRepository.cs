using EcommerceRestApi.Models;
using System.Linq.Expressions;

namespace EcommerceRestApi.Services.Base
{
    public interface IEntityBaseRepository<T> where T : class, IEntityBase, new()
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIDAsync(int id);
        Task AddAsync(T entity);

        Task UpdateAsync(int id, T entity);

        Task DeleteAsync(int id);
    }
}

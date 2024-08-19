using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<T?> GetAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> RemoveAsync(T entity);
        Task SaveAsync();
    }
}

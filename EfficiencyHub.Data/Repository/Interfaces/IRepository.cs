using System.Linq.Expressions;

namespace EfficiencyHub.Data.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetQueryableWhere(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);

        Task DeleteAsync(Guid id);
        Task DeleteEntityAsync(T entity);
    }
}

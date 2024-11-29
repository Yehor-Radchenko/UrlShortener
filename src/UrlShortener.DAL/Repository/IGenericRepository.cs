using System.Linq.Expressions;
using UrlShortener.DAL.Entities;

namespace UrlShortener.DAL.Repository;

public interface IGenericRepository<T>
    where T : class, IEntity
{
    Task<T> GetAsNoTrackingAsync(Expression<Func<T, bool>> filter = null!, string[]? includeOptions = null);

    Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string[]? includeOptions = null);

    Task<IEnumerable<T>> GetAllAsNoTrackingAsync(Expression<Func<T, bool>> filter = null!, string[]? includeOptions = null);

    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includeOptions = null);

    void Create(T entity);

    void Update(T entity);

    void Delete(T entity);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
}

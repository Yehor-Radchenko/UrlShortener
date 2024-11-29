using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UrlShortener.Common.Exceptions;
using UrlShortener.DAL.Context;
using UrlShortener.DAL.Entities;

namespace UrlShortener.DAL.Repository;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class, IEntity
{
    private readonly DbSet<T> _dbSet;

    public GenericRepository(UrlShortenerDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        _dbSet = dbContext.Set<T>();
    }

    public virtual void Create(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.AnyAsync(filter);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsNoTrackingAsync(Expression<Func<T, bool>>? filter = null, string[]? includeOptions = null)
    {
        var query = _dbSet.AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeOptions != null)
        {
            foreach (var entity in includeOptions)
            {
                query = query.Include(entity);
            }
        }

        return await query.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string[]? includeOptions = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeOptions != null)
        {
            foreach (var entity in includeOptions)
            {
                query = query.Include(entity);
            }
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T> GetAsNoTrackingAsync(Expression<Func<T, bool>>? filter = null, string[]? includeOptions = null)
    {
        var query = _dbSet.AsNoTracking();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeOptions != null)
        {
            foreach (var entity in includeOptions)
            {
                query = query.Include(entity);
            }
        }

        var entityResult = await query.FirstOrDefaultAsync();
        return entityResult ?? throw new EntityNotFoundException($"Entity {typeof(T).Name} was not found in database.");
    }

    public virtual async Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, string[]? includeOptions = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (includeOptions != null)
        {
            foreach (var entity in includeOptions)
            {
                query = query.Include(entity);
            }
        }

        var entityResult = await query.FirstOrDefaultAsync();
        return entityResult ?? throw new EntityNotFoundException($"Entity {typeof(T).Name} was not found in database.");
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>>? filter)
    {
        IQueryable<T> query = _dbSet;

        try
        {
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstAsync();
        }
        catch (InvalidOperationException)
        {
            throw new EntityNotFoundException($"Entity {typeof(T).Name} was not found in database.");
        }
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}

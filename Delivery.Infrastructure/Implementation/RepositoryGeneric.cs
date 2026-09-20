using System.Linq.Expressions;
using Delivery.Core.Interfaces;
using Delivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Infrastructure.Implementation;

public class RepositoryGeneric<T>(DeliveryDbContext context) : IRepositoryGeneric<T> where T : class
{
    protected readonly DeliveryDbContext _context = context;

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _context.Set<T>().AddRangeAsync(entities, cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().AnyAsync(predicate, cancellationToken);
    }

    public void Delete(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool disableTracking,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

        }
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        bool disableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>> predicate,
        bool disableTracking = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        if (includes != null && includes.Length > 0)
        {
            foreach (var include in includes)
            {
                query = query.Include(include.ToString());
            }
        }

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(object id, bool disableTracking = false, CancellationToken cancellationToken = default)
    {
        T? entity = await _context.Set<T>().FindAsync([id], cancellationToken);

        if (entity != null && disableTracking)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        return entity;
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
    }
}
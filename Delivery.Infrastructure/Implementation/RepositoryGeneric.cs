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
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        if (include != null) 
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        bool disableTracking = true,
        CancellationToken cancellationToken = default,
        Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        if (include != null)
        {
            query = include(query);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();

        if (include != null)
        {
            query = include(query);
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

    public async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _context.Set<T>();

        if (disableTracking) query = query.AsNoTracking();
        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);

        // Contamos el total antes de paginar
        int totalCount = await query.CountAsync(cancellationToken);

        // Ordenamos
        if (orderBy != null) query = orderBy(query);

        // Paginamos en base de datos: Saltar (Pagina - 1) * Tamaño y Tomar (Tamaño)
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
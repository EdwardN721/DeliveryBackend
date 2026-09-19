# Interfaces
Ayudame a revisar las interfaces

IUnitOfWork
```csharp
using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Billing;
using Delivery.Core.Entities.Business;
using Delivery.Core.Entities.Identity;
using Delivery.Core.Entities.Transaction;

namespace Delivery.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Identity
    IRepositoryGeneric<Rol> Roles { get; }
    IRepositoryGeneric<Usuario> Usuarios { get; }
    IRepositoryGeneric<UsuarioDireccion> UsuarioDirecciones { get; }

    // Catalog
    IRepositoryGeneric<Categoria> Categorias { get; }
    IRepositoryGeneric<MetodoPago> MetodosPago { get; }
    IRepositoryGeneric<EstadoPedido> EstadoPedidos { get; }

    // Business
    IRepositoryGeneric<Personal> Empleados { get; }
    IRepositoryGeneric<Producto> Productos { get; }
    IRepositoryGeneric<Restaurante> Restaurantes { get; }
    IRepositoryGeneric<RestauranteDireccion> RestauranteDirecciones { get; }

    // Transaction
    IRepositoryGeneric<Pedido> Pedidos { get; }
    IRepositoryGeneric<PedidoDetalle> PedidoDetalles { get; }

    // Billing
    IRepositoryGeneric<Factura> Facturas { get; }

    /// <summary>
    /// Consolida y guarda de forma asíncrona todos los cambios pendientes en el contexto actual hacia la base de datos.
    /// </summary>
    /// <remarks>
    /// Este método emite los comandos de persistencia (INSERT, UPDATE, DELETE) acumulados en los repositorios. 
    /// Si no hay una transacción explícita activa, el proveedor de datos encapsulará estos cambios en una transacción implícita propia.
    /// </remarks>
    /// <param name="cancellationToken">Un token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona. El resultado contiene el número de filas afectadas en la base de datos.</returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    #region ManejoDeTransacciones

    
    /// <summary>
    /// Inicia de forma asíncrona una nueva transacción explícita en la base de datos.
    /// </summary>
    /// <remarks>
    /// Utilice este método cuando necesite agrupar múltiples operaciones complejas 
    /// que requieren un control estricto de aislamiento, asegurando que ninguna operación 
    /// se consolide de forma permanente hasta que se invoque <see cref="CommitTransactionAsync"/>.
    /// </remarks>
    /// <param name="cancellationToken">Un token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma y aplica de manera definitiva la transacción explícita activa actual en la base de datos.
    /// </summary>
    /// <remarks>
    /// Este método hace permanentes todos los cambios enviados previamente mediante <see cref="CommitAsync"/>. 
    /// Debe invocarse únicamente si se inició previamente una transacción con <see cref="BeginTransactionAsync"/>.
    /// </remarks>
    /// <param name="cancellationToken">Un token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    /// <exception cref="InvalidOperationException">Se lanza si no existe una transacción activa para confirmar.</exception>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Revierte de forma asíncrona todos los cambios realizados durante la transacción explícita actual, descartándolos por completo.
    /// </summary>
    /// <remarks>
    /// Invoque este método dentro de un bloque catch cuando ocurra un error o excepción en el flujo de negocio, 
    /// garantizando que la base de datos regrese a su estado original previo al <see cref="BeginTransactionAsync"/>.
    /// </remarks>
    /// <param name="cancellationToken">Un token para monitorear solicitudes de cancelación.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    #endregion
}
```

---
IGenericRepository
```csharp
using System.Linq.Expressions;

namespace Delivery.Core.Interfaces;

public interface IRepositoryGeneric<T> where T : class
{
    /// <summary>
    /// Obtiene informacion por Id
    /// </summary>
    /// <param name="id">Ide del objeto a buscar.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    /// <returns>Objeto de la base de datos.</returns>
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una lista de todos los objetos de la base de datos.
    /// </summary>
    /// <param name="disableTracking">Habilita la modificacion de datos.</param>
    /// <param name="cancellationToken">Token de cancelacion</param>
    /// <returns>Lista de objetos</returns>
    Task<IEnumerable<T>> GetAllAsync(bool disableTracking = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una lista de objetos que cumplan con la condición.
    /// </summary>
    /// <param name="predicate">Condicion o parametros a definir.</param>
    /// <param name="disableTracking">Habilita la modificacion de datos.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    /// <returns>Lista de objetos filtrada.</returns>
    Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> predicate, bool disableTracking = false, CancellationToken cancellationToken = default, params string[] includes);

    /// <summary>
    /// Verifica si existe algún objeto que cumpla con la condición especificada.
    /// </summary>
    /// <param name="predicate">Condición o parametro a definir.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Regresa true si existe al menos un objeto que cumpla con la condición, false en caso contrario.</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el primer objeto que cumpla con la condición especificada o null si no existe ninguno.
    /// </summary>
    /// <param name="predicate">Condición o parametros a definir.</param>
    /// <param name="disableTracking">Habilita la modificacion de datos.</param>
    /// <param name="cancellationToken">Token de cancelacion</param>
    /// <param name="includes">Condicion de Join</param>
    /// <returns>Regresa el primer objeto que cumpla con la condición o null si no existe ninguno.</returns>
    Task<T?>  FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool disableTracking, CancellationToken cancellationToken = default, 
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Agrega una entidad a la base de datos.
    /// </summary>
    /// <param name="entity">Información a agregar.</param>
    /// <param name="cancellationToken">Token de cancelacion.</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una entidad.
    /// </summary>
    /// <param name="entity">Datos de la entidad actualizada.</param>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    /// <param name="entity">Entidad a elminar.</param>
    void Delete(T entity);

    /// <summary>
    /// Agrega varias entidades al contexto de persistencia de forma asíncrona.
    /// </summary>
    /// <param name="entities">Entidades que se agregarán.</param>
    /// <param name="cancellationToken">Token para cancelar la operación.</param>
    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza varias entidades en el contexto de persistencia.
    /// </summary>
    /// <param name="entities">Entidades que se actualizarán.</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Elimina varias entidades del contexto de persistencia.
    /// </summary>
    /// <param name="entities">Entidades que se eliminarán.</param>
    void DeleteRange(IEnumerable<T> entities);
}
```

---
UnitOfWork
```csharp
using Delivery.Core.Entities.Billing;
using Delivery.Core.Entities.Business;
using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Identity;
using Delivery.Core.Entities.Transaction;
using Delivery.Core.Interfaces;
using Delivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Delivery.Infrastructure.Implementation;

public class UnitOfWork(DeliveryDbContext dbContext) : IUnitOfWork
{
    private readonly DeliveryDbContext _dbContext = dbContext;
    private IDbContextTransaction? _currentTransaction;

    #region Repository Generic

    private IRepositoryGeneric<Rol>? _rol;
    private IRepositoryGeneric<Usuario>? _usuario;
    private IRepositoryGeneric<UsuarioDireccion>? _usuarioDireccion;
    private IRepositoryGeneric<Categoria>? _categoria;
    private IRepositoryGeneric<EstadoPedido>? _estadoPedido;
    private IRepositoryGeneric<MetodoPago>? _metodoPago;
    private IRepositoryGeneric<Personal>? _personal;
    private IRepositoryGeneric<Producto>? _producto;
    private IRepositoryGeneric<Restaurante>? _restaurante;
    private IRepositoryGeneric<RestauranteDireccion>? _restauranteDireccion;
    private IRepositoryGeneric<Pedido>? _pedido;
    private IRepositoryGeneric<PedidoDetalle>? _pedidoDetalle;
    private IRepositoryGeneric<Factura>? _factura;

    public IRepositoryGeneric<Rol> Roles
    {
        get { return _rol ??= new RepositoryGeneric<Rol>(_dbContext); }
    }

    public IRepositoryGeneric<Usuario> Usuarios
    {
        get { return _usuario ??= new RepositoryGeneric<Usuario>(_dbContext); }
    }

    public IRepositoryGeneric<UsuarioDireccion> UsuarioDirecciones
    {
        get { return _usuarioDireccion ??= new RepositoryGeneric<UsuarioDireccion>(_dbContext); }
    }

    public IRepositoryGeneric<Categoria> Categorias
    {
        get { return _categoria ??= new RepositoryGeneric<Categoria>(_dbContext); }
    }

    public IRepositoryGeneric<EstadoPedido> EstadoPedidos
    {
        get { return _estadoPedido ??= new RepositoryGeneric<EstadoPedido>(_dbContext); }
    }

    public IRepositoryGeneric<MetodoPago> MetodosPago
    {
        get { return _metodoPago ??= new RepositoryGeneric<MetodoPago>(_dbContext); }
    }

    public IRepositoryGeneric<Personal> Empleados
    {
        get { return _personal ??= new RepositoryGeneric<Personal>(_dbContext); }
    }

    public IRepositoryGeneric<Producto> Productos
    {
        get { return _producto ??= new RepositoryGeneric<Producto>(_dbContext); }
    }

    public IRepositoryGeneric<Restaurante> Restaurantes
    {
        get { return _restaurante ??= new RepositoryGeneric<Restaurante>(_dbContext); }
    }

    public IRepositoryGeneric<RestauranteDireccion> RestauranteDirecciones
    {
        get { return _restauranteDireccion ??= new RepositoryGeneric<RestauranteDireccion>(_dbContext); }
    }

    public IRepositoryGeneric<Pedido> Pedidos
    {
        get { return _pedido ??= new RepositoryGeneric<Pedido>(_dbContext); }
    }

    public IRepositoryGeneric<PedidoDetalle> PedidoDetalles
    {
        get { return _pedidoDetalle ??= new RepositoryGeneric<PedidoDetalle>(_dbContext); }
    }

    public IRepositoryGeneric<Factura> Facturas
    {
        get { return _factura ??= new RepositoryGeneric<Factura>(_dbContext); }
    }

    #endregion

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("Ya éciste una transacción en curso.");
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await CommitAsync(cancellationToken);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }


    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {

        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            _currentTransaction?.Dispose();
            _currentTransaction = null;
        }

    }
}
```

---
RepositoryGeneric
```csharp
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
        if (disableTracking)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(cancellationToken);
        }
        return await _context.Set<T>().FindAsync([id], cancellationToken);
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
```
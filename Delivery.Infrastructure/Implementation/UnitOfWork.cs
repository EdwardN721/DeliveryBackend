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
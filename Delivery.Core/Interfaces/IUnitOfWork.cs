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
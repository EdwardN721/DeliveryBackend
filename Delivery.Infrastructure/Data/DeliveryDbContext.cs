using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Billing;
using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Business;
using Delivery.Core.Entities.Identity;
using Delivery.Core.Entities.Transaction;

namespace Delivery.Infrastructure.Data;

public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : DbContext(options)
{
    // Identity
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioDireccion> UsuarioDirecciones => Set<UsuarioDireccion>();

    // Business
    public DbSet<Personal> Empleados => Set<Personal>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Restaurante> Restaurantes => Set<Restaurante>();
    public DbSet<RestauranteDireccion> RestauranteDirecciones => Set<RestauranteDireccion>();

    // Catalog
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<EstadoPedido> EstadoPedidos => Set<EstadoPedido>();
    public DbSet<MetodoPago> MetodoPagos => Set<MetodoPago>();

    // Transaction
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<PedidoDetalle> PedidoDetalles => Set<PedidoDetalle>();
    
    // Billing
    public DbSet<Factura> Facturas => Set<Factura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
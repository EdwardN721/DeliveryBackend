# Infrastrucutre 
Revisa el proyecto actual

## Data
Data/DeliveryDbContext
```csharp
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
```

---
## Interceptor
Interceptor/AuditInterceptor
```csharp
using Delivery.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Delivery.Infrastructure.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        DbContext? dbContext = eventData.Context;
        if (dbContext is null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = dbContext.ChangeTracker.Entries<BaseEntity>();
        var entriesCatalog = dbContext.ChangeTracker.Entries<BaseEntityCatalog>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.CreatedBy = "System"; // Como agrego el usuario que esta haciendo la modificacion?
            }
            
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedBy = "System";
            }

            // Soft Delete Automático: Si intentan borrar, lo cambiamos a modificado y marcamos IsDeleted
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.EsEliminado = true;
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        foreach (var entry in entriesCatalog)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                entry.Entity.CreatedBy = "System";
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedBy = "System";
            }

            // Soft Delete Automático: Si intentan borrar, lo cambiamos a modificado y marcamos IsDeleted
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.EsEliminado = true;
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

---
## Configurations
Configurations/Billing/billig.md
Configurations/Business/business.md
Configurations/Catalog/catalog.md
Configurations/Identity/Identity.md
Configurations/Transaction/Transaction.md
using Delivery.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Delivery.Core.Interfaces;

namespace Delivery.Infrastructure.Interceptors;

public class AuditInterceptor(ICurrentUserService currentUserService) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService = currentUserService;

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
                entry.Entity.CreatedBy = _currentUserService.ObtenerUsuario();
            }
            
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedBy = _currentUserService.ObtenerUsuario();
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
                entry.Entity.CreatedBy = _currentUserService.ObtenerUsuario();
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
                entry.Entity.UpdatedBy = _currentUserService.ObtenerUsuario();
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
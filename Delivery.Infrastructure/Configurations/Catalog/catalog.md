# Catalog configuration
Valida los archivos

Categoria
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Catalog;

public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categorias", "Catalog");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Descripcion)
            .HasMaxLength(500);

        AuditConfiguration.ConfigureAuditCatalog(builder);
    }
}

```

---
EstadoPedido
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Catalog;

public class EstadoPedidoConfiguration : IEntityTypeConfiguration<EstadoPedido>
{
    public void Configure(EntityTypeBuilder<EstadoPedido> builder)
    {
        builder.ToTable("EstadosPedidos", "Catalog");
        builder.HasKey(ep => ep.Id);

        builder.Property(ep => ep.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        AuditConfiguration.ConfigureAuditCatalog(builder);
    }
}
```

---
MetodoPago
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Catalog;

public class MetodoPagoConfiguration : IEntityTypeConfiguration<MetodoPago>
{
    public void Configure(EntityTypeBuilder<MetodoPago> builder)
    {
        builder.ToTable("MetodosPagos", "Catalog");
        builder.HasKey(mp => mp.Id);

        builder.Property(mp => mp.Nombre)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(mp => mp.Descripcion)
            .HasMaxLength(500);

        AuditConfiguration.ConfigureAuditCatalog(builder);
    }
}
```

---
Auditable
```csharp
using Delivery.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations;

public abstract class AuditConfiguration
{
    public static void ConfigureAuditBase<T>(EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .IsRequired(false);

        builder.Property(x => x.EsActivo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.EsEliminado)
            .IsRequired()
            .HasDefaultValue(false);
    }

    public static void ConfigureAuditCatalog<T>(EntityTypeBuilder<T> builder) where T : BaseEntityCatalog
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .IsRequired(false);

        builder.Property(x => x.EsActivo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.EsEliminado)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
```
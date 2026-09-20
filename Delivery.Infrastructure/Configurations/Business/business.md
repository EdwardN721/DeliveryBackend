# Business Configuration
Ayudame a validar

Personal
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Business;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Business;

public class PersonalConfiguration : IEntityTypeConfiguration<Personal>
{
    public void Configure(EntityTypeBuilder<Personal> builder)
    {
        builder.ToTable("Personal", "Business");
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Sucursal)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(p => p.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
```

---
Producto
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Business;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Business;

public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Producto", "Business");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Descripcion)
            .IsRequired()
            .HasMaxLength(500);
    
        builder.Property(p => p.Precio)
            .IsRequired()
            .HasPrecision(18, 2);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
```

---
Restaurante
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Business;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Business;

public class RestauranteConfiguration : IEntityTypeConfiguration<Restaurante>
{
    public void Configure(EntityTypeBuilder<Restaurante> builder)
    {
        builder.ToTable("Restaurante", "Business");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        AuditConfiguration.ConfigureAuditBase(builder);

        builder.HasMany(r => r.Direcciones)
            .WithOne(rd => rd.Restaurante)
            .HasForeignKey(rd => rd.RestauranteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Usuarios)
            .WithOne(p => p.Sucursal)
            .HasForeignKey(p => p.RestauranteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```
---
RestauranteDireccion
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Business;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Business;

public class RestauranteDireccionConfiguration : IEntityTypeConfiguration<RestauranteDireccion>
{
    public void Configure(EntityTypeBuilder<RestauranteDireccion> builder)
    {
        builder.ToTable("RestauranteDireccion", "Business");

        builder.HasKey(rd => rd.Id);

        builder.Property(rd => rd.Calle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rd => rd.Numero)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(rd => rd.Colonia)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rd => rd.Ciudad)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rd => rd.Estado)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(rd => rd.CodigoPostal)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(rd => rd.Restaurante)
            .WithMany(r => r.Direcciones)
            .HasForeignKey(rd => rd.RestauranteId)
            .OnDelete(DeleteBehavior.Cascade);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
```


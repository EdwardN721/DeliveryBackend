# Transaction Configuration

Pedido
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Transaction;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Transaction;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedido", "Transaction");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Total)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.HasOne(p => p.Restaurante)
            .WithMany()
            .HasForeignKey(p => p.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Cliente)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.EstadoPedido)
            .WithMany()
            .HasForeignKey(p => p.EstadoPedidoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(p => p.Direccion)
            .WithMany()
            .HasForeignKey(p => p.DireccionId)
            .OnDelete(DeleteBehavior.Restrict);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
```

---
PedidoDetalleConfiguration
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Transaction;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Transaction;

public class PedidoDetalleConfiguration : IEntityTypeConfiguration<PedidoDetalle>
{
    public void Configure(EntityTypeBuilder<PedidoDetalle> builder)
    {
        builder.ToTable("PedidoDetalles", "Transaction");
        builder.HasKey(p => p.Id);

        builder.Property(pd => pd.Cantidad)
            .IsRequired();

        builder.Property(pd => pd.PrecioUnitario)
            .IsRequired()
            .HasPrecision(18,2);

        builder.HasOne(pd => pd.Pedido)
            .WithMany()
            .HasForeignKey(pd => pd.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pd => pd.Producto)
            .WithMany()
            .HasForeignKey(pd => pd.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
```
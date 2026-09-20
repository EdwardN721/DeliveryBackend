# billing configuration
Atudame a revisarlo

Factura
```csharp
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Billing;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Billing;

public class FacturaConfiguration : IEntityTypeConfiguration<Factura>
{
    public void Configure(EntityTypeBuilder<Factura> builder)
    {
        builder.ToTable("Factura", "Billing");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Total)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(f => f.UrlFactura)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(f => f.Pedido)
            .WithMany()
            .HasForeignKey(f => f.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.MetodoPago)
            .WithMany()
            .HasForeignKey(f => f.MetodoPagoId)
            .OnDelete(DeleteBehavior.Restrict);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
```
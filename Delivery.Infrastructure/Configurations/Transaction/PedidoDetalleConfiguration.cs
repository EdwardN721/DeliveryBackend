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
            .WithMany(p => p.Detalles)
            .HasForeignKey(pd => pd.PedidoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pd => pd.Producto)
            .WithMany()
            .HasForeignKey(pd => pd.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
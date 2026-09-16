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
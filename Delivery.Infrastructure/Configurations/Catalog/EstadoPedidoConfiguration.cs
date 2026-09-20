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
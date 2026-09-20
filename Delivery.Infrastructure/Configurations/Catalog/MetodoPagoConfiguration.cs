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
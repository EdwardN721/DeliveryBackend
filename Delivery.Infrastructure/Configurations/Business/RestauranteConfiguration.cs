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
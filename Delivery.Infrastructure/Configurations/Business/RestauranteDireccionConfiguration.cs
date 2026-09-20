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
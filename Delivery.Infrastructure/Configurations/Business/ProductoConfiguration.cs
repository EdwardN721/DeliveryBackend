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

        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Delivery.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Identity;

public class UsuarioDireccionConfiguration : IEntityTypeConfiguration<UsuarioDireccion>
{
    public void Configure(EntityTypeBuilder<UsuarioDireccion> builder)
    {
        builder.ToTable("UsuarioDirecciones", "Identity");
        builder.HasKey(ud => ud.Id);

        builder.Property(ud => ud.Calle)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(ud => ud.Numero)
            .IsRequired()
            .HasMaxLength(10);
        
        builder.Property(ud => ud.Colonia)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ud => ud.Ciudad)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ud => ud.CodigoPostal)
            .IsRequired()
            .HasMaxLength(5);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
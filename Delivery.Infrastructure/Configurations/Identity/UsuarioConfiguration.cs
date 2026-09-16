using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Identity;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios", "Identity");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombres)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(u => u.PrimerApellido)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.SegundoApellido)
            .HasMaxLength(50);

        builder.Property(u => u.Correo)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(u => u.Telefono)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(500);

        AuditConfiguration.ConfigureAuditBase(builder);

        builder.HasMany(u => u.Roles)
        .WithMany(r => r.Usuarios)
        .UsingEntity("UsuarioRoles");

        builder.HasMany(u => u.Direcciones)
        .WithOne(ud => ud.Usuario)
        .HasForeignKey(ud => ud.UsuarioId)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
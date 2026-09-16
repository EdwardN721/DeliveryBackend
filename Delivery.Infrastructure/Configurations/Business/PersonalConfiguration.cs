using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Business;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations.Business;

public class PersonalConfiguration : IEntityTypeConfiguration<Personal>
{
    public void Configure(EntityTypeBuilder<Personal> builder)
    {
        builder.ToTable("Personal", "Business");
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Sucursal)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(p => p.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);

        AuditConfiguration.ConfigureAuditBase(builder);
    }
}
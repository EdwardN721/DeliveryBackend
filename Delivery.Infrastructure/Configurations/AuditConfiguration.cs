using Delivery.Core.Entities;
using Delivery.Core.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Configurations;

public abstract class AuditConfiguration
{
    public static void ConfigureAuditBase<T>(EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .IsRequired(false);

        builder.Property(x => x.EsActivo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.EsEliminado)
            .IsRequired()
            .HasDefaultValue(false);
    }

    public static void ConfigureAuditCatalog<T>(EntityTypeBuilder<T> builder) where T : BaseEntityCatalog
    {
        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        builder.Property(x => x.UpdatedAt)
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        builder.Property(x => x.UpdatedBy)
            .IsRequired(false);

        builder.Property(x => x.EsActivo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.EsEliminado)
            .IsRequired()
            .HasDefaultValue(false);
    }
}
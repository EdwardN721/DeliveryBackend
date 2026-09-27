namespace Delivery.Core.Entities.Transaction;

public class BaseAuditableEntity : BaseEntity
{
    public DateTimeOffset FechaCreacion { get; set; }
}

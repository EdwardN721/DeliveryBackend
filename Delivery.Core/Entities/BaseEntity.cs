namespace Delivery.Core.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool EsActivo { get; set; } = true;
    public bool EsEliminado { get; set; } = false;
}

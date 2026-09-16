namespace Delivery.Core.Entities.Catalog;

public class BaseEntityCatalog
{
    public int Id { get; set; }
    public bool EsActivo { get; set; } = true;
    public bool EsEliminado { get; set; } = false;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? UpdatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
}
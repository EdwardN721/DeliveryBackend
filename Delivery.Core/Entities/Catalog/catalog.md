# Revisa Catalog
Puedes revisar catalogo

BaseEntityCatalog
```csharp
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
```

----
Categoria
```csharp
namespace Delivery.Core.Entities.Catalog;

public class Categoria : BaseEntityCatalog
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
```

---
EstadoPedido
```csharp
namespace Delivery.Core.Entities.Catalog;

public class EstadoPedido : BaseEntityCatalog
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
```

---
Enum
```csharp
namespace Delivery.Core.Enum;

public enum EstadoPedidoEnum
{
    Pendiente,
    EnProceso,
    Completado,
    Cancelado
}
```

---
MetodoPago
```csharp
namespace Delivery.Core.Entities.Catalog;

public class MetodoPago : BaseEntityCatalog
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
```
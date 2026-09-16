# Valida Business
Que tal Business

Restaurante
```csharp
namespace Delivery.Core.Entities.Business;

public class Restaurante : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;

    public virtual ICollection<RestauranteDireccion> Direcciones { get; set; } = new List<RestauranteDireccion>();
    public virtual ICollection<Personal> Usuarios { get; set; } = new List<Personal>();
}
```

---
Direccion
```csharp
namespace Delivery.Core.Entities.Business;

public class RestauranteDireccion : BaseEntity
{
    public string Calle {get; set; } = string.Empty;
    public string Numero {get; set; } = string.Empty;
    public string Colonia {get; set; } = string.Empty;
    public string Ciudad {get; set; } = string.Empty;
    public string Estado {get; set; } = string.Empty;
    public string CodigoPostal {get; set; } = string.Empty;

    public Guid RestauranteId {get; set; }
    public virtual Restaurante? Restaurante { get; set; }
}
```

---
Personal
```csharp
using Delivery.Core.Entities.Identity;

namespace Delivery.Core.Entities.Business;

public class Personal : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }

    public Guid RestauranteId { get; set; }
    public virtual Restaurante? Sucursal { get; set; }
}
```

---
Product
```csharp
using Delivery.Core.Entities.Catalog;

namespace Delivery.Core.Entities.Business;

public class Producto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }

    public Guid RestauranteId { get; set; }
    public Restaurante Restaurante { get; set; } = null!;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
}
```
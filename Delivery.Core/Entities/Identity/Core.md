# Valida Core
Valida core por favor

BaseEntity
```csharp
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

```

---
Roles
```csharp
namespace Delivery.Core.Entities.Identity;

public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();    
}
```

---
Usuarios
```csharp
namespace Delivery.Core.Entities.Identity;

public class Usuario : BaseEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public virtual ICollection<Rol> Roles { get; set; } = new List<Rol>();
    public virtual ICollection<UsuarioDireccion> Direcciones { get; set; } = new List<UsuarioDireccion>();
}
```

---
Direcciones
```csharp
namespace Delivery.Core.Entities.Identity;

public class UsuarioDireccion : BaseEntity
{
    public string Calle {get; set; } = string.Empty;
    public string Numero {get; set; } = string.Empty;
    public string Colonia {get; set; } = string.Empty;
    public string Ciudad {get; set; } = string.Empty;
    public string Estado {get; set; } = string.Empty;
    public string CodigoPostal {get; set; } = string.Empty;

    public Guid UsuarioId {get; set; }
    public virtual Usuario? Usuario { get; set; }
}
```

**Algunos nombre pueden cambiar en el entity contra el script, solo es por mejor convencion o entendimiento**
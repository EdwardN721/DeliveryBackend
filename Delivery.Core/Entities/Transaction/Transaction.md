# Transaction
Evalua Transaction

Pedido
```csharp
using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Business;
using Delivery.Core.Entities.Identity;

namespace Delivery.Core.Entities.Transaction;

public class Pedido : BaseEntity
{
    public decimal Total { get; set; }

    public Guid UsuarioId { get; set; }
    public virtual Usuario Cliente { get; set; } = null!;

    public Guid RestauranteId { get; set; }
    public virtual Restaurante Restaurante { get; set; } = null!;

    public Guid EstadoPedidoId { get; set; }
    public virtual EstadoPedido EstadoPedido { get; set; } = null!;
}

```

---
PedidoDetall
```csharp
using Delivery.Core.Entities.Business;

namespace Delivery.Core.Entities.Transaction;

public class PedidoDetalle : BaseEntity
{
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public Guid PedidoId { get; set; }
    public virtual Pedido Pedido { get; set; } = null!;

    public Guid ProductoId { get; set; }
    public virtual Producto Producto { get; set; } = null!;
}
```

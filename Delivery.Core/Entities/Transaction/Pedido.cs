using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Business;
using Delivery.Core.Entities.Identity;

namespace Delivery.Core.Entities.Transaction;

public class Pedido : BaseAuditableEntity
{
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    public decimal CostoEnvio { get; set; }

    public Guid UsuarioId { get; set; }
    public virtual Usuario Cliente { get; set; } = null!;

    public Guid RestauranteId { get; set; }
    public virtual Restaurante Restaurante { get; set; } = null!;

    public int EstadoPedidoId { get; set; }
    public virtual EstadoPedido EstadoPedido { get; set; } = null!;

    public Guid DireccionEntregaId { get; set; }
    public virtual UsuarioDireccion DireccionEntrega { get; set; } = null!;

    public virtual ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
}

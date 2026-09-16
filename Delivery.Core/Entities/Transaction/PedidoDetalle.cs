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
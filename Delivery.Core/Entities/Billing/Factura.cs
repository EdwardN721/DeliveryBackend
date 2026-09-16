using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Transaction;

namespace Delivery.Core.Entities.Billing;

public class Factura : BaseEntity
{
    public decimal Total { get; set; }
    public string UrlFactura { get; set; } = string.Empty;

    public Guid PedidoId { get; set; }
    public virtual Pedido Pedido { get; set; } = null!;
    
    public int MetodoPagoId { get; set; }
    public virtual MetodoPago MetodoPago { get; set; } = null!;
}
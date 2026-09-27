using Delivery.Core.Entities.Transaction;

namespace Delivery.Application.Mappers;

public static class PedidoDetalleMapper
{
    public static PedidoDetalle MapToEntity(Guid producto, int cantidad, decimal precioUnitario)
    {
        return new PedidoDetalle
        {
            ProductoId = producto,
            Cantidad = cantidad,
            PrecioUnitario = precioUnitario,
            Impuesto = 0, 
            Descuento = 0
        };
    }
}
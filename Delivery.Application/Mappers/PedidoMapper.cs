using Delivery.Application.Dto.Response;
using Delivery.Core.Entities.Transaction;
using Delivery.Application.Features.Commands.Transaction.Pedidos;

namespace Delivery.Application.Mappers;

public static class PedidoMapper
{
    public static Pedido MapToEntity(this CreatePedidoCommand command)
    {
        return new Pedido
        {
            RestauranteId = command.RestauranteId,
            DireccionEntregaId = command.DireccionEntregaId,
        };
    }

    public static PedidoDto MapToDto(this Pedido pedido)
    {
        string nombreUsuario = string.Join(" ", 
            new[] { pedido.Cliente?.Nombres, pedido.Cliente?.PrimerApellido, pedido.Cliente?.SegundoApellido }
            .Where(parte => !string.IsNullOrWhiteSpace(parte)));
        
        string direccionUsuario = pedido.DireccionEntrega != null 
            ? $"Calle: {pedido.DireccionEntrega.Calle} - Numero: {pedido.DireccionEntrega.Numero} - Ciudad: {pedido.DireccionEntrega.Ciudad} - CP: {pedido.DireccionEntrega.CodigoPostal}"
            : "Dirección no disponible";

        return new PedidoDto
        {
            Id = pedido.Id,
            FechaCreacion = pedido.FechaCreacion,
            SubTotal = pedido.SubTotal,
            CostoEnvio = pedido.CostoEnvio,
            Total = pedido.Total,
            EstadoPedido = pedido.EstadoPedido?.Nombre ?? "Desconocido",
            NombreUsuario = nombreUsuario,
            DireccionUsuario = direccionUsuario,
            NombreRestaurante = pedido.Restaurante?.Nombre ?? "Desconocido",

            Detalles = pedido.Detalles?.Select(d => new DetallePedidoDto
            {
                Id = d.Id,
                ProductoId = d.ProductoId,
                NombreProducto = d.Producto?.Nombre ?? "Desconocido",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Impuesto = d.Impuesto,
                Descuento = d.Descuento
            }).ToList() ?? []
        };
    }

    public static IEnumerable<PedidoDto> MapToDto(this IEnumerable<Pedido>? pedidos)
    {
        return pedidos?.Select(MapToDto) ?? [];
    }
}

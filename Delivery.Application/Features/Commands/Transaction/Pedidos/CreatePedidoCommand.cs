using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Transaction.Pedidos;

public record CreatePedidoCommand : IRequest<Result<Guid>>
{
    public Guid RestauranteId { get; init; }
    public Guid DireccionEntregaId { get; init; }

    // Lista de producto - cantidad
    public List<PedidoDetalleCommand> Carrito { get; init; } = new List<PedidoDetalleCommand>(); 

    // UsuarioId no se pide, se obtiene del token
}

public record PedidoDetalleCommand
{
    public Guid ProductoId { get; init; }
    public int Cantidad { get; init; }
}

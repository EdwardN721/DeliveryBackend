using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Transaction.Pedidos;

public class CambiarEstadoPedidoCommand : IRequest<Result>
{
    public Guid IdPedido { get; init; }
    
    public int NuevoEstadoId { get; init; }
}

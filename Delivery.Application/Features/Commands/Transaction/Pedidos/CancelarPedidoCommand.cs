using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Transaction.Pedidos;

public class CancelarPedidoCommand : IRequest<Result>
{
    public Guid IdPedido { get; init; }
    public string MotivoCancelacion { get; init; } = string.Empty;
}

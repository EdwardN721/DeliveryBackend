using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Transaction.Pedidos;

public record GetPedidoByIdQuery(Guid Id) : IRequest<Result<PedidoDto>>;
using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;
using Delivery.Core.Pagination;

namespace Delivery.Application.Features.Queries.Transaction.Pedidos;

public class GetMisPedidosQuery : PaginationParams, IRequest<Result<PagedList<PedidoDto>>>
{
    public int? EstadoPedidoId { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
}
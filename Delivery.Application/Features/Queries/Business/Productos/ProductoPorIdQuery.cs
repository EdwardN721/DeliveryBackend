using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Business.Productos;

public record ProductoPorIdQuery : IRequest<Result<ProductoDto>>
{
    public Guid Id { get; init; }
}

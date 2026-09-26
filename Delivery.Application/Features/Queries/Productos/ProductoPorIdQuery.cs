using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Productos;

public record ProductoPorIdQuery : IRequest<Result<ProductoDto>>
{
    public Guid Id { get; init; }
}

using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.RestauranteDirecciones;

public record RestauranteDireccionListQuery : IRequest<Result<IEnumerable<RestauranteDireccionDto>>>
{
    public Guid? RestauranteId { get; init; }
}
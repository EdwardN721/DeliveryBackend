using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Business.RestauranteDirecciones;

public record RestauranteDireccionByIdQuery : IRequest<Result<RestauranteDireccionDto>>
{
    public Guid Id { get; init; }
}
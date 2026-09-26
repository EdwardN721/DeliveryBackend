using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.RestauranteDirecciones;

public record DeleteRestauranteDireccionCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
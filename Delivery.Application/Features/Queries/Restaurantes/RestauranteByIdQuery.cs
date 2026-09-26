using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Restaurantes;

public record RestauranteByIdQuery : IRequest<Result<RestauranteDto>>
{
    public Guid Id { get; init; }
}
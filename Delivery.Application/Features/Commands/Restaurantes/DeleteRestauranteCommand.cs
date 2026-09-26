using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Restaurantes;

public record DeleteRestauranteCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}
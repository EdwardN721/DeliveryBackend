using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Restaurantes;

public record CreateRestauranteCommand : IRequest<Result<Guid>>
{
    public string Nombre { get; init; } = string.Empty;
}
using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Business.Restaurantes;

public record UpdateRestauranteCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
}
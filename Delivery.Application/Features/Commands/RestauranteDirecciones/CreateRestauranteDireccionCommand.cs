using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.RestauranteDirecciones;

public record CreateRestauranteDireccionCommand : IRequest<Result<Guid>>
{
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid RestauranteId { get; init; }
}
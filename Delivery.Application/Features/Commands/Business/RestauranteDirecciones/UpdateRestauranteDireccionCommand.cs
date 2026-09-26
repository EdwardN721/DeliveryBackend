using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Business.RestauranteDirecciones;

public record UpdateRestauranteDireccionCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid RestauranteId { get; init; }
}
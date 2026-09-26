using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public record CreateUsuarioDireccionCommand : IRequest<Result<Guid>>
{
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid UsuarioId { get; init; }
}

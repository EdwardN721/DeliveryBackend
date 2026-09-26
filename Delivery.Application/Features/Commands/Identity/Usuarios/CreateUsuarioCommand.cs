using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public record CreateUsuarioCommand : IRequest<Result<Guid>>
{
    public string Nombres { get; init; } = string.Empty;
    public string PrimerApellido { get; init; } = string.Empty;
    public string? SegundoApellido { get; init; }
    public string Telefono { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public IReadOnlyList<int> RolIds { get; init; } = [];
}

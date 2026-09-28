using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public record UpdateUsuarioCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string? Nombres { get; init; }
    public string? PrimerApellido { get; init; }
    public string? SegundoApellido { get; init; }
    public string? Telefono { get; init; }
    public string? Correo { get; init; }
    public string? Password { get; init; }
    public IReadOnlyList<int>? RolIds { get; init; }
}

using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Usuarios;

public record UsuarioListQuery : IRequest<Result<IEnumerable<UsuarioDto>>>
{
    public string? Correo { get; init; }
    public bool SoloActivos { get; init; }
}

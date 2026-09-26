using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;

public record UsuarioDireccionListQuery : IRequest<Result<IEnumerable<UsuarioDireccionDto>>>
{
    public Guid? UsuarioId { get; init; }
}

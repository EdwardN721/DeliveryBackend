using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;

public record UsuarioDireccionByIdQuery : IRequest<Result<UsuarioDireccionDto>>
{
    public Guid Id { get; init; }
}

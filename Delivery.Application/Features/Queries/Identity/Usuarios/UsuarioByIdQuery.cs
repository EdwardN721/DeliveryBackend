using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Usuarios;

public record UsuarioByIdQuery : IRequest<Result<UsuarioDto>>
{
    public Guid Id { get; init; }
}

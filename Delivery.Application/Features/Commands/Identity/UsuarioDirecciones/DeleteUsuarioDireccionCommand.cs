using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public record DeleteUsuarioDireccionCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

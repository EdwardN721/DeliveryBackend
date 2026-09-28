using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public record DeleteUsuarioCommand : IRequest<Result>
{
    public Guid Id { get; init; }
}

using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public record UpdateRolCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string? Nombre { get; init; }
}

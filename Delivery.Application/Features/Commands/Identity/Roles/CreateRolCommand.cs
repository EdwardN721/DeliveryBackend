using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public record CreateRolCommand : IRequest<Result<int>>
{
    public string Nombre { get; init; } = string.Empty;
}

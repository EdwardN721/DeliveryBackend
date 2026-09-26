using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public record DeleteRolCommand : IRequest<Result>
{
    public int Id { get; init; }
}

using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Roles;

public record RolByIdQuery : IRequest<Result<RolDto>>
{
    public int Id { get; init; }
}

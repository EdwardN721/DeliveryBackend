using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Roles;

public record RolListQuery : IRequest<Result<IEnumerable<RolDto>>>;

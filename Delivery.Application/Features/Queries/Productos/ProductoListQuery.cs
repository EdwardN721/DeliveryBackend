using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Productos;

public record ProductoListQuery : IRequest<Result<IEnumerable<ProductoDto>>>
{ }

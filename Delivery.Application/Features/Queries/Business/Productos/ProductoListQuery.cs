using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Business.Productos;

public record ProductoListQuery : IRequest<Result<IEnumerable<ProductoDto>>>
{ }

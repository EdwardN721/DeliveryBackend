using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Categorias;

public record CategoriaListQuery : IRequest<Result<IEnumerable<CategoriaDto>>>;
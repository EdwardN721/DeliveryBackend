using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Catalog.Categorias;

public record CategoriaByIdQuery : IRequest<Result<CategoriaDto>>
{
    public int Id { get; init;}
}

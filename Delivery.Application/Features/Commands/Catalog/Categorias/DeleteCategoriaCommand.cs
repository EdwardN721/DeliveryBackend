using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Catalog.Categorias;

public record DeleteCategoriaCommand : IRequest<Result>
{
    public int Id { get; init; }
}

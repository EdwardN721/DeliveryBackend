using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Catalog.Categorias;

public record UpdateCategoriaCommand :  IRequest<Result>
{
    public int Id { get; init; }
    public string? Nombre { get; init; }
    public string? Descripcion { get; init; }
}

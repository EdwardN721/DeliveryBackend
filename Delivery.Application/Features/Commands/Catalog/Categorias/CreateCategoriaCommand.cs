using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Catalog.Categorias;

public class CreateCategoriaCommand : IRequest<Result<int>>
{
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
}

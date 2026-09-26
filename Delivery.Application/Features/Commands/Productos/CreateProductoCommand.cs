using MediatR;
using Delivery.Core.Result;

namespace Delivery.Application.Features.Commands.Productos;

public record CreateProductoCommand : IRequest<Result<Guid>>
{
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public decimal Precio { get; init; }
    public Guid RestauranteId { get; init; }
    public int CategoriaId { get; init; }
}
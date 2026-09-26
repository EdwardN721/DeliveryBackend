using Delivery.Core.Result;
using MediatR;

namespace Delivery.Application.Features.Commands.Business.Productos;

public record UpdateProductoCommand : IRequest<Result>
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string? Descripcion { get; init; }
    public decimal Precio { get; init; }
    public Guid RestauranteId { get; init; }
    public int CategoriaId { get; init; }
}

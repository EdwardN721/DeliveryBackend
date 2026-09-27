namespace Delivery.Application.Dto.Response;

public record DetallePedidoDto
{
    public Guid Id { get; init; }
    public Guid ProductoId { get; init; }
    public string NombreProducto { get; init; } = string.Empty;
    public int Cantidad { get; init; }
    public decimal PrecioUnitario { get; init; }
    public decimal Impuesto { get; init; }
    public decimal Descuento { get; init; }

    public decimal SubTotalLinea => (Cantidad * PrecioUnitario) + Impuesto - Descuento;
}

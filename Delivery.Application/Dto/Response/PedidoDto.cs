namespace Delivery.Application.Dto.Response;

public record PedidoDto
{
    public Guid Id { get; init; }
    public DateTimeOffset FechaCreacion { get; init; }

    // Datos pedido
    public decimal SubTotal { get; init; }
    public decimal CostoEnvio { get; init; }
    public decimal Total { get; init; }
    public string EstadoPedido { get; init; } = string.Empty;

    // Datos usuario
    public string NombreUsuario { get; init; } = string.Empty;
    public string DireccionUsuario { get; init; } = string.Empty;

    // Datos restaurante
    public string NombreRestaurante { get; init; } = string.Empty;
    
    public IReadOnlyList<DetallePedidoDto> Detalles { get; init; } = [];
}

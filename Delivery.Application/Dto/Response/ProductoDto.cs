namespace Delivery.Application.Dto.Response;

public record ProductoDto
{
    public Guid Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Descripcion { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public Guid RestauranteId { get; init; }
    public string? RestauranteNombre { get; init; }
    public int CategoriaId { get; init; }
    public string? CategoriaNombre { get; init; } 
    public bool EsActivo { get; init; }
}

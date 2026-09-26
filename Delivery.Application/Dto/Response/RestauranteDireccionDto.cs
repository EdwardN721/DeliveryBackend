namespace Delivery.Application.Dto.Response;

public record RestauranteDireccionDto
{
    public Guid Id { get; init; }
    public string Calle { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string Colonia { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CodigoPostal { get; set; } = string.Empty;
    public Guid RestauranteId { get; init; }
    public string? RestauranteNombre { get; init; }
    public bool EsActivo { get; set; } = true;
}
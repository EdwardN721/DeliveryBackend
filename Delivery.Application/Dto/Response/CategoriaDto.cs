namespace Delivery.Application.Dto.Response;

public record CategoriaDto
{
    public int Id { get; init; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool EsActivo { get; set; } = true;
    public bool EsEliminado { get; set; } = false;
}

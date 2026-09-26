namespace Delivery.Application.Dto.Response;

public record UsuarioDireccionDto
{
    public Guid Id { get; init; }
    public string Calle { get; init; } = string.Empty;
    public string Numero { get; init; } = string.Empty;
    public string Colonia { get; init; } = string.Empty;
    public string Ciudad { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
    public string CodigoPostal { get; init; } = string.Empty;
    public Guid UsuarioId { get; init; }
    public string? UsuarioNombre { get; init; }
    public bool EsActivo { get; init; }
}

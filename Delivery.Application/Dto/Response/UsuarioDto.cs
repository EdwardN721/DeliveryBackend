namespace Delivery.Application.Dto.Response;

public record UsuarioDto
{
    public Guid Id { get; init; }
    public string Nombres { get; init; } = string.Empty;
    public string PrimerApellido { get; init; } = string.Empty;
    public string? SegundoApellido { get; init; }
    public string NombreCompleto { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Correo { get; init; } = string.Empty;
    public IReadOnlyList<RolDto> Roles { get; init; } = [];
    public bool EsActivo { get; init; }
    public bool EsEliminado { get; init; }
}

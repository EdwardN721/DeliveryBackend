namespace Delivery.Application.Dto.Response;

public record RolDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
}

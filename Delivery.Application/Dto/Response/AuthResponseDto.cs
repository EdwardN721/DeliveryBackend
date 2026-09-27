namespace Delivery.Application.Dto.Response;

public record AuthResponseDto
{
    public string Token { get; init; } = string.Empty;
}

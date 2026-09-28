using MediatR;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Commands.Identity.Auth;

public record LoginCommand : IRequest<Result<AuthResponseDto>>
{
    public string Correo { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

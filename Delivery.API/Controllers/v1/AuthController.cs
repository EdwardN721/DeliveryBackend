using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.Auth;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador para la autenticación de usuarios.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController(ISender sender, ILogger<AuthController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Inicia sesión y obtiene un token de acceso JWT.
    /// </summary>
    /// <param name="command">Credenciales del usuario.</param>
    /// <returns>Token JWT.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        Result<AuthResponseDto> result = await _sender.Send(command);

        if (result.IsFailure)
        {
            _logger.LogWarning("Intento de login fallido para el correo: {Correo}. Razón: {Error}", command.Correo, result.Error?.Code);
            return BadRequest(result.Error);
        }

        _logger.LogInformation("Login exitoso para el correo: {Correo}", command.Correo);
        return Ok(result.Value);
    }
}

using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Identity.Usuarios;
using Delivery.Application.Features.Commands.Identity.Usuarios;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra los usuarios
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class UsuarioController(ISender sender, ILogger<UsuarioController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<UsuarioController> _logger = logger;

    /// <summary>
    /// Crear Usuario. La contraseña se almacena cifrada con BCrypt.
    /// </summary>
    /// <param name="command">Información para crear un usuario.</param>
    /// <returns>Usuario creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearUsuario([FromBody] CreateUsuarioCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(UsuarioController), nameof(CrearUsuario), result.Value);
        return CreatedAtAction(nameof(ObtenerUsuarioPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener usuario por su Id.
    /// </summary>
    /// <param name="id">Id del usuario.</param>
    /// <returns>Usuario encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerUsuarioPorId([FromRoute] Guid id)
    {
        UsuarioByIdQuery query = new UsuarioByIdQuery { Id = id };
        Result<UsuarioDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(UsuarioController), nameof(ObtenerUsuarioPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener usuarios. Se puede filtrar por correo y por activos.
    /// </summary>
    /// <param name="correo">Correo exacto para filtrar.</param>
    /// <param name="soloActivos">Si es true, solo devuelve usuarios activos.</param>
    /// <returns>Listado de usuarios.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerUsuarios([FromQuery] string? correo, [FromQuery] bool soloActivos = false)
    {
        UsuarioListQuery query = new UsuarioListQuery { Correo = correo, SoloActivos = soloActivos };
        Result<IEnumerable<UsuarioDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(UsuarioController), nameof(ObtenerUsuarios));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar usuario. Si se envía Password, se recalcula el hash. Si se envía RolIds, se sincronizan los roles.
    /// </summary>
    /// <param name="id">Id del usuario a actualizar.</param>
    /// <param name="command">Información para actualizar el usuario.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarUsuario([FromRoute] Guid id, [FromBody] UpdateUsuarioCommand command)
    {
        UpdateUsuarioCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(UsuarioController), nameof(ActualizarUsuario), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar lógicamente un usuario por su Id (soft delete).
    /// </summary>
    /// <param name="id">Id del usuario a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarUsuario([FromRoute] Guid id)
    {
        DeleteUsuarioCommand command = new DeleteUsuarioCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(UsuarioController), nameof(EliminarUsuario), id);
        return NoContent();
    }
}

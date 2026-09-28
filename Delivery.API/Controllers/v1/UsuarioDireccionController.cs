using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;
using Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra las direcciones de los usuarios
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class UsuarioDireccionController(ISender sender, ILogger<UsuarioDireccionController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<UsuarioDireccionController> _logger = logger;

    /// <summary>
    /// Crear dirección de usuario.
    /// </summary>
    /// <param name="command">Información para crear una dirección.</param>
    /// <returns>Dirección creada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearUsuarioDireccion([FromBody] CreateUsuarioDireccionCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(UsuarioDireccionController), nameof(CrearUsuarioDireccion), result.Value);
        return CreatedAtAction(nameof(ObtenerUsuarioDireccionPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener dirección por su Id.
    /// </summary>
    /// <param name="id">Id de la dirección.</param>
    /// <returns>Dirección encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UsuarioDireccionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerUsuarioDireccionPorId([FromRoute] Guid id)
    {
        UsuarioDireccionByIdQuery query = new UsuarioDireccionByIdQuery { Id = id };
        Result<UsuarioDireccionDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(UsuarioDireccionController), nameof(ObtenerUsuarioDireccionPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener direcciones. Se puede filtrar por usuario con el query param usuarioId.
    /// </summary>
    /// <param name="usuarioId">Id opcional del usuario para filtrar.</param>
    /// <returns>Listado de direcciones.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UsuarioDireccionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerUsuarioDirecciones([FromQuery] Guid? usuarioId)
    {
        UsuarioDireccionListQuery query = new UsuarioDireccionListQuery { UsuarioId = usuarioId };
        Result<IEnumerable<UsuarioDireccionDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(UsuarioDireccionController), nameof(ObtenerUsuarioDirecciones));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar dirección de usuario.
    /// </summary>
    /// <param name="id">Id de la dirección a actualizar.</param>
    /// <param name="command">Información para actualizar la dirección.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarUsuarioDireccion([FromRoute] Guid id, [FromBody] UpdateUsuarioDireccionCommand command)
    {
        UpdateUsuarioDireccionCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(UsuarioDireccionController), nameof(ActualizarUsuarioDireccion), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar una dirección por su Id.
    /// </summary>
    /// <param name="id">Id de la dirección a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarUsuarioDireccion([FromRoute] Guid id)
    {
        DeleteUsuarioDireccionCommand command = new DeleteUsuarioDireccionCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(UsuarioDireccionController), nameof(EliminarUsuarioDireccion), id);
        return NoContent();
    }
}

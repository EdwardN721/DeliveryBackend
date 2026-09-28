using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Identity.Roles;
using Delivery.Application.Features.Commands.Identity.Roles;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra los roles
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class RolController(ISender sender, ILogger<RolController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<RolController> _logger = logger;

    /// <summary>
    /// Crear Rol.
    /// </summary>
    /// <param name="command">Información para crear un rol.</param>
    /// <returns>Rol creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearRol([FromBody] CreateRolCommand command)
    {
        Result<int> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(RolController), nameof(CrearRol), result.Value);
        return CreatedAtAction(nameof(ObtenerRolPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener rol por su Id.
    /// </summary>
    /// <param name="id">Id del rol.</param>
    /// <returns>Rol encontrado.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RolDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerRolPorId([FromRoute] int id)
    {
        RolByIdQuery query = new RolByIdQuery { Id = id };
        Result<RolDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(RolController), nameof(ObtenerRolPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener todos los roles.
    /// </summary>
    /// <returns>Listado de roles.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RolDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerRoles()
    {
        RolListQuery query = new RolListQuery();
        Result<IEnumerable<RolDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(RolController), nameof(ObtenerRoles));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar rol.
    /// </summary>
    /// <param name="id">Id del rol a actualizar.</param>
    /// <param name="command">Información para actualizar el rol.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarRol([FromRoute] int id, [FromBody] UpdateRolCommand command)
    {
        UpdateRolCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(RolController), nameof(ActualizarRol), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un rol por su Id. No se permite si tiene usuarios asignados.
    /// </summary>
    /// <param name="id">Id del rol a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarRol([FromRoute] int id)
    {
        DeleteRolCommand command = new DeleteRolCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(RolController), nameof(EliminarRol), id);
        return NoContent();
    }
}

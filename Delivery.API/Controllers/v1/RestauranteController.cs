using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Business.Restaurantes;
using Delivery.Application.Features.Commands.Business.Restaurantes;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra los restaurantes
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class RestauranteController(ISender sender, ILogger<RestauranteController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<RestauranteController> _logger = logger;

    /// <summary>
    /// Crear restaurante.
    /// </summary>
    /// <param name="command">Información para crear un restaurante.</param>
    /// <returns>Restaurante creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearRestaurante([FromBody] CreateRestauranteCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(RestauranteController), nameof(CrearRestaurante), result.Value);
        return CreatedAtAction(nameof(ObtenerRestaurantePorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener restaurante por su Id.
    /// </summary>
    /// <param name="id">Id del restaurante.</param>
    /// <returns>Restaurante encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RestauranteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerRestaurantePorId([FromRoute] Guid id)
    {
        RestauranteByIdQuery query = new RestauranteByIdQuery { Id = id };
        Result<RestauranteDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(RestauranteController), nameof(ObtenerRestaurantePorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener todos los restaurantes.
    /// </summary>
    /// <returns>Listado de restaurantes.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RestauranteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerRestaurantes()
    {
        RestauranteListQuery query = new RestauranteListQuery();
        Result<IEnumerable<RestauranteDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(RestauranteController), nameof(ObtenerRestaurantes));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar restaurante.
    /// </summary>
    /// <param name="id">Id del restaurante a actualizar.</param>
    /// <param name="command">Información para actualizar restaurante.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarRestaurante([FromRoute] Guid id, [FromBody] UpdateRestauranteCommand command)
    {
        UpdateRestauranteCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(RestauranteController), nameof(ActualizarRestaurante), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar un restaurante por su Id.
    /// </summary>
    /// <param name="id">Id del restaurante a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarRestaurante([FromRoute] Guid id)
    {
        DeleteRestauranteCommand command = new DeleteRestauranteCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(RestauranteController), nameof(EliminarRestaurante), id);
        return NoContent();
    }
}
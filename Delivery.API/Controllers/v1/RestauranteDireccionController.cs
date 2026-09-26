using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.RestauranteDirecciones;
using Delivery.Application.Features.Commands.RestauranteDirecciones;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra las direcciones de los restaurantes
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class RestauranteDireccionController(ISender sender, ILogger<RestauranteDireccionController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<RestauranteDireccionController> _logger = logger;

    /// <summary>
    /// Crear dirección de restaurante.
    /// </summary>
    /// <param name="command">Información para crear una dirección.</param>
    /// <returns>Dirección creada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearRestauranteDireccion([FromBody] CreateRestauranteDireccionCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(RestauranteDireccionController), nameof(CrearRestauranteDireccion), result.Value);
        return CreatedAtAction(nameof(ObtenerRestauranteDireccionPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener dirección por su Id.
    /// </summary>
    /// <param name="id">Id de la dirección.</param>
    /// <returns>Dirección encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RestauranteDireccionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerRestauranteDireccionPorId([FromRoute] Guid id)
    {
        RestauranteDireccionByIdQuery query = new RestauranteDireccionByIdQuery { Id = id };
        Result<RestauranteDireccionDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(RestauranteDireccionController), nameof(ObtenerRestauranteDireccionPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener direcciones. Se puede filtrar por restaurante con el query param restauranteId.
    /// </summary>
    /// <param name="restauranteId">Id opcional del restaurante para filtrar.</param>
    /// <returns>Listado de direcciones.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RestauranteDireccionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerRestauranteDirecciones([FromQuery] Guid? restauranteId)
    {
        RestauranteDireccionListQuery query = new RestauranteDireccionListQuery { RestauranteId = restauranteId };
        Result<IEnumerable<RestauranteDireccionDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(RestauranteDireccionController), nameof(ObtenerRestauranteDirecciones));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar dirección de restaurante.
    /// </summary>
    /// <param name="id">Id de la dirección a actualizar.</param>
    /// <param name="command">Información para actualizar la dirección.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarRestauranteDireccion([FromRoute] Guid id, [FromBody] UpdateRestauranteDireccionCommand command)
    {
        UpdateRestauranteDireccionCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(RestauranteDireccionController), nameof(ActualizarRestauranteDireccion), id);
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
    public async Task<IActionResult> EliminarRestauranteDireccion([FromRoute] Guid id)
    {
        DeleteRestauranteDireccionCommand command = new DeleteRestauranteDireccionCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(RestauranteDireccionController), nameof(EliminarRestauranteDireccion), id);
        return NoContent();
    }
}
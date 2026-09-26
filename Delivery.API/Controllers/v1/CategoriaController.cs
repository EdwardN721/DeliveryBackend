using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Categorias;
using Delivery.Application.Features.Commands.Categorias;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra las categorias
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/[controller]")]
public class CategoriaController(ISender sender, ILogger<CategoriaController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<CategoriaController> _logger = logger;

    /// <summary>
    /// Crear Categoria.
    /// </summary>
    /// <param name="command">Información para crear una categoria.</param>
    /// <returns>Categoria creada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearCategoria([FromBody] CreateCategoriaCommand command)
    {
        Result<int> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(CategoriaController), nameof(CrearCategoria), result.Value);
        return CreatedAtAction(nameof(ObtenerCategoriaPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtener categoria por su Id.
    /// </summary>
    /// <param name="id">Id de la categoria.</param>
    /// <returns>Categoria encontrada.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerCategoriaPorId([FromRoute] int id)
    {
        CategoriaByIdQuery query = new CategoriaByIdQuery { Id = id };
        Result<CategoriaDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);
        
        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(CategoriaController), nameof(ObtenerCategoriaPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtener todas las categorias.
    /// </summary>
    /// <returns>Listado de categorias.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerCategorias()
    {
        CategoriaListQuery query = new CategoriaListQuery();
        Result<IEnumerable<CategoriaDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(CategoriaController), nameof(ObtenerCategorias));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar categoria.
    /// </summary>
    /// <param name="id">Id de la categoria a actualizar.</param>
    /// <param name="command">Información para actualizar categoria.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarCategoria([FromRoute] int id, [FromBody] UpdateCategoriaCommand command)
    {
        UpdateCategoriaCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();
        
        _logger.LogInformation("{Controlador} - {Operacion} - Éxito actualizando Id: {Id}", nameof(CategoriaController), nameof(ActualizarCategoria), id);
        return NoContent();
    }

    /// <summary>
    /// Eliminar una categoria por su Id.
    /// </summary>
    /// <param name="id">Id de la categoria a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarCategoria([FromRoute] int id)
    {
        DeleteCategoriaCommand query = new DeleteCategoriaCommand { Id = id};
        Result result = await _sender.Send(query);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito eliminando Id: {Id}", nameof(CategoriaController), nameof(EliminarCategoria), id);
        return NoContent();
    }

}
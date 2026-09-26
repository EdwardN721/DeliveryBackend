using MediatR;
using Asp.Versioning;
using Delivery.Extensions;
using Microsoft.AspNetCore.Mvc;
using Delivery.Application.Features.Commands.Productos;
using Delivery.Core.Result;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Queries.Productos;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador que administra los productos del catalogo
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductoController(ISender sender, ILogger<ProductoController> logger) : ControllerBase
{
    private readonly ISender _sender = sender;
    private readonly ILogger<ProductoController> _logger = logger;

    /// <summary>
    /// Crear un nuevo producto
    /// </summary>
    /// <param name="command">Información del producto a crear.</param>
    /// <returns>El Id del producto creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearProducto([FromBody] CreateProductoCommand command)
    {
        Result<Guid> result = await _sender.Send(command);

        if (result.IsFailure) return BadRequest(result.Error);
        
        _logger.LogInformation("{Controlador} - {Operacion} - Éxito con Id: {Id}", nameof(ProductoController), nameof(CrearProducto), result.Value);
        return CreatedAtAction(nameof(ObtenerProductoPorId), new { id = result.Value }, result.Value);
    }

    /// <summary>
    /// Obtiene un producto por su Id.
    /// </summary>
    /// <param name="id">Id del producto.</param>
    /// <returns>Producto encontrado con su categoría.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerProductoPorId([FromRoute] Guid id)
    {
        ProductoPorIdQuery query = new ProductoPorIdQuery { Id = id };
        Result<ProductoDto> result = await _sender.Send(query);

        if (result.IsFailure) return NotFound(result.Error);
        
        _logger.LogInformation("{Controlador} - {Operacion} - Éxito consultando Id: {Id}", nameof(ProductoController), nameof(ObtenerProductoPorId), id);
        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene el catálogo completo de productos.
    /// </summary>
    /// <returns>Listado de productos con sus categorías.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerProductos()
    {
        ProductoListQuery query = new ProductoListQuery();
        Result<IEnumerable<ProductoDto>> result = await _sender.Send(query);

        if (result.IsFailure) return BadRequest(result.Error);

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(ProductoController), nameof(ObtenerProductos));
        return Ok(result.Value);
    }

    /// <summary>
    /// Actualizar un producto.
    /// </summary>
    /// <param name="id">Id del producto a actualizar</param>
    /// <param name="command">Información a actualizar.</param>
    /// <returns>Estado de la actualización.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarProducto([FromRoute] Guid id, [FromBody] UpdateProductoCommand command)
    {
        UpdateProductoCommand commandSeguro = command with { Id = id };

        Result result = await _sender.Send(commandSeguro);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(ProductoController), nameof(ActualizarProducto));
        return NoContent();
    }

    /// <summary>
    /// Eliminar producto.
    /// </summary>
    /// <param name="id">Id del producto a eliminar.</param>
    /// <returns>Estado de la eliminación.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarProducto([FromRoute] Guid id)
    {
        DeleteProductoCommand command = new DeleteProductoCommand { Id = id };
        Result result = await _sender.Send(command);

        if (result.IsFailure) return result.ToErrorActionResult();

        _logger.LogInformation("{Controlador} - {Operacion} - Éxito", nameof(ProductoController), nameof(EliminarProducto));
        return NoContent();
    }
}

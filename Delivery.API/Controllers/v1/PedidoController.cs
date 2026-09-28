using MediatR;
using Asp.Versioning;
using Delivery.Core.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Delivery.Application.Features.Commands.Transaction.Pedidos;
using Delivery.Application.Features.Queries.Transaction.Pedidos;
using Delivery.Application.Dto.Response;
using System.Text.Json;
using Delivery.Core.Pagination;

namespace Delivery.Controllers.v1;

/// <summary>
/// Controlador para la gestión transaccional de Pedidos.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class PedidoController(ISender sender) : ControllerBase
{
    /// <summary>
    /// Crea un nuevo pedido a partir de un carrito de compras.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreatePedidoCommand command)
    {
        Result<Guid> result = await sender.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Avanza o retrocede la logística del pedido (solo uso interno/negocio).
    /// </summary>
    [HttpPut("estado")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CambiarEstado([FromBody] CambiarEstadoPedidoCommand command)
    {
        Result result = await sender.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    /// <summary>
    /// Cancela un pedido existente.
    /// </summary>
    [HttpPut("cancelar")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancelar([FromBody] CancelarPedidoCommand command)
    {
        Result result = await sender.Send(command);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }

    /// <summary>
    /// Obtiene el historial de pedidos del usuario autenticado de forma paginada y filtrada.
    /// </summary>
    [HttpGet("mis-pedidos")]
    [ProducesResponseType(typeof(IEnumerable<PedidoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMisPedidos([FromQuery] GetMisPedidosQuery query)
    {
        Result<PagedList<PedidoDto>> result = await sender.Send(query);

        if (result.IsFailure)
            return BadRequest(result.Error);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.Value?.MetaData));

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene el detalle exacto de un ticket de compra.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id)
    {
        Result<PedidoDto> result = await sender.Send(new GetPedidoByIdQuery(id));

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}

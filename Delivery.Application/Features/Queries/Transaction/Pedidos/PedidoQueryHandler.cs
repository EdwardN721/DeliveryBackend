using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Application.Dto.Response;
using Delivery.Core.Entities.Transaction;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Application.Features.Queries.Transaction.Pedidos;

public class PedidoQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) :
    IRequestHandler<GetPedidoByIdQuery, Result<PedidoDto>>,
    IRequestHandler<GetMisPedidosQuery, Result<IEnumerable<PedidoDto>>>
{
    public async Task<Result<PedidoDto>> Handle(GetPedidoByIdQuery request, CancellationToken cancellationToken = default)
    {
        // Consultamos el pedido con todas sus relaciones
        Pedido? pedido = await unitOfWork.Pedidos.FirstOrDefaultAsync(
            predicate: p => p.Id == request.Id,
            include: query => query
                .Include(p => p.Cliente)
                .Include(p => p.Restaurante)
                .Include(p => p.DireccionEntrega)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto), // Segundo nivel
            disableTracking: true,
            cancellationToken: cancellationToken);

        if (pedido is null)
            return Result<PedidoDto>.Failure(new ErrorResult("Pedido.NotFound", "El pedido no existe."));

        if (pedido.UsuarioId.ToString() != currentUserService.UserId)
            return Result<PedidoDto>.Failure(new ErrorResult("Pedido.Unauthorized", "No tienes permiso para ver este ticket."));

        return Result<PedidoDto>.Success(pedido.MapToDto());
    }

    public async Task<Result<IEnumerable<PedidoDto>>> Handle(GetMisPedidosQuery request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(currentUserService.UserId, out Guid usuarioId))
            return Result<IEnumerable<PedidoDto>>.Failure(new ErrorResult("Auth.Unauthorized", "Usuario no identificado."));

        // Traemos todo el historial del usuario logueado
        IEnumerable<Pedido>? pedidos = await unitOfWork.Pedidos.GetAsync(
            predicate: p => p.UsuarioId == usuarioId,
            include: query => query
                .Include(p => p.Cliente)
                .Include(p => p.Restaurante)
                .Include(p => p.DireccionEntrega)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto), 
            disableTracking: true,
            cancellationToken: cancellationToken);

        // Ordenamos del más reciente al más antiguo antes de mapear
        var pedidosOrdenados = pedidos.OrderByDescending(p => p.FechaCreacion);

        return Result<IEnumerable<PedidoDto>>.Success(pedidosOrdenados.MapToDto());
    }
}
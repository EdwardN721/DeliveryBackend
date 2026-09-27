using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Transaction.Pedidos;

public class PedidoQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) :
    IRequestHandler<GetPedidoByIdQuery, Result<PedidoDto>>,
    IRequestHandler<GetMisPedidosQuery, Result<IEnumerable<PedidoDto>>>
{
    public async Task<Result<PedidoDto>> Handle(GetPedidoByIdQuery request, CancellationToken cancellationToken = default)
    {
        // 1. Consultamos el pedido con todas sus relaciones
        var pedido = await unitOfWork.Pedidos.FirstOrDefaultAsync(
            predicate: p => p.Id == request.Id,
            disableTracking: true, // Lectura pura, súper rápido
            cancellationToken: cancellationToken,
            includes: [
                p => p.Cliente, 
                p => p.Restaurante, 
                p => p.DireccionEntrega, 
                p => p.EstadoPedido, 
                p => p.Detalles
            ]);

        if (pedido is null)
            return Result<PedidoDto>.Failure(new ErrorResult("Pedido.NotFound", "El pedido no existe."));

        // 2. Seguridad de Tenencia (Multi-tenant data): Evitar que Miguel vea el pedido de Juan
        if (pedido.UsuarioId.ToString() != currentUserService.UserId)
            return Result<PedidoDto>.Failure(new ErrorResult("Pedido.Unauthorized", "No tienes permiso para ver este ticket."));

        return Result<PedidoDto>.Success(pedido.MapToDto());
    }

    public async Task<Result<IEnumerable<PedidoDto>>> Handle(GetMisPedidosQuery request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(currentUserService.UserId, out Guid usuarioId))
            return Result<IEnumerable<PedidoDto>>.Failure(new ErrorResult("Auth.Unauthorized", "Usuario no identificado."));

        // 1. Traemos todo el historial del usuario logueado
        var pedidos = await unitOfWork.Pedidos.GetAsync(
            predicate: p => p.UsuarioId == usuarioId,
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: [
                p => p.Cliente, 
                p => p.Restaurante, 
                p => p.DireccionEntrega, 
                p => p.EstadoPedido, 
                p => p.Detalles
            ]);

        // Ordenamos del más reciente al más antiguo antes de mapear
        var pedidosOrdenados = pedidos.OrderByDescending(p => p.FechaCreacion);

        return Result<IEnumerable<PedidoDto>>.Success(pedidosOrdenados.MapToDto());
    }
}
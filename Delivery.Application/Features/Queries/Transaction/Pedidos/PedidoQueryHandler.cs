using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Application.Dto.Response;
using Delivery.Core.Entities.Transaction;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Pagination;

namespace Delivery.Application.Features.Queries.Transaction.Pedidos;

public class PedidoQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) :
    IRequestHandler<GetPedidoByIdQuery, Result<PedidoDto>>,
    IRequestHandler<GetMisPedidosQuery, Result<PagedList<PedidoDto>>>
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

    public async Task<Result<PagedList<PedidoDto>>> Handle(GetMisPedidosQuery request, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(currentUserService.UserId, out Guid usuarioId))
            return Result<PagedList<PedidoDto>>.Failure(new ErrorResult("Auth.Unauthorized", "Usuario no identificado."));

        // Construimos el filtro dinámico combinando todos los parámetros
        System.Linq.Expressions.Expression<Func<Pedido, bool>> filtro = p => 
            p.UsuarioId == usuarioId &&
            (!request.EstadoPedidoId.HasValue || p.EstadoPedidoId == request.EstadoPedidoId) &&
            (!request.FechaInicio.HasValue || p.FechaCreacion >= request.FechaInicio) &&
            (!request.FechaFin.HasValue || p.FechaCreacion <= request.FechaFin);

        // Ejecutamos la consulta paginada
        (IEnumerable<Pedido> pedidos, int totalCount) = await unitOfWork.Pedidos.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: filtro,
            orderBy: query => query.OrderByDescending(p => p.FechaCreacion),
            include: query => query
                .Include(p => p.Cliente)
                .Include(p => p.Restaurante)
                .Include(p => p.DireccionEntrega)
                .Include(p => p.EstadoPedido)
                .Include(p => p.Detalles).ThenInclude(d => d.Producto),
            disableTracking: true,
            cancellationToken: cancellationToken);

        // Mapeamos y empaquetamos en la lista paginada
        List<PedidoDto> pedidosDto = pedidos.MapToDto().ToList(); 
        PagedList<PedidoDto> pagedList = new PagedList<PedidoDto>(pedidosDto, totalCount, request.PageNumber, request.PageSize);

        return Result<PagedList<PedidoDto>>.Success(pagedList);
    }
}
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Constants;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;
using Delivery.Core.Entities.Transaction;

namespace Delivery.Application.Features.Commands.Transaction.Pedidos;

public class PedidoCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) :
    IRequestHandler<CreatePedidoCommand, Result<Guid>>,
    IRequestHandler<CambiarEstadoPedidoCommand, Result>,
    IRequestHandler<CancelarPedidoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUserService _currentUserService = currentUserService;

    public async Task<Result<Guid>> Handle(CreatePedidoCommand command, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(_currentUserService.UserId, out Guid usuarioId))
        {
            return Result<Guid>.Failure(new ErrorResult("Auth.Unauthorized", "Usuario no identificado."));
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            Pedido pedido = command.MapToEntity();
            pedido.UsuarioId = usuarioId;
            pedido.EstadoPedidoId = EstadosPedido.Pendiente;

            List<Guid> productosIds = command.Carrito.Select(c => c.ProductoId).ToList();

            IEnumerable<Producto> productosDbResult = await _unitOfWork.Productos.GetAsync(
                predicate: p => productosIds.Contains(p.Id),
                disableTracking: true,
                cancellationToken: cancellationToken
            );

            List<Producto> productosDb = productosDbResult.ToList();

            if (productosDb.Count != productosIds.Count)
            {
                return Result<Guid>.Failure(new ErrorResult("Pedido.ProductosInvalidos", "Uno o más productos del carrito no existen."));
            }

            decimal subTotalCalculado = 0;

            foreach (PedidoDetalleCommand item in command.Carrito)
            {
                Producto productoReal = productosDb.First(p => p.Id == item.ProductoId);

                PedidoDetalle detalle = PedidoDetalleMapper.MapToEntity(productoReal.Id, item.Cantidad, productoReal.Precio);
                
                // Cálculo de IVA (16%)
                detalle.Impuesto = (detalle.PrecioUnitario * 0.16m) * detalle.Cantidad;

                subTotalCalculado += (detalle.PrecioUnitario * detalle.Cantidad) + detalle.Impuesto;
                pedido.Detalles.Add(detalle);
            }

            pedido.SubTotal = subTotalCalculado;
            pedido.CostoEnvio = 35.00m;
            pedido.Total = pedido.SubTotal + pedido.CostoEnvio;

            await _unitOfWork.Pedidos.AddAsync(pedido);
            await _unitOfWork.CommitAsync();

            return Result<Guid>.Success(pedido.Id);
        }
        catch (Exception) 
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result<Guid>.Failure(new ErrorResult("Pedido.ErrorInterno", "Ocurrió un error al crear el pedido."));
        }
    }

   public async Task<Result> Handle(CambiarEstadoPedidoCommand command, CancellationToken cancellationToken = default)
    {
        Pedido? pedido = await _unitOfWork.Pedidos.GetByIdAsync(command.IdPedido, false, cancellationToken);
        
        if (pedido is null)
            return Result.Failure(new ErrorResult("Pedido.NotFound", "El pedido no existe."));

        pedido.EstadoPedidoId = command.NuevoEstadoId;
        
        _unitOfWork.Pedidos.Update(pedido);
        await _unitOfWork.CommitAsync(); 

        return Result.Success();
    }

    public async Task<Result> Handle(CancelarPedidoCommand command, CancellationToken cancellationToken = default)
    {
        Pedido? pedido = await _unitOfWork.Pedidos.GetByIdAsync(command.IdPedido, false, cancellationToken);
        
        if (pedido is null)
            return Result.Failure(new ErrorResult("Pedido.NotFound", "El pedido no existe."));

        pedido.EstadoPedidoId = EstadosPedido.Cancelado; 
        
        _unitOfWork.Pedidos.Update(pedido);
        await _unitOfWork.CommitAsync(); 

        return Result.Success();
    }
}
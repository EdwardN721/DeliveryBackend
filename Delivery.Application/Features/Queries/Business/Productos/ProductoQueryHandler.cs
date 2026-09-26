using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;
using Delivery.Application.Dto.Response;
using System.Linq.Expressions;

namespace Delivery.Application.Features.Queries.Business.Productos;

public class ProductoQueryHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<ProductoPorIdQuery, Result<ProductoDto>>,
    IRequestHandler<ProductoListQuery, Result<IEnumerable<ProductoDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ProductoDto>> Handle(ProductoPorIdQuery request, CancellationToken cancellationToken = default)
    {
        Producto? producto = await _unitOfWork.Productos.FirstOrDefaultAsync
            (
                predicate: p => p.Id == request.Id, 
                disableTracking: false, 
                cancellationToken: cancellationToken, 
                includes: new Expression<Func<Producto, object>>[] { c => c.Categoria, r => r.Restaurante });

        if (producto == null) 
            return Result<ProductoDto>.Failure(new ErrorResult("Producto.NotFound", $"No se encontró el producto con el Id: {request.Id}"));

        return Result<ProductoDto>.Success(producto.MapToDto());
    }

    public async Task<Result<IEnumerable<ProductoDto>>> Handle(ProductoListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Producto> productos = await _unitOfWork.Productos.GetAllAsync(
            disableTracking: false, 
            cancellationToken: cancellationToken,
            includeProperties: new Expression<Func<Producto, object>>[] { c => c.Categoria, r => r.Restaurante }
        );

        return Result<IEnumerable<ProductoDto>>.Success(productos.MapToDto());
    }
}

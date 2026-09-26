using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Catalog;
using Delivery.Core.Entities.Business;

namespace Delivery.Application.Features.Commands.Productos;

public class ProductoCommandHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<CreateProductoCommand, Result<Guid>>,
    IRequestHandler<UpdateProductoCommand, Result>,
    IRequestHandler<DeleteProductoCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateProductoCommand request, CancellationToken cancellationToken = default)
    {
        Restaurante? restaurante = await _unitOfWork.Restaurantes.GetByIdAsync(request.RestauranteId, true, cancellationToken);

        if (restaurante == null)
        {
            return Result<Guid>.Failure(new ErrorResult("Producto.RestauranteInvalido",
                $"No se encontro el restaurante con el Id: {request.RestauranteId}"));
        }

        Categoria? categoria = await _unitOfWork.Categorias.GetByIdAsync(request.CategoriaId, true, cancellationToken);

        if (categoria == null)
        {
            return Result<Guid>.Failure(new ErrorResult("Producto.CategoriaInvalida",
                $"No se encontro la categoria con el Id: {request.CategoriaId}"));
        }

        Producto producto = request.MapToEntity();

        await _unitOfWork.Productos.AddAsync(producto, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(producto.Id);
    }

    public async Task<Result> Handle(UpdateProductoCommand request, CancellationToken cancellationToken = default)
    {
        Producto? producto = await _unitOfWork.Productos.GetByIdAsync(request.Id, false, cancellationToken);
        if (producto == null) return Result.Failure(new ErrorResult("Producto.NotFound", 
                $"El Producto con el Id: {request.Id} no se encotró."));

        Restaurante? restaurante = await _unitOfWork.Restaurantes.GetByIdAsync(request.RestauranteId, true, cancellationToken);
        if (restaurante == null) return Result.Failure(new ErrorResult("Producto.RestauranteInvalido", 
                $"No se encontro el restaurante con el Id: {request.RestauranteId}"));

        Categoria? categoria = await _unitOfWork.Categorias.GetByIdAsync(request.CategoriaId, true, cancellationToken);
        if (categoria == null) return Result.Failure(new ErrorResult("Producto.CategoriaInvalida", 
                $"No se encontro la categoria con el Id: {request.CategoriaId}"));

        producto.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteProductoCommand request, CancellationToken cancellationToken = default)
    {
        Producto? producto = await _unitOfWork.Productos.GetByIdAsync(request.Id, false, cancellationToken);
        if (producto == null) return Result.Failure(new ErrorResult("Producto.NotFound", 
                $"El Producto con el Id: {request.Id} no se encotró."));

        _unitOfWork.Productos.Delete(producto);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }
}

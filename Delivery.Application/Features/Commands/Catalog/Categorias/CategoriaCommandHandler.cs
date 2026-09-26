using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Catalog;

namespace Delivery.Application.Features.Commands.Catalog.Categorias;

public class CategoriaCommandHandler(IUnitOfWork unitOfWork) : 
    IRequestHandler<CreateCategoriaCommand, Result<int>>,
    IRequestHandler<UpdateCategoriaCommand, Result>,
    IRequestHandler<DeleteCategoriaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<int>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken = default)
    {
        Categoria categoria = request.MapToEntity();
        await _unitOfWork.Categorias.AddAsync(categoria, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return Result<int>.Success(categoria.Id);
    }

    public async Task<Result> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken = default)
    {
        Categoria? categoria = await BuscarCategoriaPorId(request.Id, cancellationToken);

        if (categoria == null)
        {
            return Result.Failure(new ErrorResult("Categoria.NotFound", $"No se encontró la categoria con el Id: {request.Id}"));
        }

        categoria.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken = default)
    {
        Categoria? categoria = await BuscarCategoriaPorId(request.Id, cancellationToken);

        if (categoria == null)
        {
            return Result.Failure(new ErrorResult("Categoria.NotFound", $"No se encontró la categoria con el Id: {request.Id}"));
        }

        _unitOfWork.Categorias.Delete(categoria);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<Categoria?> BuscarCategoriaPorId(int id, CancellationToken cancellationToken = default)
    {
        Categoria? categoria = await _unitOfWork.Categorias.GetByIdAsync(id, false, cancellationToken);

        return categoria;
    }

    #endregion
}

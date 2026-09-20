using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Catalog;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Categorias;

public class CategoriaQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CategoriaByIdQuery, Result<CategoriaDto>>,
    IRequestHandler<CategoriaListQuery, Result<IEnumerable<CategoriaDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CategoriaDto>> Handle(CategoriaByIdQuery request, CancellationToken cancellationToken = default)
    {
        Categoria? categoria = await _unitOfWork.Categorias.GetByIdAsync(request.Id, true, cancellationToken);

        if (categoria == null)
        {
            return Result<CategoriaDto>.Failure(new ErrorResult("Categoria.NotFound", $"No se encontró la categoría con Id {request.Id}"));
        }

        return Result<CategoriaDto>.Success(categoria.MapToDto());
    }

    public async Task<Result<IEnumerable<CategoriaDto>>> Handle(CategoriaListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Categoria> categorias = await _unitOfWork.Categorias.GetAllAsync(true, cancellationToken);
        return Result<IEnumerable<CategoriaDto>>.Success(categorias.MapToDto());
    }
}

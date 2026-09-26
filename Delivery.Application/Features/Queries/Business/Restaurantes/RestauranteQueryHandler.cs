using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Business.Restaurantes;

public class RestauranteQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<RestauranteByIdQuery, Result<RestauranteDto>>,
    IRequestHandler<RestauranteListQuery, Result<IEnumerable<RestauranteDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RestauranteDto>> Handle(RestauranteByIdQuery request, CancellationToken cancellationToken = default)
    {
        Restaurante? restaurante = await _unitOfWork.Restaurantes.GetByIdAsync(request.Id, true, cancellationToken);

        if (restaurante == null)
        {
            return Result<RestauranteDto>.Failure(new ErrorResult("Restaurante.NotFound", $"No se encontró el restaurante con el Id: {request.Id}"));
        }

        return Result<RestauranteDto>.Success(restaurante.MapToDto());
    }

    public async Task<Result<IEnumerable<RestauranteDto>>> Handle(RestauranteListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Restaurante> restaurantes = await _unitOfWork.Restaurantes.GetAllAsync(true, cancellationToken);

        return Result<IEnumerable<RestauranteDto>>.Success(restaurantes.MapToDto());
    }
}
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.RestauranteDirecciones;

public class RestauranteDireccionQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<RestauranteDireccionByIdQuery, Result<RestauranteDireccionDto>>,
    IRequestHandler<RestauranteDireccionListQuery, Result<IEnumerable<RestauranteDireccionDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RestauranteDireccionDto>> Handle(RestauranteDireccionByIdQuery request, CancellationToken cancellationToken = default)
    {
        RestauranteDireccion? direccion = await _unitOfWork.RestauranteDirecciones.FirstOrDefaultAsync(
            predicate: rd => rd.Id == request.Id,
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: rd => rd.Restaurante);

        if (direccion == null)
        {
            return Result<RestauranteDireccionDto>.Failure(new ErrorResult("RestauranteDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        return Result<RestauranteDireccionDto>.Success(direccion.MapToDto());
    }

    public async Task<Result<IEnumerable<RestauranteDireccionDto>>> Handle(RestauranteDireccionListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<RestauranteDireccion> direcciones = request.RestauranteId is null
            ? await _unitOfWork.RestauranteDirecciones.GetAllAsync(true, cancellationToken, rd => rd.Restaurante)
            : await _unitOfWork.RestauranteDirecciones.GetAsync(
                predicate: rd => rd.RestauranteId == request.RestauranteId.Value,
                disableTracking: true,
                cancellationToken: cancellationToken,
                includes: rd => rd.Restaurante);

        return Result<IEnumerable<RestauranteDireccionDto>>.Success(direcciones.MapToDto());
    }
}
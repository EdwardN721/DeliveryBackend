using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;

namespace Delivery.Application.Features.Commands.Business.RestauranteDirecciones;

public class RestauranteDireccionCommandHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CreateRestauranteDireccionCommand, Result<Guid>>,
    IRequestHandler<UpdateRestauranteDireccionCommand, Result>,
    IRequestHandler<DeleteRestauranteDireccionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateRestauranteDireccionCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionRestaurante = await ValidarRestauranteExiste(request.RestauranteId, cancellationToken);

        if (validacionRestaurante.IsFailure)
        {
            return Result<Guid>.Failure(validacionRestaurante.Error);
        }

        RestauranteDireccion direccion = request.MapToEntity();

        await _unitOfWork.RestauranteDirecciones.AddAsync(direccion, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(direccion.Id);
    }

    public async Task<Result> Handle(UpdateRestauranteDireccionCommand request, CancellationToken cancellationToken = default)
    {
        RestauranteDireccion? direccion = await BuscarDireccionPorId(request.Id, cancellationToken);

        if (direccion == null)
        {
            return Result.Failure(new ErrorResult("RestauranteDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        Result validacionRestaurante = await ValidarRestauranteExiste(request.RestauranteId, cancellationToken);

        if (validacionRestaurante.IsFailure)
        {
            return validacionRestaurante;
        }

        direccion.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteRestauranteDireccionCommand request, CancellationToken cancellationToken = default)
    {
        RestauranteDireccion? direccion = await BuscarDireccionPorId(request.Id, cancellationToken);

        if (direccion == null)
        {
            return Result.Failure(new ErrorResult("RestauranteDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        _unitOfWork.RestauranteDirecciones.Delete(direccion);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<RestauranteDireccion?> BuscarDireccionPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.RestauranteDirecciones.GetByIdAsync(id, false, cancellationToken);
    }

    private async Task<Result> ValidarRestauranteExiste(Guid restauranteId, CancellationToken cancellationToken = default)
    {
        bool existe = await _unitOfWork.Restaurantes.AnyAsync(r => r.Id == restauranteId, cancellationToken);

        if (!existe)
        {
            return Result.Failure(new ErrorResult("RestauranteDireccion.RestauranteInvalido",
                $"No se encontró el restaurante con el Id: {restauranteId}"));
        }

        return Result.Success();
    }

    #endregion
}
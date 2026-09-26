using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Business;

namespace Delivery.Application.Features.Commands.Business.Restaurantes;

public class RestauranteCommandHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CreateRestauranteCommand, Result<Guid>>,
    IRequestHandler<UpdateRestauranteCommand, Result>,
    IRequestHandler<DeleteRestauranteCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateRestauranteCommand request, CancellationToken cancellationToken = default)
    {
        Restaurante restaurante = request.MapToEntity();

        await _unitOfWork.Restaurantes.AddAsync(restaurante, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(restaurante.Id);
    }

    public async Task<Result> Handle(UpdateRestauranteCommand request, CancellationToken cancellationToken = default)
    {
        Restaurante? restaurante = await BuscarRestaurantePorId(request.Id, cancellationToken);

        if (restaurante == null)
        {
            return Result.Failure(new ErrorResult("Restaurante.NotFound", $"No se encontró el restaurante con el Id: {request.Id}"));
        }

        restaurante.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteRestauranteCommand request, CancellationToken cancellationToken = default)
    {
        Restaurante? restaurante = await BuscarRestaurantePorId(request.Id, cancellationToken);

        if (restaurante == null)
        {
            return Result.Failure(new ErrorResult("Restaurante.NotFound", $"No se encontró el restaurante con el Id: {request.Id}"));
        }

        _unitOfWork.Restaurantes.Delete(restaurante);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<Restaurante?> BuscarRestaurantePorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Restaurantes.GetByIdAsync(id, false, cancellationToken);
    }

    #endregion
}
using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;

namespace Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

public class UsuarioDireccionCommandHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CreateUsuarioDireccionCommand, Result<Guid>>,
    IRequestHandler<UpdateUsuarioDireccionCommand, Result>,
    IRequestHandler<DeleteUsuarioDireccionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<Guid>> Handle(CreateUsuarioDireccionCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionUsuario = await ValidarUsuarioExiste(request.UsuarioId, cancellationToken);

        if (validacionUsuario.IsFailure)
        {
            return Result<Guid>.Failure(validacionUsuario.Error);
        }

        UsuarioDireccion direccion = request.MapToEntity();

        await _unitOfWork.UsuarioDirecciones.AddAsync(direccion, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(direccion.Id);
    }

    public async Task<Result> Handle(UpdateUsuarioDireccionCommand request, CancellationToken cancellationToken = default)
    {
        UsuarioDireccion? direccion = await BuscarDireccionPorId(request.Id, cancellationToken);

        if (direccion == null)
        {
            return Result.Failure(new ErrorResult("UsuarioDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        Result validacionUsuario = await ValidarUsuarioExiste(request.UsuarioId, cancellationToken);

        if (validacionUsuario.IsFailure)
        {
            return validacionUsuario;
        }

        direccion.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteUsuarioDireccionCommand request, CancellationToken cancellationToken = default)
    {
        UsuarioDireccion? direccion = await BuscarDireccionPorId(request.Id, cancellationToken);

        if (direccion == null)
        {
            return Result.Failure(new ErrorResult("UsuarioDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        _unitOfWork.UsuarioDirecciones.Delete(direccion);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<UsuarioDireccion?> BuscarDireccionPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.UsuarioDirecciones.GetByIdAsync(id, false, cancellationToken);
    }

    private async Task<Result> ValidarUsuarioExiste(Guid usuarioId, CancellationToken cancellationToken = default)
    {
        bool existe = await _unitOfWork.Usuarios.AnyAsync(u => u.Id == usuarioId && !u.EsEliminado, cancellationToken);

        if (!existe)
        {
            return Result.Failure(new ErrorResult("UsuarioDireccion.UsuarioInvalido",
                $"No se encontró el usuario con el Id: {usuarioId}"));
        }

        return Result.Success();
    }

    #endregion
}

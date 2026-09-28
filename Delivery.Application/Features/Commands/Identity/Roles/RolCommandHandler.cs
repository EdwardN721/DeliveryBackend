using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;

namespace Delivery.Application.Features.Commands.Identity.Roles;

public class RolCommandHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<CreateRolCommand, Result<int>>,
    IRequestHandler<UpdateRolCommand, Result>,
    IRequestHandler<DeleteRolCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<int>> Handle(CreateRolCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionNombre = await ValidarNombreDisponible(request.Nombre, null, cancellationToken);

        if (validacionNombre.IsFailure)
        {
            return Result<int>.Failure(validacionNombre.Error);
        }

        Rol rol = request.MapToEntity();

        await _unitOfWork.Roles.AddAsync(rol, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<int>.Success(rol.Id);
    }

    public async Task<Result> Handle(UpdateRolCommand request, CancellationToken cancellationToken = default)
    {
        Rol? rol = await BuscarRolPorId(request.Id, cancellationToken);

        if (rol == null)
        {
            return Result.Failure(new ErrorResult("Rol.NotFound", $"No se encontró el rol con el Id: {request.Id}"));
        }

        Result validacionNombre = await ValidarNombreDisponible(request.Nombre, request.Id, cancellationToken);

        if (validacionNombre.IsFailure)
        {
            return validacionNombre;
        }

        rol.UpdateEntity(request);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> Handle(DeleteRolCommand request, CancellationToken cancellationToken = default)
    {
        Rol? rol = await BuscarRolPorId(request.Id, cancellationToken);

        if (rol == null)
        {
            return Result.Failure(new ErrorResult("Rol.NotFound", $"No se encontró el rol con el Id: {request.Id}"));
        }

        Result validacionUsuarios = await ValidarRolSinUsuarios(request.Id, cancellationToken);

        if (validacionUsuarios.IsFailure)
        {
            return validacionUsuarios;
        }

        _unitOfWork.Roles.Delete(rol);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<Rol?> BuscarRolPorId(int id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Roles.GetByIdAsync(id, false, cancellationToken);
    }

    private async Task<Result> ValidarNombreDisponible(string? nombre, int? idExcluir, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Success();
        }

        bool existe = await _unitOfWork.Roles.AnyAsync(
            predicate: r => r.Nombre == nombre && (idExcluir == null || r.Id != idExcluir.Value),
            cancellationToken: cancellationToken);

        if (existe)
        {
            return Result.Failure(new ErrorResult("Rol.NombreDuplicado", $"Ya existe un rol con el nombre: {nombre}"));
        }

        return Result.Success();
    }

    private async Task<Result> ValidarRolSinUsuarios(int id, CancellationToken cancellationToken = default)
    {
        bool tieneUsuarios = await _unitOfWork.Usuarios.AnyAsync(
            predicate: u => u.Roles.Any(r => r.Id == id),
            cancellationToken: cancellationToken);

        if (tieneUsuarios)
        {
            return Result.Failure(new ErrorResult("Rol.ConUsuarios", $"No se puede eliminar el rol con el Id: {id} porque tiene usuarios asignados"));
        }

        return Result.Success();
    }

    #endregion
}

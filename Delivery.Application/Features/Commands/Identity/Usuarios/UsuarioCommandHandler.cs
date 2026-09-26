using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;

namespace Delivery.Application.Features.Commands.Identity.Usuarios;

public class UsuarioCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher) :
    IRequestHandler<CreateUsuarioCommand, Result<Guid>>,
    IRequestHandler<UpdateUsuarioCommand, Result>,
    IRequestHandler<DeleteUsuarioCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<Result<Guid>> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        Result validacionCorreo = await ValidarCorreoDisponible(request.Correo, null, cancellationToken);

        if (validacionCorreo.IsFailure)
        {
            return Result<Guid>.Failure(validacionCorreo.Error);
        }

        Result<IReadOnlyList<Rol>> validacionRoles = await ObtenerRolesExistentes(request.RolIds, cancellationToken);

        if (validacionRoles.IsFailure)
        {
            return Result<Guid>.Failure(validacionRoles.Error);
        }

        Usuario usuario = request.MapToEntity(_passwordHasher.Hash(request.Password));
        usuario.SincronizarRoles(validacionRoles.Value!);

        await _unitOfWork.Usuarios.AddAsync(usuario, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<Guid>.Success(usuario.Id);
    }

    public async Task<Result> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        Usuario? usuario = await BuscarUsuarioPorId(request.Id, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure(new ErrorResult("Usuario.NotFound", $"No se encontró el usuario con el Id: {request.Id}"));
        }

        Result validacionCorreo = await ValidarCorreoDisponible(request.Correo, request.Id, cancellationToken);

        if (validacionCorreo.IsFailure)
        {
            return validacionCorreo;
        }

        if (request.RolIds is not null)
        {
            Result<IReadOnlyList<Rol>> validacionRoles = await ObtenerRolesExistentes(request.RolIds, cancellationToken);

            if (validacionRoles.IsFailure)
            {
                return Result.Failure(validacionRoles.Error);
            }

            usuario.SincronizarRoles(validacionRoles.Value!);
        }

        string? passwordHash = string.IsNullOrWhiteSpace(request.Password)
            ? null
            : _passwordHasher.Hash(request.Password);

        usuario.UpdateEntity(request, passwordHash);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    /// <summary>
    /// Elimina lógicamente al usuario. No se borra físicamente porque UsuarioDireccion.UsuarioId
    /// está configurado con DeleteBehavior.Restrict.
    /// </summary>
    public async Task<Result> Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        Usuario? usuario = await BuscarUsuarioPorId(request.Id, cancellationToken);

        if (usuario == null)
        {
            return Result.Failure(new ErrorResult("Usuario.NotFound", $"No se encontró el usuario con el Id: {request.Id}"));
        }

        usuario.EsEliminado = true;
        usuario.EsActivo = false;

        _unitOfWork.Usuarios.Update(usuario);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success();
    }

    #region MetodosPrivados

    private async Task<Usuario?> BuscarUsuarioPorId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Usuarios.FirstOrDefaultAsync(
            predicate: u => u.Id == id && !u.EsEliminado,
            disableTracking: false,
            cancellationToken: cancellationToken,
            includes: u => u.Roles);
    }

    private async Task<Result> ValidarCorreoDisponible(string? correo, Guid? idExcluir, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(correo))
        {
            return Result.Success();
        }

        bool existe = await _unitOfWork.Usuarios.AnyAsync(
            predicate: u => u.Correo == correo && !u.EsEliminado && (idExcluir == null || u.Id != idExcluir.Value),
            cancellationToken: cancellationToken);

        if (existe)
        {
            return Result.Failure(new ErrorResult("Usuario.CorreoDuplicado", $"Ya existe un usuario con el correo: {correo}"));
        }

        return Result.Success();
    }

    private async Task<Result<IReadOnlyList<Rol>>> ObtenerRolesExistentes(IReadOnlyList<int> rolIds, CancellationToken cancellationToken = default)
    {
        int[] ids = rolIds.Distinct().ToArray();

        if (ids.Length == 0)
        {
            return Result<IReadOnlyList<Rol>>.Success([]);
        }

        List<Rol> roles = (await _unitOfWork.Roles.GetAsync(
            predicate: r => ids.Contains(r.Id),
            disableTracking: false,
            cancellationToken: cancellationToken)).ToList();

        if (roles.Count != ids.Length)
        {
            return Result<IReadOnlyList<Rol>>.Failure(
                new ErrorResult("Usuario.RolInvalido", "Uno o más de los roles indicados no existen."));
        }

        return Result<IReadOnlyList<Rol>>.Success(roles);
    }

    #endregion
}

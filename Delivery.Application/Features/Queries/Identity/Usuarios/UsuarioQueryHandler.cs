using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Usuarios;

public class UsuarioQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<UsuarioByIdQuery, Result<UsuarioDto>>,
    IRequestHandler<UsuarioListQuery, Result<IEnumerable<UsuarioDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UsuarioDto>> Handle(UsuarioByIdQuery request, CancellationToken cancellationToken = default)
    {
        Usuario? usuario = await _unitOfWork.Usuarios.FirstOrDefaultAsync(
            predicate: u => u.Id == request.Id && !u.EsEliminado,
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: u => u.Roles);

        if (usuario == null)
        {
            return Result<UsuarioDto>.Failure(new ErrorResult("Usuario.NotFound", $"No se encontró el usuario con el Id: {request.Id}"));
        }

        return Result<UsuarioDto>.Success(usuario.MapToDto());
    }

    public async Task<Result<IEnumerable<UsuarioDto>>> Handle(UsuarioListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Usuario> usuarios = await _unitOfWork.Usuarios.GetAsync(
            predicate: u => !u.EsEliminado
                && (!request.SoloActivos || u.EsActivo)
                && (string.IsNullOrWhiteSpace(request.Correo) || u.Correo == request.Correo),
            disableTracking: true,
            cancellationToken: cancellationToken,
            includes: u => u.Roles);

        return Result<IEnumerable<UsuarioDto>>.Success(usuarios.MapToDto());
    }
}

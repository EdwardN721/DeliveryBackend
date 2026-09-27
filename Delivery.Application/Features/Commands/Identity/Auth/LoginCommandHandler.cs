using MediatR;
using Delivery.Core.Result;
using System.Linq.Expressions;
using Delivery.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Commands.Identity.Auth;

public class LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator) :
    IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator = jwtTokenGenerator;

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand command, CancellationToken cancellationToken = default)
    {
        Expression<Func<Usuario, bool>> queryBusquedaUsuario = u => u.Correo == command.Correo && !u.EsEliminado;

        Usuario? usuario = await _unitOfWork.Usuarios.FirstOrDefaultAsync(
            predicate: queryBusquedaUsuario,
            disableTracking: true,
            cancellationToken: cancellationToken, 
            include: query => query
                .Include(u => u.Roles)
        );

        if (usuario == null || !_passwordHasher.Verify(command.Password, usuario.Password))
        {
            return Result<AuthResponseDto>.Failure(
                new ErrorResult("Auth.CredencialesInvalidas", "Correo o contraseña incorrectos.")
            );
        }

        if (!usuario.EsActivo)
        {
            return Result<AuthResponseDto>.Failure(
                new ErrorResult("Auth.UsuarioInactivo", "La cuenta se encuentra inactiva. Contacte a soporte.")
            );
        }

        string token = _jwtTokenGenerator.GenerarToken(usuario);

        return Result<AuthResponseDto>.Success(new AuthResponseDto { Token = token });
    }
}

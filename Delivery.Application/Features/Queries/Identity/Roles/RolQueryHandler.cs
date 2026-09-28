using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.Roles;

public class RolQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<RolByIdQuery, Result<RolDto>>,
    IRequestHandler<RolListQuery, Result<IEnumerable<RolDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RolDto>> Handle(RolByIdQuery request, CancellationToken cancellationToken = default)
    {
        Rol? rol = await _unitOfWork.Roles.GetByIdAsync(request.Id, true, cancellationToken);

        if (rol == null)
        {
            return Result<RolDto>.Failure(new ErrorResult("Rol.NotFound", $"No se encontró el rol con el Id: {request.Id}"));
        }

        return Result<RolDto>.Success(rol.MapToDto());
    }

    public async Task<Result<IEnumerable<RolDto>>> Handle(RolListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<Rol> roles = await _unitOfWork.Roles.GetAllAsync(true, cancellationToken);
        return Result<IEnumerable<RolDto>>.Success(roles.MapToDto());
    }
}

using MediatR;
using Delivery.Core.Result;
using Delivery.Core.Interfaces;
using Delivery.Application.Mappers;
using Microsoft.EntityFrameworkCore;
using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;

namespace Delivery.Application.Features.Queries.Identity.UsuarioDirecciones;

public class UsuarioDireccionQueryHandler(IUnitOfWork unitOfWork) :
    IRequestHandler<UsuarioDireccionByIdQuery, Result<UsuarioDireccionDto>>,
    IRequestHandler<UsuarioDireccionListQuery, Result<IEnumerable<UsuarioDireccionDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UsuarioDireccionDto>> Handle(UsuarioDireccionByIdQuery request, CancellationToken cancellationToken = default)
    {
        UsuarioDireccion? direccion = await _unitOfWork.UsuarioDirecciones.FirstOrDefaultAsync(
            predicate: ud => ud.Id == request.Id,
            disableTracking: true,
            cancellationToken: cancellationToken,
            include: query => query
                .Include(ud => ud.Usuario!));

        if (direccion == null)
        {
            return Result<UsuarioDireccionDto>.Failure(new ErrorResult("UsuarioDireccion.NotFound", $"No se encontró la dirección con el Id: {request.Id}"));
        }

        return Result<UsuarioDireccionDto>.Success(direccion.MapToDto());
    }

    public async Task<Result<IEnumerable<UsuarioDireccionDto>>> Handle(UsuarioDireccionListQuery request, CancellationToken cancellationToken = default)
    {
        IEnumerable<UsuarioDireccion> direcciones = request.UsuarioId is null
            ? await _unitOfWork.UsuarioDirecciones.GetAllAsync(
                disableTracking: true,
                cancellationToken: cancellationToken,
                include: query => query.Include(ud => ud.Usuario!))
            : await _unitOfWork.UsuarioDirecciones.GetAsync(
                predicate: ud => ud.UsuarioId == request.UsuarioId.Value,
                disableTracking: true,
                cancellationToken: cancellationToken,
                include: query => query.Include(ud => ud.Usuario!));

        return Result<IEnumerable<UsuarioDireccionDto>>.Success(direcciones.MapToDto());
    }
}

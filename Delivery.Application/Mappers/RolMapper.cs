using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.Roles;

namespace Delivery.Application.Mappers;

public static class RolMapper
{
    public static Rol MapToEntity(this CreateRolCommand command)
    {
        return new Rol
        {
            Nombre = command.Nombre
        };
    }

    public static void UpdateEntity(this Rol rol, UpdateRolCommand updateCommand)
    {
        rol.Nombre = !string.IsNullOrWhiteSpace(updateCommand.Nombre) ? updateCommand.Nombre : rol.Nombre;
    }

    public static RolDto MapToDto(this Rol rol)
    {
        return new RolDto
        {
            Id = rol.Id,
            Nombre = rol.Nombre
        };
    }

    public static IEnumerable<RolDto> MapToDto(this IEnumerable<Rol>? roles)
    {
        return roles?.Select(MapToDto) ?? [];
    }
}

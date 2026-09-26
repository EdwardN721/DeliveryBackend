using Delivery.Core.Entities.Identity;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Identity.UsuarioDirecciones;

namespace Delivery.Application.Mappers;

public static class UsuarioDireccionMapper
{
    public static UsuarioDireccion MapToEntity(this CreateUsuarioDireccionCommand command)
    {
        return new UsuarioDireccion
        {
            Calle = command.Calle,
            Numero = command.Numero,
            Colonia = command.Colonia,
            Ciudad = command.Ciudad,
            Estado = command.Estado,
            CodigoPostal = command.CodigoPostal,
            UsuarioId = command.UsuarioId
        };
    }

    public static void UpdateEntity(this UsuarioDireccion direccion, UpdateUsuarioDireccionCommand updateCommand)
    {
        direccion.Calle = !string.IsNullOrWhiteSpace(updateCommand.Calle) ? updateCommand.Calle : direccion.Calle;
        direccion.Numero = !string.IsNullOrWhiteSpace(updateCommand.Numero) ? updateCommand.Numero : direccion.Numero;
        direccion.Colonia = !string.IsNullOrWhiteSpace(updateCommand.Colonia) ? updateCommand.Colonia : direccion.Colonia;
        direccion.Ciudad = !string.IsNullOrWhiteSpace(updateCommand.Ciudad) ? updateCommand.Ciudad : direccion.Ciudad;
        direccion.Estado = !string.IsNullOrWhiteSpace(updateCommand.Estado) ? updateCommand.Estado : direccion.Estado;
        direccion.CodigoPostal = !string.IsNullOrWhiteSpace(updateCommand.CodigoPostal) ? updateCommand.CodigoPostal : direccion.CodigoPostal;
        direccion.UsuarioId = updateCommand.UsuarioId != direccion.UsuarioId ? updateCommand.UsuarioId : direccion.UsuarioId;
    }

    public static UsuarioDireccionDto MapToDto(this UsuarioDireccion direccion)
    {
        return new UsuarioDireccionDto
        {
            Id = direccion.Id,
            Calle = direccion.Calle,
            Numero = direccion.Numero,
            Colonia = direccion.Colonia,
            Ciudad = direccion.Ciudad,
            Estado = direccion.Estado,
            CodigoPostal = direccion.CodigoPostal,
            UsuarioId = direccion.UsuarioId,
            UsuarioNombre = direccion.Usuario is null
                ? null
                : string.Join(" ",
                    new[] { direccion.Usuario.Nombres, direccion.Usuario.PrimerApellido, direccion.Usuario.SegundoApellido }
                        .Where(parte => !string.IsNullOrWhiteSpace(parte))),
            EsActivo = direccion.EsActivo
        };
    }

    public static IEnumerable<UsuarioDireccionDto> MapToDto(this IEnumerable<UsuarioDireccion>? direcciones)
    {
        return direcciones?.Select(MapToDto) ?? [];
    }
}

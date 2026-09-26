using Delivery.Core.Entities.Business;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.RestauranteDirecciones;

namespace Delivery.Application.Mappers;

public static class RestauranteDireccionMapper
{
    public static RestauranteDireccion MapToEntity(this CreateRestauranteDireccionCommand command)
    {
        return new RestauranteDireccion
        {
            Calle = command.Calle,
            Numero = command.Numero,
            Colonia = command.Colonia,
            Ciudad = command.Ciudad,
            Estado = command.Estado,
            CodigoPostal = command.CodigoPostal,
            RestauranteId = command.RestauranteId
        };
    }

    public static void UpdateEntity(this RestauranteDireccion direccion, UpdateRestauranteDireccionCommand updateCommand)
    {
        direccion.Calle = !string.IsNullOrWhiteSpace(updateCommand.Calle) ? updateCommand.Calle : direccion.Calle;
        direccion.Numero = !string.IsNullOrWhiteSpace(updateCommand.Numero) ? updateCommand.Numero : direccion.Numero;
        direccion.Colonia = !string.IsNullOrWhiteSpace(updateCommand.Colonia) ? updateCommand.Colonia : direccion.Colonia;
        direccion.Ciudad = !string.IsNullOrWhiteSpace(updateCommand.Ciudad) ? updateCommand.Ciudad : direccion.Ciudad;
        direccion.Estado = !string.IsNullOrWhiteSpace(updateCommand.Estado) ? updateCommand.Estado : direccion.Estado;
        direccion.CodigoPostal = !string.IsNullOrWhiteSpace(updateCommand.CodigoPostal) ? updateCommand.CodigoPostal : direccion.CodigoPostal;
        direccion.RestauranteId = updateCommand.RestauranteId != direccion.RestauranteId ? updateCommand.RestauranteId : direccion.RestauranteId;
    }

    public static RestauranteDireccionDto MapToDto(this RestauranteDireccion direccion)
    {
        return new RestauranteDireccionDto
        {
            Id = direccion.Id,
            Calle = direccion.Calle,
            Numero = direccion.Numero,
            Colonia = direccion.Colonia,
            Ciudad = direccion.Ciudad,
            Estado = direccion.Estado,
            CodigoPostal = direccion.CodigoPostal,
            RestauranteId = direccion.RestauranteId,
            RestauranteNombre = direccion.Restaurante?.Nombre,
            EsActivo = direccion.EsActivo
        };
    }

    public static IEnumerable<RestauranteDireccionDto> MapToDto(this IEnumerable<RestauranteDireccion>? direcciones)
    {
        return direcciones?.Select(MapToDto) ?? [];
    }
}
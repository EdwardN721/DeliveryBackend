using Delivery.Core.Entities.Business;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Restaurantes;

namespace Delivery.Application.Mappers;

public static class RestauranteMapper
{
    public static Restaurante MapToEntity(this CreateRestauranteCommand command)
    {
        return new Restaurante
        {
            Nombre = command.Nombre
        };
    }

    public static void UpdateEntity(this Restaurante restaurante, UpdateRestauranteCommand updateCommand)
    {
        restaurante.Nombre = !string.IsNullOrWhiteSpace(updateCommand.Nombre) ? updateCommand.Nombre : restaurante.Nombre;
    }

    public static RestauranteDto MapToDto(this Restaurante restaurante)
    {
        return new RestauranteDto
        {
            Id = restaurante.Id,
            Nombre = restaurante.Nombre,
            EsActivo = restaurante.EsActivo,
            EsEliminado = restaurante.EsEliminado
        };
    }

    public static IEnumerable<RestauranteDto> MapToDto(this IEnumerable<Restaurante>? restaurantes)
    {
        return restaurantes?.Select(MapToDto) ?? [];
    }
}
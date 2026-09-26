using Delivery.Core.Entities.Catalog;
using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Catalog.Categorias;

namespace Delivery.Application.Mappers;

public static class CategoriaMapper
{
    public static Categoria MapToEntity(this CreateCategoriaCommand command)
    {
        return new Categoria
        {
            Nombre = command.Nombre,
            Descripcion = command.Descripcion ?? "S/D"
        };
    }

    public static void UpdateEntity(this Categoria categoria, UpdateCategoriaCommand updateCommand)
    {
        categoria.Nombre = !string.IsNullOrWhiteSpace(updateCommand.Nombre) ? updateCommand.Nombre : categoria.Nombre;
        categoria.Descripcion = !string.IsNullOrWhiteSpace(updateCommand.Descripcion) ? updateCommand.Descripcion : categoria.Descripcion;
    }

    public static CategoriaDto MapToDto(this Categoria categoria)
    {
        return new CategoriaDto
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            EsActivo = categoria.EsActivo,
            EsEliminado = categoria.EsEliminado
        };
    }

    public static IEnumerable<CategoriaDto> MapToDto(this IEnumerable<Categoria>? categorias)
    {
        return categorias?.Select(MapToDto) ?? [];
    }
}

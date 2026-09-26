using Delivery.Application.Dto.Response;
using Delivery.Application.Features.Commands.Business.Productos;
using Delivery.Core.Entities.Business;

namespace Delivery.Application.Mappers;

public static class ProductoMapper
{
    public static Producto MapToEntity(this CreateProductoCommand command)
    {
        return new Producto
        {
            Nombre = command.Nombre,
            Descripcion = command.Descripcion ?? "S/D",
            Precio = command.Precio,
            RestauranteId = command.RestauranteId,
            CategoriaId = command.CategoriaId
        };
    }

    public static void UpdateEntity(this Producto producto, UpdateProductoCommand updateProducto)
    {
        producto.Nombre = !string.IsNullOrEmpty(updateProducto.Nombre) ? updateProducto.Nombre : producto.Nombre;
        producto.Descripcion = !string.IsNullOrEmpty(updateProducto.Descripcion) ? updateProducto.Descripcion : producto.Descripcion;
        producto.Precio = updateProducto.Precio != producto.Precio ? updateProducto.Precio : producto.Precio;
        producto.RestauranteId = updateProducto.RestauranteId != producto.RestauranteId ? updateProducto.RestauranteId : producto.RestauranteId;
        producto.CategoriaId = updateProducto.CategoriaId != producto.CategoriaId ? updateProducto.CategoriaId : producto.CategoriaId;
    }

    public static ProductoDto MapToDto(this Producto producto)
    {
        return new ProductoDto
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            RestauranteId = producto.RestauranteId,
            RestauranteNombre = producto.Restaurante?.Nombre,
            CategoriaId = producto.CategoriaId,
            CategoriaNombre = producto.Categoria.Nombre,
            EsActivo = producto.EsActivo
        };
    }

    public static IEnumerable<ProductoDto> MapToDto(this IEnumerable<Producto>? productos)
    {
        return productos?.Select(MapToDto) ?? [];
    }
}

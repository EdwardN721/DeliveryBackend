using Delivery.Core.Entities.Catalog;

namespace Delivery.Core.Entities.Business;

public class Producto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }

    public Guid RestauranteId { get; set; }
    public Restaurante Restaurante { get; set; } = null!;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
}
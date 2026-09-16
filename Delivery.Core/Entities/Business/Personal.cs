using Delivery.Core.Entities.Identity;

namespace Delivery.Core.Entities.Business;

public class Personal : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public virtual Usuario? Usuario { get; set; }

    public Guid RestauranteId { get; set; }
    public virtual Restaurante? Sucursal { get; set; }
}
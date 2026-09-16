namespace Delivery.Core.Entities.Business;

public class Restaurante : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;

    public virtual ICollection<RestauranteDireccion> Direcciones { get; set; } = new List<RestauranteDireccion>();
    public virtual ICollection<Personal> Usuarios { get; set; } = new List<Personal>();
}

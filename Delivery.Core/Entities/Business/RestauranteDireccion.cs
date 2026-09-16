namespace Delivery.Core.Entities.Business;

public class RestauranteDireccion : BaseEntity
{
    public string Calle {get; set; } = string.Empty;
    public string Numero {get; set; } = string.Empty;
    public string Colonia {get; set; } = string.Empty;
    public string Ciudad {get; set; } = string.Empty;
    public string Estado {get; set; } = string.Empty;
    public string CodigoPostal {get; set; } = string.Empty;

    public Guid RestauranteId {get; set; }
    public virtual Restaurante? Restaurante { get; set; }
}
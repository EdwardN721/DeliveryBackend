namespace Delivery.Core.Entities.Identity;

public class UsuarioDireccion : BaseEntity
{
    public string Calle {get; set; } = string.Empty;
    public string Numero {get; set; } = string.Empty;
    public string Colonia {get; set; } = string.Empty;
    public string Ciudad {get; set; } = string.Empty;
    public string Estado {get; set; } = string.Empty;
    public string CodigoPostal {get; set; } = string.Empty;

    public Guid UsuarioId {get; set; }
    public virtual Usuario? Usuario { get; set; }
}
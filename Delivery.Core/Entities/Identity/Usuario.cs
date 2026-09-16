namespace Delivery.Core.Entities.Identity;

public class Usuario : BaseEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public virtual ICollection<Rol> Roles { get; set; } = new List<Rol>();
    public virtual ICollection<UsuarioDireccion> Direcciones { get; set; } = new List<UsuarioDireccion>();
}
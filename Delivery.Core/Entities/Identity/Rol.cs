namespace Delivery.Core.Entities.Identity;

public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();    
}
using Delivery.Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Delivery.Infrastructure.Data;

public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<UsuarioDireccion> UsuarioDirecciones => Set<UsuarioDireccion>();
    
}
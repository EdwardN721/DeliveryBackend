using Delivery.Core.Entities.Identity;

namespace Delivery.Infrastructure;

public interface IJwtTokenGenerator
{
    string GenerarToken(Usuario usuario);
}

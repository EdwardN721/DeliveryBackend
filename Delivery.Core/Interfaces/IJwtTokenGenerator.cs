using Delivery.Core.Entities.Identity;

namespace Delivery.Core.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerarToken(Usuario usuario);
}

using System.Text;
using System.Security.Claims;
using Delivery.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Delivery.Core.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using Delivery.Infrastructure.Settings;

namespace Delivery.Infrastructure.Implementation;

public class JwtTokenGenerator(IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtOptions = jwtOptions.Value;

    public string GenerarToken(Usuario usuario)
    {
        // 1. Definir los Claims (la "carga pública" del token)
        List<Claim> claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // 2. Agregar los Roles. 
        foreach (Rol rol in usuario.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, rol.Nombre));
        }

        // 3. Firmar el token criptográficamente
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 4. Estructurar el JWT
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

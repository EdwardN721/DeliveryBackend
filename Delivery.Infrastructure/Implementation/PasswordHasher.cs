using Delivery.Core.Interfaces;
using CoreBCrypt = BCrypt.Net.BCrypt;

namespace Delivery.Infrastructure.Implementation;

/// <summary>
/// Implementación de <see cref="IPasswordHasher"/> basada en el algoritmo BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return CoreBCrypt.HashPassword(password);
    }

    public bool Verify(string password, string hash)
    {
        return CoreBCrypt.Verify(password, hash);
    }
}

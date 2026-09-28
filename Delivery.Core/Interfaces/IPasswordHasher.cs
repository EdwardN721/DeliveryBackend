namespace Delivery.Core.Interfaces;

/// <summary>
/// Contrato para transformar contraseñas en texto plano a hashes de un solo sentido y viceversa.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash seguro para la contraseña recibida.
    /// </summary>
    /// <param name="password">Contraseña en texto plano.</param>
    /// <returns>Contraseña cifrada, apta para almacenarse.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifica si una contraseña en texto plano corresponde a un hash previamente generado.
    /// </summary>
    /// <param name="password">Contraseña en texto plano a comprobar.</param>
    /// <param name="hash">Hash almacenado con el que se comparará.</param>
    /// <returns>Regresa true si la contraseña coincide con el hash, false en caso contrario.</returns>
    bool Verify(string password, string hash);
}

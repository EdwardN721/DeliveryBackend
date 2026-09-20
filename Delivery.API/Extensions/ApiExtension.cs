using Delivery.Exceptions;

namespace Delivery.Extensions;

/// <summary>
/// Archivo de configuracion de la capa API
/// </summary>
public static class ApiExtension
{
    /// <summary>
    /// Agregar configuración del manejador de excepciones
    /// </summary>
    public static IServiceCollection AddExceptionHandlerConfiguracion(this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}

using Asp.Versioning;
using Delivery.Services;
using Delivery.Exceptions;
using Delivery.Core.Interfaces;

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

    /// <summary>
    /// Agregar configuracion de versionado
    /// </summary>
    public static IServiceCollection AddApiVersioningConfiguracion(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0); 
            options.AssumeDefaultVersionWhenUnspecified = true; 
            options.ReportApiVersions = true; 
        })
        .AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV"; 
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }

    public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}

using MediatR;
using FluentValidation;
using System.Reflection;
using Delivery.Application.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.Application.Extensions;

/// <summary>
/// Archivo de configuración de la capa Application
/// </summary>
public static class ApplicationExtension
{
    /// <summary>
    /// Agregar configuración de validadores
    /// </summary>
    public static IServiceCollection AddValidatorConfiguracion(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Registrar MediatR y los ValidationBehavior
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assembly);
            config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Registar todos los validadores de FluentValidation automáticamente
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}

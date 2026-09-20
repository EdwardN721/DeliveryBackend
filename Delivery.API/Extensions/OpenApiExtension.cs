using Scalar.AspNetCore;

namespace Delivery.Extensions;

public static class OpenApiExtension
{
    public static IServiceCollection AddDocumentacionConfiguracion(this IServiceCollection services)
    {
        // 1. Usamos el método nativo de Microsoft directo sobre IServiceCollection
        services.AddOpenApi("v1", options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "Delivery Aplicación.";
                document.Info.Description = "API de alto rendimiento construida con CQRS y MediatR.";
                document.Info.Version = "v1";
                return Task.CompletedTask;
            });
        });

        return services;
    }

    public static IApplicationBuilder UseScalarDocumentacion(this WebApplication app)
    {
        // 2. Usamos el endpoint nativo
        app.MapOpenApi();

        // 3. Monta la UI de Scalar
        app.MapScalarApiReference(options =>
        {
            options.WithTitle("Delivery API Reference");
            options.WithTheme(ScalarTheme.DeepSpace); 
            options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient); 
            options.HideModels();
        });

        return app;
    }
}
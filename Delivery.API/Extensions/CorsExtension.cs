namespace Delivery.Extensions;

public static class CorsExtension
{
    public static IServiceCollection AddCorsConfiguracion(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin() 
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination");
            });
        });

        return services;
    }
}
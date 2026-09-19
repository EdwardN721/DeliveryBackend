using Delivery.Core.Interfaces;
using Delivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Delivery.Infrastructure.Interceptors;
using Delivery.Infrastructure.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.Infrastructure.Extension;

public static class InfrastructureExtension{
    
    public static IServiceCollection AddInterceptorsConfiguracion(this IServiceCollection services)
    {
        services.AddScoped<AuditInterceptor>();

        return services;
    }

    public static IServiceCollection AddDbConfiguracion(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DeliveryDbContext>((sp, options) =>
        {
            AuditInterceptor interceptorAudit = sp.GetService<AuditInterceptor>()!;

            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(interceptorAudit);
        });

        return services;
    }

    public static IServiceCollection AddUnitOfWorkConfig(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
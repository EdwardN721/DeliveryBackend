using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Delivery.Core.Extension;

public static class CoreExtension
{
    public static IServiceCollection AddHttpContextAccesorConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}
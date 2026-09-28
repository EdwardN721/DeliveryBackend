using Delivery.Core.Interfaces;
using Delivery.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Delivery.Infrastructure.Interceptors;
using Delivery.Infrastructure.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Delivery.Infrastructure.Settings;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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

    public static IServiceCollection AddPasswordHasherConfig(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        JwtSettings jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;
        byte[] secretKey = Encoding.UTF8.GetBytes(jwtSettings!.Secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true, // Verifica que esté firmado con tu llave secreta
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };
        });

        services.AddAuthorization();

        return services;
    }
}
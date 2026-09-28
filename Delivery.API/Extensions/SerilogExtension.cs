using Serilog;
using Serilog.Events;

namespace Delivery.Extensions;

public static class SerilogExtension
{
    public static WebApplicationBuilder AddSerilogConfiguracion(this WebApplicationBuilder builder)
    {
        // Configuramos Serilog
        Log.Logger = new LoggerConfiguration()
            // Nivel mínimo general
            .MinimumLevel.Information()
            // Silenciamos los logs ruidosos internos de .NET, excepto si son advertencias o errores
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            // Agrega metadatos útiles al log (Id del hilo, etc.)
            .Enrich.FromLogContext()
            // Salida 1: La consola (con colores para desarrollo)
            .WriteTo.Console()
            // Salida 2: Archivo de texto rotativo diario
            .WriteTo.File(
                path: "Logs/log-.txt", // El guion al final es para que Serilog agregue la fecha (log-2026-09-27.txt)
                rollingInterval: RollingInterval.Day, // Crea un archivo nuevo cada día
                retainedFileCountLimit: 30, // Guarda solo los últimos 30 días para no llenar el disco duro
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .CreateLogger();

        // Le decimos a .NET que reemplace su logger por defecto con Serilog
        builder.Host.UseSerilog();

        return builder;
    }
}
using Delivery.Extensions;
using Delivery.Application.Extensions;
using Delivery.Infrastructure.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Agregar configuracion de base de datos
builder.Services.AddDbConfiguracion(builder.Configuration);

// Agregar interceptores
builder.Services.AddInterceptorsConfiguracion();

// Agregar obtener usuario que modifico
builder.Services.AddCurrentUserService();

// Registrar Unit Of Work
builder.Services.AddUnitOfWorkConfig();

// Registrar cifrado de contraseñas
builder.Services.AddPasswordHasherConfig();

// Registrar JWT y Autenticación
builder.Services.AddJwtConfig(builder.Configuration);

// Registrar Validations
builder.Services.AddValidatorConfiguracion();

// Agregar manejador de excepciones
builder.Services.AddExceptionHandlerConfiguracion();

// Agregar versionamiento
builder.Services.AddApiVersioningConfiguracion();

// Agregar la configuración de OpenAPI
builder.Services.AddDocumentacionConfiguracion();

// Registrar CORS
builder.Services.AddCorsConfiguracion();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseScalarDocumentacion();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication(); // Primero pregunta: ¿Quién eres? (Lee el token)
app.UseAuthorization();  // Luego pregunta: ¿Tienes permiso para entrar aquí?
app.MapControllers();

app.Run();
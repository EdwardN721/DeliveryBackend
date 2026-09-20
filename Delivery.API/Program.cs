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

// Registrar Validations
builder.Services.AddValidatorConfiguracion();

// Agregar manejador de excepciones
builder.Services.AddExceptionHandlerConfiguracion();

// Agregar versionamiento
builder.Services.AddApiVersioningConfiguracion();

//Agregar la configuración de OpenAPI
builder.Services.AddDocumentacionConfiguracion();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseScalarDocumentacion();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
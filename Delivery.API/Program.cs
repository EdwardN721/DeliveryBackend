using Delivery.Infrastructure.Extension;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbConfiguracion(builder.Configuration);
builder.Services.AddInterceptorsConfiguracion();
builder.Services.AddUnitOfWorkConfig();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
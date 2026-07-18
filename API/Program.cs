using API.Extensions;
using API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Usar Serilog como proveedor de logging
builder.Host.UseSerilog();

// Configuración de servicios
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Servicios personalizados de la aplicación
builder.Services.AddApplicationServices(builder.Configuration);

// Autenticación JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// Swagger con JWT
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Middleware de manejo global de excepciones (debe ir primero)
app.UseGlobalExceptionHandler();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DevManager API v1");
    options.DocumentTitle = "DevManager API Documentation";
});
//}

app.UseHttpsRedirection();

// CORS
app.UseCors("DevManagerPolicy");

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Iniciando DevManager API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

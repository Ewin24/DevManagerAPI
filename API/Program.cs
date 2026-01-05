using API.Extensions;
using API.Middleware;

var builder = WebApplication.CreateBuilder(args);

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DevManager API v1");
        options.DocumentTitle = "DevManager API Documentation";
    });
}

app.UseHttpsRedirection();

// CORS
app.UseCors("DevManagerPolicy");

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

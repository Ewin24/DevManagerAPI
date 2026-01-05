# DevManagerAPI - Instrucciones para Agentes IA

## Arquitectura del Proyecto

Este es un proyecto ASP.NET Core Web API (.NET 8.0) siguiendo **Clean Architecture** con las siguientes capas:

```
API/               → Capa de presentación (Controllers, endpoints, configuración ASP.NET Core)
Application/       → Lógica de aplicación (casos de uso, DTOs, interfaces de servicios)
Domain/            → Entidades de negocio, reglas de dominio, interfaces de repositorio
Infrastructure/    → Implementaciones concretas (acceso a datos, servicios externos)
```

### Flujo de Dependencias (Regla Fundamental)
- **API** → Application → Domain
- **Infrastructure** → Application + Domain
- **Domain**: Sin dependencias externas (núcleo puro)
- **Application**: Solo depende de Domain
- **API e Infrastructure**: Orquestación e implementación

## Convenciones del Código Base

### Namespaces y Ubicación
- Controllers: `API.Controllers` namespace en `/API/Controllers/`
- Modelos de dominio: `Domain` namespace (raíz o subcarpetas por agregado)
- Casos de uso/servicios: `Application` namespace
- Configuración: [appsettings.json](API/appsettings.json), [launchSettings.json](API/Properties/launchSettings.json)

### Configuración de Proyecto
- **Target Framework**: `net8.0`
- **Nullable**: Habilitado en todos los proyectos
- **ImplicitUsings**: Habilitado en todos los proyectos
- **Entry Point**: [API/Program.cs](API/Program.cs) - configuración minimal API style

### Patrones Establecidos
- **Controladores**: Heredan de `ControllerBase`, usan `[ApiController]` y `[Route("[controller]")]`
- **Inyección de dependencias**: Registro en `Program.cs` usando `builder.Services`
- **Swagger**: Configurado para desarrollo en [Program.cs](API/Program.cs#L15-L17)

## Comandos de Desarrollo

### Build y Ejecución
```bash
# Build de la solución completa
dotnet build DevManager.sln

# Ejecutar API (puerto HTTPS: 7265, HTTP: 5073)
dotnet run --project API/API.csproj

# Restaurar paquetes NuGet
dotnet restore
```

### Testing (cuando se implementen tests)
```bash
dotnet test
```

## Guías para Nuevas Features

### Agregar una Nueva Entidad
1. Crear clase en `/Domain/` (entidad de dominio)
2. Crear DTOs en `/Application/` si es necesario
3. Crear interface de repositorio en `/Domain/`
4. Implementar repositorio en `/Infrastructure/`
5. Crear controlador en `/API/Controllers/`

### Agregar un Nuevo Controlador
- Ubicar en `/API/Controllers/`
- Heredar de `ControllerBase`
- Aplicar `[ApiController]` y `[Route("[controller]")]`
- Inyectar dependencias vía constructor
- Documentar endpoints con nombres en atributos HTTP (ej: `[HttpGet(Name = "GetWeatherForecast")]`)

### Configuración de Swagger
- Ya configurado en [Program.cs](API/Program.cs)
- Accesible en: `https://localhost:7265/swagger` o `http://localhost:5073/swagger`
- Solo habilitado en Development por defecto

## Puntos de Atención

- **No romper la regla de dependencias**: Domain nunca debe referenciar otros proyectos
- **Application**: Contiene interfaces que Infrastructure implementa
- **API**: Punto de entrada único, maneja HTTP concerns
- El proyecto está en fase inicial - actualmente solo tiene el template WeatherForecast

## Próximos Pasos Esperados

Al desarrollar este proyecto, se espera:
- Definir entidades de dominio en `/Domain/`
- Implementar casos de uso en `/Application/`
- Configurar acceso a datos (EF Core, Dapper, etc.) en `/Infrastructure/`
- Agregar autenticación/autorización según sea necesario
- Implementar testing (crear proyectos de test en la solución)

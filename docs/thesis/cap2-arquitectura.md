# Capítulo 2: Arquitectura del Sistema

## Introducción

El presente capítulo describe el modelo arquitectónico adoptado en el desarrollo de DevManagerAPI. La arquitectura del sistema se fundamenta en principios de separación de responsabilidades, mantenibilidad y extensibilidad. Se documenta la estructura de capas, los patrones de diseño implementados y los mecanismos transversales que garantizan la seguridad, la integridad de los datos y la consistencia de las respuestas en todos los módulos del sistema.

---

## 2.1 Clean Architecture: Separación en Cuatro Capas

DevManagerAPI adopta el patrón de **Clean Architecture** propuesto por Robert C. Martin, organizando el código fuente en cuatro proyectos independientes con una dirección de dependencia estrictamente unidireccional: las capas externas dependen de las internas, pero nunca a la inversa.

### Tabla de Capas

| Capa | Proyecto | Responsabilidad | Dependencias |
|------|----------|-----------------|--------------|
| **Dominio** | `Domain` | Define las entidades del negocio (POCOs puros), las interfaces de repositorios y los enumeradores del dominio. No contiene lógica de infraestructura ni de presentación. | **Ninguna** — capa más interna, sin referencias externas. |
| **Aplicación** | `Application` | Contiene los servicios de negocio, los objetos de transferencia de datos (DTOs), las interfaces de servicios y el modelo de respuesta estandarizado (`ApiResponse<T>`). Orquesta los casos de uso. | `Domain` únicamente. |
| **Infraestructura** | `Infrastructure` | Implementa los repositorios utilizando Entity Framework Core, el `DbContext` (`DevManagerDbContext`), las entidades de mapeo EF, los servicios externos (Gemini AI, TokenService) y los servicios en segundo plano. | `Domain` + `Application`. |
| **API** | `API` | Contiene los controladores HTTP, el middleware de excepciones, las extensiones de inyección de dependencias y la configuración de Swagger. Expone los endpoints REST y maneja el ciclo de vida de las peticiones. | Todas las capas anteriores (`Domain`, `Application`, `Infrastructure`). |

### Dependencia entre Capas (Diagrama)

```
         ┌──────────────────────────────────────────┐
         │                  API/                    │
         │  Controllers · Middleware · Extensions   │
         └──────────────┬───────────────────────────┘
                        │ depende de
         ┌──────────────▼───────────────────────────┐
         │            Infrastructure/               │
         │  Repositories · DbContext · Services     │
         └──────────────┬───────────────────────────┘
                        │ depende de
         ┌──────────────▼───────────────────────────┐
         │             Application/                 │
         │  Services · DTOs · Interfaces            │
         └──────────────┬───────────────────────────┘
                        │ depende de
         ┌──────────────▼───────────────────────────┐
         │               Domain/                    │
         │  Entities · Interfaces · Enums           │
         │     (sin dependencias externas)          │
         └──────────────────────────────────────────┘
```

La ventaja principal de esta estructura es que el `Domain` puede ser probado de forma aislada y que las decisiones de infraestructura (motor de base de datos, cliente HTTP, proveedor de IA) pueden modificarse sin alterar la lógica de negocio.

---

## 2.2 Patrón Repositorio

El sistema implementa el **Patrón Repositorio** (*Repository Pattern*) para aislar la capa de acceso a datos de la lógica de negocio. Este patrón define contratos (interfaces) en la capa de Dominio e implementaciones concretas en la capa de Infraestructura.

### Estructura del Patrón

```
Domain/
└── Interfaces/
    └── Repositories/
        ├── IAuthRepository
        ├── IUserRepository
        ├── ISkillRepository
        ├── IEmployeeSkillRepository
        ├── ICertificationRepository
        ├── IProfileRepository
        ├── IProjectRepository
        ├── IApplicationRepository
        ├── IAssignmentRepository
        ├── IRolePermissionRepository
        └── IAgentRepository

Infrastructure/
└── Repositories/
    ├── AuthRepository          → implementa IAuthRepository
    ├── UserRepository          → implementa IUserRepository
    ├── SkillRepository         → implementa ISkillRepository
    ├── EmployeeSkillRepository → implementa IEmployeeSkillRepository
    ├── CertificationRepository → implementa ICertificationRepository
    ├── ProfileRepository       → implementa IProfileRepository
    ├── ProjectRepository       → implementa IProjectRepository
    ├── ApplicationRepository   → implementa IApplicationRepository
    ├── AssignmentRepository    → implementa IAssignmentRepository
    ├── RolePermissionRepository→ implementa IRolePermissionRepository
    └── AgentRepository         → implementa IAgentRepository
```

### Ventajas del Patrón

1. **Testabilidad:** Los servicios de aplicación dependen de interfaces, no de clases concretas. Esto permite sustituir las implementaciones por dobles de prueba (*mocks*) durante las pruebas unitarias.
2. **Desacoplamiento:** La capa `Application` no importa ni referencia `Entity Framework Core` directamente; toda interacción con la base de datos ocurre a través de las interfaces.
3. **Responsabilidad única:** Cada repositorio encapsula exclusivamente las consultas SQL relacionadas con una entidad de dominio o un módulo funcional específico.

Todos los repositorios reciben el `DevManagerDbContext` por inyección de dependencias y aplican los filtros de `OrganizationId` y `IsDeleted = false` en cada consulta, como se detalla en las secciones 2.4 y 2.5.

---

## 2.3 Patrón de Doble Entidad (Dual Entity Pattern)

Una decisión arquitectónica fundamental en DevManagerAPI es la existencia de **dos modelos de entidad paralelos** para representar los datos del negocio: las entidades de dominio puras y las entidades de mapeo de Entity Framework Core.

### Motivación

Entity Framework Core impone restricciones técnicas sobre las clases que mapea: requiere constructores sin parámetros o con parámetros específicos, permite anotaciones de datos o configuraciones Fluent API, y puede agregar propiedades de navegación que no pertenecen al modelo conceptual del negocio. Si se utilizara la misma clase tanto como entidad de dominio como modelo EF, la capa `Domain` adquiriría una dependencia implícita sobre los convenios de ORM, violando el principio de inversión de dependencias.

### Estructura de Doble Entidad

```
Domain/Entities/           (POCOs puros — sin atributos EF)
├── Talent/
│   ├── Skill
│   ├── EmployeeSkill
│   ├── EmployeeProfile
│   ├── Certification
│   └── SkillEvaluation
├── IAM/
│   ├── Organization
│   ├── User
│   ├── Role
│   └── Permission
├── Projects/
│   ├── Project
│   ├── ProjectSkillRequirement
│   ├── ProjectRole
│   └── ProjectApplication
└── Agent/
    ├── AgentAction
    └── AgentConfiguration

Infrastructure/Data/Entities/  (modelos EF Core — configurados para SQL Server)
├── Talent/   → clases EF con atributos de columna, longitudes, índices
├── IAM/      → FK constraints, UQ constraints, precisión de tipos
├── Projects/ → configuraciones de relación y cascada
├── Config/   → catálogos del sistema
└── Reporting/→ índices únicos, campos JSON
```

### Mecanismo de Mapeo

Cada repositorio define métodos privados de mapeo bidireccional:

```csharp
// En SkillRepository.cs (Infrastructure/Repositories/)

// Mapeo EF → Dominio: devuelve un POCO limpio a los servicios
private static Domain.Entities.Talent.Skill MapToDomain(
    Infrastructure.Data.Entities.Skill ef)
{
    return new Domain.Entities.Talent.Skill
    {
        Id              = ef.Id,
        Name            = ef.Name,
        Category        = ef.Category,
        SkillType       = ef.SkillType,
        OrganizationId  = ef.OrganizationId,
        CreatedAt       = ef.CreatedAt,
        CreatedByUserId = ef.CreatedByUserId,
        UpdatedAt       = ef.UpdatedAt,
        UpdatedByUserId = ef.UpdatedByUserId,
        IsDeleted       = ef.IsDeleted,
        DeletedAt       = ef.DeletedAt,
        DeletedByUserId = ef.DeletedByUserId
    };
}

// Mapeo Dominio → EF: construye el modelo EF para persistencia
private static Infrastructure.Data.Entities.Skill MapToEntity(
    Domain.Entities.Talent.Skill domain)
{
    return new Infrastructure.Data.Entities.Skill
    {
        Id              = domain.Id == default ? Guid.NewGuid() : domain.Id,
        Name            = domain.Name,
        Category        = domain.Category,
        SkillType       = domain.SkillType,
        OrganizationId  = domain.OrganizationId,
        CreatedAt       = domain.CreatedAt == default
                              ? DateTime.UtcNow
                              : domain.CreatedAt,
        CreatedByUserId = domain.CreatedByUserId,
        IsDeleted       = false
    };
}
```

De esta manera, los servicios de la capa `Application` trabajan únicamente con POCOs del dominio; la lógica de ORM queda totalmente encapsulada en los repositorios.

---

## 2.4 Multitenencia por Reclamación JWT (Multi-tenancy)

DevManagerAPI implementa una estrategia de **multitenencia lógica** (*logical multi-tenancy*): todos los registros comparten la misma base de datos y el mismo esquema físico, pero cada fila pertenece a una organización identificada por la columna `OrganizationId` (GUID). El aislamiento de datos se aplica en la capa de repositorio mediante un filtro `WHERE OrganizationId = @orgId` en cada consulta.

### Flujo de Extracción y Aplicación del Claim

```
Solicitud HTTP entrante
         │
         ▼
[JWT Bearer Middleware]
 Valida firma HMACSHA512,
 expiración e issuer/audience
         │
         ▼
Extracción de Claims del token:
  ├─ nameid        → UserId  (Guid)
  ├─ email         → Email
  ├─ name          → Nombre completo
  ├─ OrganizationId→ OrganizationId (Guid)  ◄── CRÍTICO para multi-tenancy
  └─ jti           → JWT ID único

         │
         ▼
[Controlador]
 Obtiene OrganizationId desde ClaimsPrincipal:
   var orgId = Guid.Parse(
       User.FindFirst("OrganizationId")!.Value);

         │
         ▼
[Servicio de Aplicación]
 Recibe orgId como parámetro y lo
 pasa al repositorio correspondiente

         │
         ▼
[Repositorio EF Core]
 Aplica doble filtro en TODAS las consultas:
   WHERE OrganizationId = @orgId
     AND IsDeleted = 0

         │
         ▼
[SQL Server]
 Ejecuta consulta aislada por organización
 en el esquema correspondiente:
   iam.* | talent.* | projects.* | reporting.*
```

### Garantías del Modelo

- **Aislamiento:** Un usuario autenticado de la Organización A no puede acceder, leer ni modificar datos de la Organización B, independientemente del identificador de recurso utilizado en la URL.
- **Sin lógica de tenant en servicios:** Los servicios de aplicación no deciden el contexto de organización; reciben el `OrganizationId` como parámetro del controlador, que a su vez lo extrae del token JWT.
- **Trazabilidad:** Todos los registros creados incluyen `CreatedByUserId` y `OrganizationId`, lo que permite auditar el origen de cada operación.

### Patrón HITL (Human-in-the-Loop) y Multi-tenancy

Las acciones del agente inteligente también respetan la multitenencia. Cada `AgentAction` almacena el `OrganizationId` de la organización que originó la solicitud. Cuando una acción requiere aprobación humana (estado `PENDING_APPROVAL`), únicamente los usuarios con el `OrganizationId` correspondiente pueden aprobarla o rechazarla a través de los endpoints `POST /agent/approve/{id}` y `POST /agent/reject/{id}`.

---

## 2.5 Borrado Lógico Universal (Soft Delete)

DevManagerAPI no ejecuta sentencias `DELETE` físicas en la base de datos. Todas las entidades del dominio que requieren eliminación heredan de la clase abstracta `AuditableEntity`, que provee siete campos de auditoría.

### Clase Base AuditableEntity

```csharp
// Domain/Common/AuditableEntity.cs
public abstract class AuditableEntity
{
    public DateTime  CreatedAt         { get; set; }
    public Guid?     CreatedByUserId   { get; set; }
    public DateTime? UpdatedAt         { get; set; }
    public Guid?     UpdatedByUserId   { get; set; }
    public bool      IsDeleted         { get; set; }
    public DateTime? DeletedAt         { get; set; }
    public Guid?     DeletedByUserId   { get; set; }
}
```

### Campos de Auditoría

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `CreatedAt` | `DateTime` (UTC) | Fecha y hora de creación del registro. |
| `CreatedByUserId` | `Guid?` | Identificador del usuario que creó el registro. |
| `UpdatedAt` | `DateTime?` (UTC) | Fecha y hora de la última modificación. `NULL` si nunca fue modificado. |
| `UpdatedByUserId` | `Guid?` | Identificador del usuario que realizó la última modificación. |
| `IsDeleted` | `bool` | Bandera de eliminación lógica. `true` indica registro eliminado. |
| `DeletedAt` | `DateTime?` (UTC) | Fecha y hora en que se marcó el registro como eliminado. |
| `DeletedByUserId` | `Guid?` | Identificador del usuario que ejecutó la eliminación lógica. |

### Comportamiento en Repositorios

Toda consulta de recuperación de datos aplica el filtro `!s.IsDeleted` (equivalente a `WHERE IsDeleted = 0` en SQL):

```csharp
// Ejemplo en SkillRepository.cs
var efSkills = await _context.Skills
    .AsNoTracking()
    .Where(s => s.OrganizationId == organizationId && !s.IsDeleted)
    .OrderBy(s => s.Name)
    .ToListAsync();
```

Las operaciones de eliminación concretan el borrado lógico actualizando los tres campos correspondientes:

```csharp
// Eliminación lógica en cualquier repositorio
efSkill.IsDeleted         = true;
efSkill.DeletedAt         = DateTime.UtcNow;
efSkill.DeletedByUserId   = deletedByUserId;
await _context.SaveChangesAsync();
```

### Ventajas del Borrado Lógico

1. **Recuperabilidad:** Los datos marcados como eliminados pueden restaurarse sin necesidad de copias de respaldo.
2. **Auditoría completa:** Se mantiene el historial de quién eliminó cada registro y cuándo.
3. **Integridad referencial:** Las claves foráneas de otros registros no se ven afectadas por la eliminación lógica.
4. **Consistencia:** El patrón se aplica de forma uniforme en todos los módulos del sistema (IAM, Talent, Projects, Reporting).

---

## 2.6 Manejo Centralizado de Excepciones y Respuestas Estandarizadas

### GlobalExceptionHandlerMiddleware

El sistema centraliza el tratamiento de errores no controlados en el componente `GlobalExceptionHandlerMiddleware`, registrado como el primer middleware en el pipeline de ASP.NET Core. Su función es interceptar cualquier excepción que atraviese el pipeline y transformarla en una respuesta HTTP estructurada con código de estado apropiado.

```
Solicitud HTTP
     │
     ▼
[GlobalExceptionHandlerMiddleware]
     │  try { await _next(context); }
     │  catch (Exception ex) → HandleExceptionAsync(context, ex)
     │
     ├─ ApplicationException (errores de negocio)
     │    → HTTP 400/404/409 según StatusCode de la excepción
     │    → ErrorCode (e.g. "INVALID_ARGUMENT", "NOT_FOUND")
     │
     ├─ UnauthorizedAccessException
     │    → HTTP 401 Unauthorized
     │    → ErrorCode: "UNAUTHORIZED"
     │
     ├─ ArgumentNullException / ArgumentException
     │    → HTTP 400 Bad Request
     │    → ErrorCode: "INVALID_ARGUMENT"
     │
     └─ Exception (cualquier otro error)
          → HTTP 500 Internal Server Error
          → ErrorCode: "INTERNAL_ERROR"
          → TraceId incluido en la respuesta
```

El middleware serializa la respuesta en JSON con política de nomenclatura en camelCase, incluyendo el `TraceId` de la solicitud HTTP para facilitar la correlación de registros (*log correlation*) en Serilog.

### ApiResponse\<T\> — Respuestas Estandarizadas de Éxito

Todos los controladores devuelven objetos de tipo `ApiResponse<T>` para los casos exitosos, garantizando una estructura de respuesta homogénea en toda la API:

```csharp
// Application/Common/Models/ApiResponse.cs
public class ApiResponse<T>
{
    public bool     Success   { get; set; }
    public string   Message   { get; set; } = string.Empty;
    public T?       Data      { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(
        T data,
        string message = "Operación exitosa") => new()
    {
        Success = true,
        Message = message,
        Data    = data
    };

    public static ApiResponse<T> SuccessResponse(string message) => new()
    {
        Success = true,
        Message = message
    };
}
```

### Estructura de Respuesta para Errores

```csharp
// Application/Common/Models/ErrorResponse (serializado por el middleware)
{
    "success"   : false,
    "message"   : "Descripción del error",
    "errorCode" : "CODIGO_ERROR",
    "errors"    : { "campo": ["mensaje de validación"] },  // opcional
    "timestamp" : "2025-01-01T00:00:00Z",
    "traceId"   : "0HMXXXXXX:00000001"
}
```

Esta arquitectura garantiza que los consumidores de la API puedan inspeccionar el campo `success` para determinar el resultado de cualquier operación, sin necesidad de analizar códigos de estado HTTP de forma exclusiva.

---

## 2.7 Inyección de Dependencias Centralizada

La configuración de todos los servicios, repositorios y opciones de infraestructura se centraliza en la clase estática `ApplicationServiceExtensions`, ubicada en `API/Extensions/ApplicationServiceExtensions.cs`. Esta clase expone tres métodos de extensión sobre `IServiceCollection`:

### Métodos de Extensión

#### `AddApplicationServices(IServiceCollection, IConfiguration)`

Registra el `DbContext` de Entity Framework Core con la cadena de conexión de SQL Server, todos los repositorios (IAM, Talent, Projects), todos los servicios de aplicación (IAM, Talent, Projects, Configuración, IA y Agente), el cliente HTTP de Gemini y los servicios en segundo plano:

```csharp
// Registro del DbContext
services.AddDbContext<DevManagerDbContext>(options =>
    options.UseSqlServer(
        configuration.GetConnectionString("DefaultConnection")));

// Repositorios — IAM
services.AddScoped<IAuthRepository,           AuthRepository>();
services.AddScoped<IUserRepository,           UserRepository>();
services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

// Repositorios — Talent
services.AddScoped<IProfileRepository,        ProfileRepository>();
services.AddScoped<ISkillRepository,          SkillRepository>();
services.AddScoped<IEmployeeSkillRepository,  EmployeeSkillRepository>();
services.AddScoped<ICertificationRepository,  CertificationRepository>();

// Repositorios — Projects
services.AddScoped<IProjectRepository,        ProjectRepository>();
services.AddScoped<IApplicationRepository,    ApplicationRepository>();
services.AddScoped<IAssignmentRepository,     AssignmentRepository>();

// Servicios de IA
services.AddHttpClient<IGeminiService, GeminiService>();
services.AddScoped<IAgentService,      AgentService>();
services.AddScoped<IAgentRepository,   AgentRepository>();

// Servicios en segundo plano (condicional por configuración)
if (configuration.GetValue<bool>("Agent:EnableBackgroundServices", true))
{
    services.AddHostedService<ReportSnapshotGeneratorService>();
    services.AddHostedService<RecommendationOptimizerService>();
}
```

#### `AddJwtAuthentication(IServiceCollection, IConfiguration)`

Configura el esquema de autenticación JWT Bearer con los parámetros de validación del token:

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey)),   // HMACSHA512
            ValidateIssuer    = true,
            ValidIssuer       = issuer,               // "DevManagerAPI"
            ValidateAudience  = true,
            ValidAudience     = audience,             // "DevManagerClient"
            ValidateLifetime  = true,
            ClockSkew         = TimeSpan.Zero         // sin margen de tolerancia
        };
    });
```

#### `AddSwaggerDocumentation(IServiceCollection)`

Configura Swagger/OpenAPI con soporte para autenticación Bearer, comentarios XML e información del documento:

```csharp
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "DevManager API",
        Version = "v1",
        Description = "API para gestión de talento y proyectos con multi-tenancy"
    });
    // Esquema de seguridad Bearer para autenticación en Swagger/OpenAPI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { ... });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { ... });
});
```

### Uso en Program.cs

```csharp
// API/Program.cs
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

// ...
app.UseGlobalExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
```

La centralización de la configuración de DI en un único archivo permite localizar rápidamente cualquier dependencia del sistema y facilita la incorporación de nuevos módulos sin modificar `Program.cs`.

---

## 2.8 Trabajo Futuro

Durante el análisis de la implementación actual se identificaron dos áreas de deuda técnica deliberada en la arquitectura:

### 2.8.1 OrganizationId Hardcodeado en LoginAsync

El método `AuthService.LoginAsync()` contiene un identificador de organización fijo (`Guid.Parse("11111111-1111-1111-1111-111111111111")`) utilizado para localizar al usuario durante el proceso de autenticación. Esta decisión responde a la ausencia de un mecanismo de resolución de organización en la versión actual del sistema.

```csharp
// AuthService.cs — Deuda técnica documentada
var tempOrgId = Guid.Parse("11111111-1111-1111-1111-111111111111");
// TODO: Implementar lógica de resolución de organización
// (e.g., por subdominio, por cabecera HTTP o por campo en el cuerpo)
```

**Impacto:** En la versión actual, todos los intentos de autenticación se resuelven contra la organización con ese GUID predeterminado. El sistema de multitenencia funciona correctamente una vez que el token JWT es generado y distribuido; la limitación se circunscribe únicamente al proceso de login.

**Solución propuesta:** Implementar un servicio de resolución de tenant que infiera el `OrganizationId` a partir de un subdominio (`organizacion.devmanager.com`), una cabecera HTTP personalizada (`X-Organization-Id`) o un campo incluido en el cuerpo de la solicitud de autenticación.

### 2.8.2 Ausencia de Claims de Rol en el Token JWT

Las tablas `iam.Roles`, `iam.UserRoles`, `iam.Permissions` y `iam.RolePermissions` se encuentran completamente implementadas y pobladas en la base de datos. Sin embargo, el servicio `TokenService` no emite los roles del usuario como claims en el token JWT generado. Como consecuencia, los atributos `[Authorize(Roles = "...")]` en los controladores no pueden utilizarse de forma efectiva en la versión actual.

**Impacto:** El sistema de autorización basado en roles (RBAC) está diseñado y modelado en la base de datos, pero no se refleja en el token de autenticación. Todos los endpoints protegidos verifican únicamente que el token sea válido (`[Authorize]`), sin distinción de rol.

**Solución propuesta:** Modificar `TokenService.GenerateToken()` para incluir los roles del usuario como claims adicionales de tipo `ClaimTypes.Role`, consultando `iam.UserRoles` durante la generación del token.

---

## 2.9 Síntesis del Capítulo

El presente capítulo ha documentado los siete pilares arquitectónicos de DevManagerAPI:

1. **Clean Architecture** organiza el sistema en cuatro capas con dependencias unidireccionales, garantizando que el dominio permanezca libre de dependencias externas.
2. **Patrón Repositorio** desacopla la lógica de negocio del acceso a datos, con interfaces en `Domain` e implementaciones en `Infrastructure`.
3. **Patrón de Doble Entidad** preserva la pureza del dominio mediante la separación entre POCOs de negocio y modelos de Entity Framework Core, conectados por los métodos `MapToDomain()` y `MapToEntity()` en cada repositorio.
4. **Multitenencia por JWT** garantiza el aislamiento de datos por organización mediante el claim `OrganizationId` extraído del token Bearer y aplicado como filtro en cada consulta de repositorio. El patrón HITL (aprobación humana de acciones del agente) también respeta este aislamiento.
5. **Borrado Lógico Universal** mediante la clase `AuditableEntity` (7 campos de auditoría) elimina los `DELETE` físicos del sistema, preservando la trazabilidad histórica completa.
6. **Manejo Centralizado de Excepciones** con `GlobalExceptionHandlerMiddleware` y respuestas estandarizadas con `ApiResponse<T>` garantizan una interfaz de errores consistente para todos los consumidores de la API.
7. **Inyección de Dependencias Centralizada** en `ApplicationServiceExtensions.cs` concentra toda la configuración de servicios en un punto único y auditable.

Las deudas técnicas identificadas (resolución dinámica de tenant en login y emisión de claims de rol en JWT) se documentan como trabajo futuro con propuestas de solución concretas, sin afectar el funcionamiento de los módulos core del sistema en su versión actual.

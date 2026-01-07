# DevManagerAPI - Instrucciones para Agentes IA

## Dominio del Negocio: Sistema Multi-tenant de Gestión de Talento y Proyectos

**DevManager** es un sistema de gestión de talento, habilidades y asignación de proyectos con multi-tenancy por organización. El esquema SQL completo define el modelo de datos en [Infrastructure/Database/DDL/DDL_Dev_Manager.sql](Infrastructure/Database/DDL/DDL_Dev_Manager.sql).

### Agregados de Dominio (por esquema SQL)

1. **IAM (Identity & Access)** - `iam.*`
   - Organizations, Users, Roles, UserRoles
   - Multi-tenant: `OrganizationId` en todas las tablas
   - Auditoría completa: CreatedAt, UpdatedAt, IsDeleted, DeletedAt
   - Seguridad: PasswordHash/Salt en Users

2. **Talent** - `talent.*`
   - EmployeeProfiles, Skills, EmployeeSkills (niveles 1-5), Certifications
   - SkillEvaluations: sistema de tracking de evolución de skills con DeltaLevel

3. **Projects** - `projects.*`
   - Projects (Estados: Draft, Open, InProgress, Closed, Cancelled)
   - ProjectSkillRequirements, ProjectRoles, ProjectApplications
   - ProjectAssignments (asignaciones activas a proyectos)
   - ProjectParticipation: historial de contribuciones

4. **Reporting** - `reporting.*`
   - ReportSnapshots (métricas en JSON), RecommendationRules, RecommendationLogs

## Arquitectura Clean Architecture (.NET 8.0)

```
API/               → Controllers, Program.cs, appsettings (Presentación)
Application/       → Casos de uso, DTOs, interfaces (Lógica de aplicación)
Domain/            → Entidades puras, reglas de negocio (Núcleo)
Infrastructure/    → Repositorios, acceso a datos, servicios externos
```

**Reglas de dependencia**:
- Domain: Sin dependencias externas
- Application: Solo Domain
- API/Infrastructure: Pueden referenciar Application + Domain

## Convenciones Críticas

### Multi-tenancy
- **TODA consulta/comando** debe filtrar por `OrganizationId`
- Índices únicos incluyen `OrganizationId`: `UX_Users_Org_Email`, `UX_Projects_Org_Code`
- Foreign keys siempre validados dentro del contexto de la organización

### Auditoría y Soft Delete
- Todas las tablas transaccionales tienen: `CreatedAt`, `UpdatedAt`, `IsDeleted`, `DeletedAt`
- **NO eliminar físicamente**: siempre usar `IsDeleted = 1`
- Índices filtrados con `WHERE IsDeleted = 0`

### Tipos de datos SQL Server
- IDs: `uniqueidentifier` (Guid en C#)
- Timestamps: `datetime2(3)` con precisión milisegundos
- Defaults: `sysutcdatetime()` para timestamps
- Enums almacenados como `tinyint` con CHECK constraints

### Convención de Nombres
- Namespaces: `API.Controllers`, `Domain`, `Application`, `Infrastructure`
- Controllers: Sufijo `Controller`, hereda `ControllerBase`
- Entidades de dominio: Nombres singulares (User, Project, Skill)

## Setup y Desarrollo

### Prerequisitos
- .NET 8.0 SDK
- SQL Server (esquema en [DDL_Dev_Manager.sql](Infrastructure/Database/DDL/DDL_Dev_Manager.sql))

### Comandos
```bash
# Build completo
dotnet build DevManager.sln

# Ejecutar API (HTTPS: 7265, HTTP: 5073)
dotnet run --project API/API.csproj

# Swagger UI
https://localhost:7265/swagger
```

### Ejecutar DDL (primera vez)
```sql
-- Crear base de datos y ejecutar script completo
CREATE DATABASE DevManager;
GO
USE DevManager;
GO
-- Ejecutar Infrastructure/Database/DDL/DDL_Dev_Manager.sql
```

## Guía de Implementación

### Agregar un nuevo agregado (ejemplo: Organization CRUD)

1. **Domain** - Crear entidad siguiendo el modelo SQL:
```csharp
namespace Domain;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LegalName { get; set; }
    public string? Nit { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    // ... campos de auditoría
}
```

2. **Domain** - Interface de repositorio:
```csharp
namespace Domain;
public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id);
    Task<IEnumerable<Organization>> GetAllAsync();
    // ...
}
```

3. **Infrastructure** - Implementar repositorio (ADO.NET, Dapper o EF Core según se decida)

4. **Application** - DTOs y servicios:
```csharp
namespace Application;
public record CreateOrganizationDto(string Name, string? LegalName, string? Nit);
```

5. **API** - Controller:
```csharp
namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganizationsController : ControllerBase
{
    // Inyectar servicios de Application
}
```

6. **Program.cs** - Registrar dependencias:
```csharp
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
```

## Estado Actual del Proyecto (Actualizado: 6 Enero 2026)

### ✅ Completado (100% - MVP Base)
- ✅ Clean Architecture implementada (4 capas)
- ✅ Esquema SQL completo (541 líneas, 18 tablas, 4 esquemas)
- ✅ Multi-tenancy implementado y funcional
- ✅ Dapper 2.1.66 configurado con Stored Procedures
- ✅ JWT Authentication (Bearer + Claims)
- ✅ Password hashing (HMACSHA512 + Salt)
- ✅ Manejo de errores global y estandarizado
- ✅ Módulo IAM completo (Auth + Users)
  - 18 entidades de dominio
  - 5 enumeraciones
  - 12 DTOs
  - 3 servicios
  - 3 repositorios con Dapper
  - 2 controllers (7 endpoints)
  - 7 stored procedures
- ✅ Swagger UI con JWT authentication
- ✅ Documentación completa (5 archivos markdown)

### ⚠️ Pendiente (Siguientes fases)
- ⚠️ Módulo Talent (tablas existen, falta implementar servicios)
- ⚠️ Módulo Projects (tablas existen, falta implementar servicios)
- ⚠️ Módulo Reporting (tablas existen, falta implementar servicios)
- ⚠️ Tests unitarios e integración
- ⚠️ FluentValidation
- ⚠️ Paginación y filtros avanzados

## Próximos Pasos Recomendados

### Para implementar módulos Talent/Projects (seguir este patrón):
1. Crear DTOs en `Application/DTOs/[Modulo]/`
2. Crear interfaces de servicio en `Application/Interfaces/`
3. Implementar servicios en `Application/Services/`
4. Crear repositorios en `Infrastructure/Repositories/`
5. Crear stored procedures en `Infrastructure/Database/StoredProcedures/`
6. Crear controllers en `API/Controllers/`
7. Registrar en `ApplicationServiceExtensions.cs`

### Referencias de código existente:
- **Service pattern:** `Application/Services/AuthService.cs` y `UserService.cs`
- **Repository pattern:** `Infrastructure/Repositories/AuthRepository.cs` y `UserRepository.cs`
- **Controller pattern:** `API/Controllers/AuthController.cs` y `UsersController.cs`
- **Stored Procedures:** `Infrastructure/Database/StoredProcedures/01_IAM_Procedures.sql`
- **DTOs:** `Application/DTOs/Auth/` y `Application/DTOs/Users/`
- **Middleware:** `API/Middleware/GlobalExceptionHandlerMiddleware.cs`
- **Extensions:** `API/Extensions/ApplicationServiceExtensions.cs`

### Documentación de referencia:
- `README.md` - Guía principal
- `ESTADO_PROYECTO.md` - Estado detallado y checklist
- `SETUP_DATABASE.md` - Configuración de base de datos
- `API_EXAMPLES.md` - Ejemplos de uso con curl, PowerShell, C#
- `RESUMEN_EJECUTIVO.md` - Overview ejecutivo del proyecto

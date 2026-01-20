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
   - **AgentActions** - Auditoría de acciones del agente IA
   - **AgentConfiguration** - Configuración del agente por organización

5. **Agent** - Agente Cognitivo de IA (NUEVO v2.0)
   - Integración con Google Gemini AI (gemini-2.5-flash-lite)
   - Model Context Protocol (MCP) - Tool Use Pattern
   - Chain of Thought (CoT) reasoning implementado
   - HITL (Human-in-the-loop) para decisiones críticas
   - Background services para procesamiento asíncrono

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
-- Ejecutar Infrastructure/Database/DDL/DDL_Agent_Tables.sql (NUEVO v2.0)
```

### Configurar Agente IA (NUEVO v2.0)
```json
// API/appsettings.json
{
  "GoogleAI": {
    "ApiKey": "YOUR_GOOGLE_AI_API_KEY_HERE",
    "Model": "gemini-1.5-flash"
  },
  "Agent": {
    "EnableBackgroundServices": true,
    "RequireHumanApproval": true
  }
}
```

**Obtener API Key**: https://aistudio.google.com/app/apikey

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

3. **Infrastructure** - Implementar repositorio usando Entity Framework Core

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

## Estado Actual del Proyecto (Actualizado: 19 Enero 2026)

### ✅ Completado v2.0 - Agente IA Implementado
- ✅ Clean Architecture implementada (4 capas)
- ✅ Esquema SQL completo (18 tablas + 2 tablas de agente)
- ✅ Multi-tenancy implementado y funcional
- ✅ Entity Framework Core 8.0.11
- ✅ JWT Authentication (Bearer + Claims)
- ✅ Password hashing (HMACSHA512 + Salt)
- ✅ Manejo de errores global y estandarizado
- ✅ Módulo IAM completo (Auth + Users)
- ✅ **Agente Cognitivo de IA** (NUEVO)
  - Google Gemini AI integrado (gemini-2.5-flash-lite)
  - 5 endpoints del agente (/agent/*)
  - Validación semántica de skills
  - Matching inteligente de candidatos
  - Consultas en lenguaje natural
  - HITL (Human-in-the-loop)
  - Background services (snapshots + optimizer)
  - Auditoría completa (reporting.AgentActions)
  - Model Context Protocol (MCP)
  - Chain of Thought (CoT) reasoning
- ✅ Swagger UI con JWT authentication
- ✅ Documentación completa (8+ archivos markdown)

### ⚠️ Pendiente (Siguientes fases)
- ⚠️ Módulo Talent completo (para matching real con datos)
- ⚠️ Módulo Projects completo (para matching real con proyectos)
- ⚠️ Vector embeddings para semantic search
- ⚠️ Fine-tuning del agente con datos históricos
- ⚠️ Tests unitarios e integración
- ⚠️ FluentValidation
- ⚠️ Dashboard de métricas del agente

## Próximos Pasos Recomendados

### Para implementar módulos Talent/Projects (seguir este patrón):
1. Crear DTOs en `Application/DTOs/[Modulo]/`
2. Crear interfaces de servicio en `Application/Interfaces/`
3. Implementar servicios en `Application/Services/`
4. Crear repositorios en `Infrastructure/Repositories/`
5. Configurar entidades en `Infrastructure/Data/Configuration/`
6. Crear controllers en `API/Controllers/`
7. Registrar en `ApplicationServiceExtensions.cs`

### Referencias de código existente:
- **Service pattern:** `Application/Services/AuthService.cs`, `UserService.cs`, `AgentService.cs`
- **Repository pattern:** `Infrastructure/Repositories/AuthRepository.cs`, `UserRepository.cs`, `AgentRepository.cs`
- **Controller pattern:** `API/Controllers/AuthController.cs`, `UsersController.cs`, `AgentController.cs`
- **AI Integration:** `Infrastructure/Services/AI/GeminiService.cs` (Google Gemini API)
- **Background Services:** `Infrastructure/BackgroundServices/` (IHostedService)
- **Entity Configuration:** `Infrastructure/Data/Configuration/` (Fluent API)
- **DTOs:** `Application/DTOs/Auth/`, `Application/DTOs/Users/`, `Application/DTOs/Agent/`
- **Middleware:** `API/Middleware/GlobalExceptionHandlerMiddleware.cs`
- **Extensions:** `API/Extensions/ApplicationServiceExtensions.cs`

### Documentación de referencia:
- `README.md` - Guía principal
- `AGENT_GUIDE.md` - Guía completa del agente IA (500+ líneas)
- `QUICKSTART_AGENT.md` - Setup rápido del agente
- `IMPLEMENTATION_SUMMARY.md` - Resumen de implementación v2.0
- `Examples/AgentClientExample.cs` - Cliente C# de ejemplo
- `ESTADO_PROYECTO.md` - Estado detallado y checklist
- `SETUP_DATABASE.md` - Configuración de base de datos
- `API_EXAMPLES.md` - Ejemplos de uso con curl, PowerShell, C#
- `RESUMEN_EJECUTIVO.md` - Overview ejecutivo del proyecto

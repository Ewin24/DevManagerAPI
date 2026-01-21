# DevManagerAPI - AI Agent Instructions

## System Overview: Multi-tenant Talent & Project Management

**DevManager** is a talent management, skills tracking, and project assignment system with organization-level multi-tenancy and embedded AI agent capabilities (Google Gemini). Database schema: [Infrastructure/Database/DDL/DDL_Dev_Manager.sql](Infrastructure/Database/DDL/DDL_Dev_Manager.sql)

## Architecture: Clean Architecture (.NET 8.0)

```
API/               → Controllers, middleware, DI registration (Presentation)
Application/       → Use cases, DTOs, service interfaces (Business Logic)
Domain/            → Pure entities, business rules, repository interfaces (Core)
Infrastructure/    → EF Core repos, external services (AI, background jobs)
```

**Dependency Rules (strictly enforced)**:
- `Domain`: NO external dependencies (pure C# POCOs)
- `Application`: References `Domain` only
- `API` + `Infrastructure`: Can reference `Application` + `Domain`

### Key Files
- [Program.cs](API/Program.cs): Startup, middleware pipeline (order matters: exception handler → auth → CORS)
- [ApplicationServiceExtensions.cs](API/Extensions/ApplicationServiceExtensions.cs): DI registration (services, repos, background jobs)
- [DevManagerDbContext.cs](Infrastructure/Data/DevManagerDbContext.cs): EF Core context with automatic configuration discovery

## Critical Patterns (THIS PROJECT SPECIFIC)

### 1. Multi-tenancy (MANDATORY)
**Every query/command MUST filter by `OrganizationId`** extracted from JWT claims:
```csharp
// In Controllers - get from JWT claims
private Guid GetOrganizationId() =>
    Guid.Parse(User.FindFirst("OrganizationId")?.Value!);

private Guid GetUserId() =>
    Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

// In Repositories - ALWAYS add .Where()
await _context.Users
    .Where(u => u.OrganizationId == organizationId && !u.IsDeleted)
    .FirstOrDefaultAsync();
```
Unique indexes include `OrganizationId`: `UX_Users_Org_Email`, `UX_Projects_Org_Code`

### 2. Soft Delete (NEVER physical delete)
All entities inherit [AuditableEntity](Domain/Common/AuditableEntity.cs):
```csharp
public abstract class AuditableEntity {
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }  // ← Use this, never DELETE
    public DateTime? DeletedAt { get; set; }
}
```
**Always** use `.Where(x => !x.IsDeleted)` in queries

### 3. Standardized API Responses
Use [ApiResponse<T>](Application/Common/Models/ApiResponse.cs) for success:
```csharp
return Ok(new ApiResponse<UserDto> {
    Success = true,
    Message = "Usuario obtenido exitosamente",
    Data = userDto
});
```
Throw [custom exceptions](Application/Common/Exceptions/ApplicationExceptions.cs) for errors (middleware handles them):
```csharp
throw new NotFoundException("User", userId);
throw new ConflictException("Email ya registrado");
throw new UnauthorizedException("Token inválido");
```
[GlobalExceptionHandlerMiddleware](API/Middleware/GlobalExceptionHandlerMiddleware.cs) converts to ErrorResponse automatically.

### 4. Service Registration Pattern
ALL services/repos registered in [ApplicationServiceExtensions.cs](API/Extensions/ApplicationServiceExtensions.cs):
```csharp
// Repos
services.AddScoped<IUserRepository, UserRepository>();

// Services
services.AddScoped<IUserService, UserService>();

// AI Services (HttpClient injected)
services.AddHttpClient<IGeminiService, GeminiService>();

// Background Services (IHostedService)
services.AddHostedService<ReportSnapshotGeneratorService>();
```

### 5. EF Core Configuration
- DbContext: [DevManagerDbContext.cs](Infrastructure/Data/DevManagerDbContext.cs)
- Auto-discovers configurations: `modelBuilder.ApplyConfigurationsFromAssembly(typeof(DevManagerDbContext).Assembly)`
- Entities in `Infrastructure.Data.Entities` (EF Core models)
- Domain entities in `Domain/Entities/` (pure POCOs)
- Repositories map between them

## AI Agent Integration (Google Gemini)

**5 Agent Endpoints** in [AgentController.cs](API/Controllers/AgentController.cs):
- `POST /agent/query` - Natural language queries
- `POST /agent/validate-skill` - Semantic skill validation
- `POST /agent/match-candidates` - Intelligent candidate matching
- `POST /agent/actions/{actionId}/approve` - HITL approval
- `POST /agent/actions/{actionId}/reject` - HITL rejection

[AgentService.cs](Application/Services/AgentService.cs) orchestrates MCP Tool Use pattern:
1. Extract OrganizationId from JWT
2. Call Gemini with Chain of Thought prompt
3. Log action to `reporting.AgentActions` (audit trail)
4. If `RequireApproval=true`, return `ActionId` for HITL workflow

[GeminiService.cs](Infrastructure/Services/AI/GeminiService.cs):
```csharp
// Simple query
var response = await _geminiService.QueryAsync(prompt);

// With reasoning (CoT)
var (response, reasoning) = await _geminiService.QueryWithReasoningAsync(prompt);
```

Background services run every 24 hours (configurable in appsettings):
- [ReportSnapshotGeneratorService.cs](Infrastructure/BackgroundServices/ReportSnapshotGeneratorService.cs)
- [RecommendationOptimizerService.cs](Infrastructure/BackgroundServices/RecommendationOptimizerService.cs)

## Development Workflow

### Build & Run
```bash
# Full solution build
dotnet build DevManager.sln

# Run API (https://localhost:7265)
dotnet run --project API/API.csproj

# Swagger UI
https://localhost:7265/swagger
```

### Database Setup
1. Create database: `CREATE DATABASE DevManager;`
2. Execute DDL: `Infrastructure/Database/DDL/DDL_Dev_Manager.sql`
3. Execute Agent tables: `Infrastructure/Database/DDL/DDL_Agent_Tables.sql`
4. Update connection string in `API/appsettings.json`

### AI Agent Configuration
```json
{
  "GoogleAI": {
    "ApiKey": "YOUR_KEY_HERE",  // Get from https://aistudio.google.com/app/apikey
    "Model": "gemini-1.5-flash"
  },
  "Agent": {
    "EnableBackgroundServices": true,
    "RequireHumanApproval": true
  }
}
```

### Testing Agent Endpoints
See [API_EXAMPLES.md](API/API_EXAMPLES.md) for curl/PowerShell/C# examples.
PowerShell script: [Scripts/Test-Agent.ps1](Scripts/Test-Agent.ps1)

## Adding New Functionality

### Example: Add Organization CRUD

1. **Domain** - Entity:
```csharp
namespace Domain.Entities.IAM;
public class Organization {
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
```

2. **Domain** - Repository interface:
```csharp
namespace Domain.Interfaces.Repositories;
public interface IOrganizationRepository {
    Task<Organization?> GetByIdAsync(Guid id);
}
```

3. **Infrastructure** - Repository implementation:
```csharp
namespace Infrastructure.Repositories;
public class OrganizationRepository : IOrganizationRepository {
    private readonly DevManagerDbContext _context;
    // Implementation using _context.Organizations...
}
```

4. **Application** - DTO:
```csharp
namespace Application.DTOs.Organizations;
public record CreateOrganizationDto(string Name, string? LegalName);
```

5. **Application** - Service interface & implementation:
```csharp
namespace Application.Interfaces;
public interface IOrganizationService {
    Task<OrganizationDto> GetByIdAsync(Guid id, Guid organizationId);
}

namespace Application.Services;
public class OrganizationService : IOrganizationService { /* ... */ }
```

6. **API** - Controller:
```csharp
namespace API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase {
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) {
        var orgId = GetOrganizationId();  // ← Multi-tenancy!
        var result = await _service.GetByIdAsync(id, orgId);
        return Ok(ApiResponse<OrganizationDto>.SuccessResponse(result));
    }
}
```

7. **Registration** - [ApplicationServiceExtensions.cs](API/Extensions/ApplicationServiceExtensions.cs):
```csharp
services.AddScoped<IOrganizationRepository, OrganizationRepository>();
services.AddScoped<IOrganizationService, OrganizationService>();
```

## SQL Server Conventions
- IDs: `uniqueidentifier` (Guid in C#)
- Timestamps: `datetime2(3)` with millisecond precision
- Defaults: `sysutcdatetime()` for audit fields
- Enums: `tinyint` with CHECK constraints
- Indexes: Filtered with `WHERE IsDeleted = 0`

## Reference Implementations
- **Service pattern**: [AuthService.cs](Application/Services/AuthService.cs), [UserService.cs](Application/Services/UserService.cs), [AgentService.cs](Application/Services/AgentService.cs)
- **Repository pattern**: [AuthRepository.cs](Infrastructure/Repositories/AuthRepository.cs), [UserRepository.cs](Infrastructure/Repositories/UserRepository.cs)
- **Controller pattern**: [AuthController.cs](API/Controllers/AuthController.cs), [UsersController.cs](API/Controllers/UsersController.cs), [AgentController.cs](API/Controllers/AgentController.cs)
- **AI Integration**: [GeminiService.cs](Infrastructure/Services/AI/GeminiService.cs), [AgentService.cs](Application/Services/AgentService.cs)
- **Background Services**: [ReportSnapshotGeneratorService.cs](Infrastructure/BackgroundServices/ReportSnapshotGeneratorService.cs)

## Documentation
- [README.md](README.md) - Main guide
- [AGENT_GUIDE.md](AGENT_GUIDE.md) - Complete AI agent guide (500+ lines)
- [AGENT_TESTING_GUIDE.md](AGENT_TESTING_GUIDE.md) - Testing guide
- [HITL_WORKFLOW_GUIDE.md](HITL_WORKFLOW_GUIDE.md) - Human-in-the-loop workflow
- [API_EXAMPLES.md](API/API_EXAMPLES.md) - curl, PowerShell, C# examples
- [CHANGELOG.md](CHANGELOG.md) - Version history

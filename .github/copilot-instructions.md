# DevManagerAPI - AI Coding Agent Instructions

> .NET 8.0 Clean Architecture API for talent management with multi-tenancy and Google Gemini AI integration.

## Architecture

| Layer | Purpose | Dependencies |
|-------|---------|--------------|
| `Domain/` | Entities, enums, repo interfaces | None |
| `Application/` | Services, DTOs, business logic | Domain only |
| `Infrastructure/` | EF Core, AI services, background jobs | Domain + Application |
| `API/` | Controllers, middleware, DI | All layers |

**Key files**: [Program.cs](API/Program.cs) (middleware order matters), [ApplicationServiceExtensions.cs](API/Extensions/ApplicationServiceExtensions.cs) (ALL DI here)

## Critical Patterns

### 1. Multi-tenancy — Filter EVERY query by OrganizationId
```csharp
// Controller: extract from JWT claim
var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

// Repository: ALWAYS filter + soft delete check
await _context.Users.Where(u => u.OrganizationId == organizationId && !u.IsDeleted)...
```

### 2. Soft Delete — NEVER physical DELETE
All entities inherit `AuditableEntity` (IsDeleted, DeletedAt, DeletedByUserId). Use `SoftDeleteAsync()` in repos.

### 3. Standardized API Responses
```csharp
// Success → ApiResponse<T>
return Ok(ApiResponse<ProjectResponse>.SuccessResponse(data));

// Errors → throw exceptions (GlobalExceptionHandlerMiddleware handles them)
throw new NotFoundException("Project", projectId);  // 404
throw new ConflictException("Email ya existe");     // 409
throw new UnauthorizedException();                  // 401
throw new ForbiddenException("Sin permisos");       // 403
```

### 4. Dual Entity Pattern (Domain ↔ Infrastructure)
- `Domain/Entities/{Module}/` = Pure POCOs, business logic
- `Infrastructure/Data/Entities/` = EF Core models with navigation properties
- Repositories map between them with `MapToDomain()` / `MapToEntity()` methods

## Adding New Features

1. **Domain**: Entity in `Domain/Entities/{Module}/`, interface in `Domain/Interfaces/Repositories/`
2. **Infrastructure**: EF entity in `Infrastructure/Data/Entities/`, repository in `Infrastructure/Repositories/`
3. **Application**: DTOs in `Application/DTOs/{Module}/`, service interface in `Application/Interfaces/`, implementation in `Application/Services/`
4. **API**: Controller with `[Authorize]`, extract `OrganizationId` from JWT
5. **DI**: Register in [ApplicationServiceExtensions.cs](API/Extensions/ApplicationServiceExtensions.cs)

## AI Agent (Google Gemini)

- **Orchestrator**: [AgentService.cs](Application/Services/AgentService.cs) — Chain-of-Thought + Tool Use pattern
- **AI calls**: `IGeminiService.QueryWithReasoningAsync()` returns (response, reasoning)
- **HITL**: Actions logged to `reporting.AgentActions`, approve via `/agent/actions/{id}/approve`
- **Background**: `ReportSnapshotGeneratorService` (24h), `RecommendationOptimizerService` (6h)

## Commands

```bash
dotnet build DevManager.sln                    # Build all
dotnet run --project API/API.csproj            # Run → https://localhost:7265
sqlcmd -S localhost -d DevManager -E -i Infrastructure/Database/DDL/DDL_Dev_Manager.sql   # Init DB
sqlcmd -S localhost -d DevManager -E -i Infrastructure/Database/Seeders/Seeder.sql        # Seed test data
```

**Test credentials**: `admin@techcorp.com` / `Password123!`

## Reference Examples

| Pattern | File |
|---------|------|
| Controller | [ProjectsController.cs](API/Controllers/ProjectsController.cs) |
| Service | [UserService.cs](Application/Services/UserService.cs) |
| Repository | [UserRepository.cs](Infrastructure/Repositories/UserRepository.cs) |
| Exceptions | [ApplicationExceptions.cs](Application/Common/Exceptions/ApplicationExceptions.cs) |
| AI Agent | [AgentService.cs](Application/Services/AgentService.cs) |

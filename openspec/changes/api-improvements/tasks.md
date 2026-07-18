# Tasks: API Improvements (Roles, Users, Reports)

## Overview
Implementation of three key improvements: Roles module enhancement with `userCount`, Users module null-safety for `roleName`, and new Reports endpoints for skills distribution, project metrics, and AI summaries.

---

## Phase 1: Infrastructure & DTOs Foundation

- [x] 1.1 Create `Application/DTOs/Reports/ReportsDtos.cs` with response types: `SkillDistributionResponse`, `SkillLevelDistribution`, `ProjectMetricsResponse`, `MostDemandedSkill`, `AiSummaryResponse`
- [x] 1.2 Update `Application/DTOs/RolesPermissions/RolesPermissionsDtos.cs` - add `UserCount` property to `RoleSummaryResponse` (already exists, verify it's populated)
- [x] 1.3 Update `Application/DTOs/Users/UserDTOs.cs` - add `RoleId` (Guid?) to `UserResponse` and ensure `RoleName` has default value `"Sin asignar"` when null
- [x] 1.4 Create `Application/Interfaces/IReportsService.cs` with methods: `GetSkillsDistributionAsync()`, `GetProjectMetricsAsync()`, `GetAiSummaryAsync()`

---

## Phase 2: Database & Service Layer

- [x] 2.1 Update `RolePermissionService.cs` - modify `GetAllRolesAsync()` to include user count per role via SQL COUNT or LINQ join with Users table
- [x] 2.2 Update `UserService.cs` - ensure `GetAllAsync()` and `GetByIdAsync()` return DTO with `RoleName` set to `"Sin asignar"` when `roleId` is null
- [x] 2.3 Create `Application/Services/ReportsService.cs` implementing `IReportsService` interface with three methods:
  - `GetSkillsDistributionAsync(Guid organizationId)` - GROUP BY skill, calculate averageLevel and levelDistribution
  - `GetProjectMetricsAsync(Guid organizationId)` - COUNT active projects, identify at-risk projects, list most demanded skills
  - `GetAiSummaryAsync(Guid organizationId)` - call existing AgentService to generate markdown summary
- [x] 2.4 Register `IReportsService` in `API/Extensions/ApplicationServiceExtensions.cs` dependency injection

---

## Phase 3: API Controller & Endpoints

- [x] 3.1 Create `API/Controllers/ReportsController.cs` with [Authorize] attribute and GetOrganizationId() helper
- [x] 3.2 Add `GET /api/reports/skills-distribution` endpoint - returns `ApiResponse<IEnumerable<SkillDistributionResponse>>` (200 OK)
- [x] 3.3 Add `GET /api/reports/project-metrics` endpoint - returns `ApiResponse<ProjectMetricsResponse>` (200 OK)
- [x] 3.4 Add `GET /api/reports/ai-summary` endpoint - returns `ApiResponse<AiSummaryResponse>` (200 OK)
- [x] 3.5 Add ProducesResponseType attributes and XML documentation comments to all three endpoints

---

## Phase 4: Integration & Testing

- [ ] 4.1 Update `RolesController.GetAll()` - verify it now returns RoleSummaryResponse with populated UserCount
- [ ] 4.2 Test `GET /api/roles` - confirm userCount appears in response for each role
- [ ] 4.3 Test `GET /api/users` and `GET /api/users/{id}` - verify roleName is "Sin asignar" (not null) when user has no role
- [ ] 4.4 Test `GET /api/reports/skills-distribution` - verify response structure matches proposal JSON with skill distribution by level
- [ ] 4.5 Test `GET /api/reports/project-metrics` - verify active projects count, at-risk projects, and most demanded skills
- [ ] 4.6 Test `GET /api/reports/ai-summary` - verify markdown summary is generated and returned
- [ ] 4.7 Test `GET /api/reports/employee-utilization` - verify employee allocation status and utilization percentages
- [ ] 4.8 Test `GET /api/reports/department-metrics` - verify team statistics and experience distribution
- [ ] 4.9 Verify multi-tenancy: All endpoints filter by OrganizationId from JWT claim

---

## Phase 5: Documentation & Verification

- [ ] 5.1 Add XML documentation to `ReportsService.cs` methods explaining business logic (GROUP BY, skill aggregation logic)
- [ ] 5.2 Add remarks to ReportsController endpoints with example requests and responses matching proposal JSON
- [ ] 5.3 Add remarks for new endpoints (employee-utilization, department-metrics) with examples
- [ ] 5.4 Update `API_GUIDE.md` with complete Reports section covering all 5 endpoints
- [ ] 5.5 Verify all three improvements align with proposal requirements in `docs/02_Propuesta_Mejoras_API.md`
- [ ] 5.6 Test edge cases: organization with no skills, no projects, no users with skills - ensure endpoints gracefully return empty or zero values
- [ ] 5.7 Verify multi-tenancy filtering on both new endpoints (employee-utilization, department-metrics)

---

## Implementation Notes

**Dependency Order:**
1. Phase 1 must complete first (DTOs and interfaces define the contract)
2. Phase 2 depends on Phase 1 (services implement interfaces using DTOs)
3. Phase 3 depends on Phase 1 & 2 (controllers inject services and use DTOs)
4. Phase 4 tests the integration (ensures all layers work together)
5. Phase 5 documents and verifies against proposal

**Key Technical Decisions:**
- `RoleSummaryResponse.UserCount` already exists in codebase - verify it's populated in GetAllRolesAsync()
- `UserResponse.RoleName` must default to `"Sin asignar"` in mapper (not null) to avoid frontend null checks
- Skills distribution requires database JOIN across EmployeeSkills, Skills, and skill levels
- Project metrics requires filtering active projects and checking required skills vs available employees
- AI summary reuses existing `AgentService.QueryAsync()` with a pre-packaged organizational analysis prompt
- **NEW:** Employee utilization calculated from ProjectAssignment entries, with percentage estimation (1 project ~25%, max 100%)
- **NEW:** Department metrics aggregates EmployeeProfile and EmployeeSkill data for organization-wide statistics

**Additional Endpoints Added (Phase 4+):**
1. `GET /api/reports/employee-utilization` - Returns EmployeeUtilizationResponse with per-employee allocation status
2. `GET /api/reports/department-metrics` - Returns DepartmentMetricsResponse with organizational statistics

**Testing Priority:**
- Null-safety for roleName (prevents frontend UI crashes)
- User count accuracy (verify COUNT query includes active users only)
- Skills distribution aggregation (verify GROUP BY level distribution)
- Employee utilization calculation (verify allocation percentages and project counting)
- Department metrics aggregation (verify experience distribution and skill averages)

# Changelog - DevManager API

Todos los cambios notables de este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

---

## [1.0.0] - 2026-01-06

### 🎉 Versión Inicial - MVP Base Completo

#### ✨ Agregado

**Infraestructura**
- Clean Architecture implementada (4 capas: API, Application, Domain, Infrastructure)
- Configuración de solución .NET 8.0 con 4 proyectos
- Dapper 2.1.66 como micro ORM
- Microsoft.Data.SqlClient 6.1.3 para SQL Server
- DapperContext para gestión de conexiones
- Patrón Repository implementado
- Dependency Injection configurada

**Seguridad y Autenticación**
- JWT Bearer Authentication (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.11)
- TokenService para generación de JWT
- Password hashing con HMACSHA512
- Salt único por usuario
- Claims personalizados (UserId, Email, Name, OrganizationId)
- Middleware de autenticación configurado
- Swagger UI con soporte JWT Bearer

**Manejo de Errores**
- GlobalExceptionHandlerMiddleware para manejo centralizado
- Jerarquía de excepciones custom:
  - ApplicationException (base)
  - NotFoundException (404)
  - ConflictException (409)
  - UnauthorizedException (401)
  - ForbiddenException (403)
  - BusinessValidationException (400)
- ApiResponse<T> para respuestas exitosas estandarizadas
- ErrorResponse para errores estandarizados
- Logging automático de errores

**Domain Layer**
- 18 entidades de dominio:
  - **IAM:** Organization, User, Role, UserRole
  - **Talent:** EmployeeProfile, Skill, EmployeeSkill, Certification, SkillEvaluation
  - **Projects:** Project, ProjectSkillRequirement, ProjectRole, ProjectApplication, ProjectAssignment, ProjectParticipation
  - **Reporting:** ReportSnapshot, RecommendationRule, RecommendationLog
- 5 enumeraciones:
  - ProjectStatus
  - ProjectComplexity
  - ApplicationStatus
  - AssignmentStatus
  - SkillEvaluationSource
- AuditableEntity base class con auditoría completa
- 4 interfaces de repositorio (IAM, Talent, Projects, Reporting)

**Application Layer**
- 12 DTOs organizados por módulo:
  - Auth: LoginRequest, LoginResponse, RegisterOrganizationRequest, RegisterOrganizationResponse
  - Users: CreateUserRequest, UpdateUserRequest, UserResponse
  - Profiles, Skills, Projects, Applications, Assignments, Feedback
- 3 servicios implementados:
  - AuthService (login, registro, password hashing)
  - UserService (CRUD completo)
  - TokenService (generación JWT)
- 3 interfaces de servicio (IAuthService, IUserService, ITokenService)

**Infrastructure Layer**
- 3 repositorios con Dapper:
  - AuthRepository (GetUserByEmail, RegisterOrganization)
  - UserRepository (CRUD + EmailExists + UpdateLastLogin)
  - DapperContext (connection factory)
- Stored Procedures pattern implementado
- Transacciones con BEGIN/COMMIT/ROLLBACK
- DynamicParameters para OUTPUT parameters

**API Layer**
- 2 controllers funcionales:
  - AuthController ([AllowAnonymous] Login, RegisterOrganization)
  - UsersController ([Authorize] CRUD completo)
- 7 endpoints funcionales:
  - POST /auth/login
  - POST /auth/register
  - GET /users
  - GET /users/{id}
  - POST /users
  - PUT /users/{id}
  - DELETE /users/{id}
- GlobalExceptionHandlerMiddleware registrado
- ApplicationServiceExtensions para DI limpia
- CORS configurado
- Swagger/OpenAPI con documentación completa

**Base de Datos**
- DDL completo: 541 líneas, 18 tablas, 4 esquemas
- Multi-tenancy implementado (OrganizationId en todas las tablas)
- Índices optimizados con filtros (WHERE IsDeleted = 0)
- Constraints e integridad referencial
- Auditoría completa (CreatedAt, UpdatedAt, IsDeleted, etc.)
- Soft delete en todas las tablas
- 7 Stored Procedures (módulo IAM):
  - sp_Iam_GetUserByEmail
  - sp_Iam_RegisterOrganization (transaccional)
  - sp_Iam_GetUsers
  - sp_Iam_GetUserById
  - sp_Iam_CreateUser
  - sp_Iam_UpdateUser
  - sp_Iam_SoftDeleteUser

**Documentación**
- README.md: Guía principal completa
- ESTADO_PROYECTO.md: Estado detallado con checklist
- SETUP_DATABASE.md: Instrucciones de configuración DB
- API_EXAMPLES.md: Ejemplos curl, PowerShell, C#, JavaScript
- RESUMEN_EJECUTIVO.md: Overview ejecutivo
- .github/copilot-instructions.md: Guía para agentes IA
- CHANGELOG.md: Historial de cambios

**Configuración**
- appsettings.json con ConnectionStrings y JwtSettings
- appsettings.Development.json con configuración dev
- .gitignore completo para .NET
- launchSettings.json configurado (HTTP 5073, HTTPS 7265)

#### 🔧 Técnico

**Arquitectura**
- Separación de concerns (Clean Architecture)
- Dependency Inversion Principle
- Repository Pattern
- Service Layer Pattern
- DTO Pattern
- Claims-based Authorization

**Multi-tenancy**
- OrganizationId en todas las tablas
- Filtrado automático por organización
- JWT claims incluyen OrganizationId
- Índices únicos compuestos con OrganizationId
- Validación en capa de repositorio

**Performance**
- Dapper para performance óptima
- Stored Procedures pre-compilados
- Índices optimizados
- Connection pooling configurado
- Async/await en todas las operaciones IO

#### 📊 Métricas

- **Archivos de código:** ~64 archivos .cs
- **Archivos de documentación:** 7 archivos .md
- **Archivos SQL:** 2 scripts
- **Líneas de código:** ~3,500
- **Endpoints funcionales:** 7
- **Stored Procedures:** 7
- **Tablas:** 18
- **Entidades de dominio:** 18
- **DTOs:** 12
- **Tests:** 0 (pendiente)

#### 🐛 Corregido

- Dependencia circular Domain → Application removida
- Referencias de proyecto correctamente configuradas
- Package version conflicts resueltos (JWT 8.0.11 vs 10.0.1)
- Archivos template (WeatherForecast) eliminados

---

## [Unreleased] - Próximas versiones

### 🚀 Por Hacer

**Módulo Talent**
- [ ] Implementar ProfileService y ProfileRepository
- [ ] Implementar SkillService y SkillRepository
- [ ] Implementar EmployeeSkillService con lógica de niveles
- [ ] Implementar CertificationService
- [ ] Implementar SkillEvaluationService con tracking
- [ ] Crear ~15 Stored Procedures
- [ ] Crear 5 Controllers
- [ ] Agregar DTOs faltantes

**Módulo Projects**
- [ ] Implementar ProjectService con estados
- [ ] Implementar ApplicationService con workflow
- [ ] Implementar AssignmentService (asignaciones activas)
- [ ] Implementar ParticipationService (historial)
- [ ] Implementar FeedbackService (lógica de agente inteligente)
- [ ] Crear ~18 Stored Procedures
- [ ] Crear 6 Controllers
- [ ] Lógica de recomendación de skills

**Módulo Reporting**
- [ ] Implementar ReportService con snapshots JSON
- [ ] Implementar RecommendationRules configurables
- [ ] Implementar RecommendationLogs audit trail
- [ ] Dashboards básicos
- [ ] Crear ~5 Stored Procedures
- [ ] Crear 1 Controller

**Testing**
- [ ] Tests unitarios con xUnit
- [ ] Tests de integración
- [ ] Tests de repositorios con TestContainers
- [ ] Tests de servicios con Moq
- [ ] Tests de controllers con WebApplicationFactory

**Validación**
- [ ] FluentValidation para DTOs
- [ ] Validaciones de negocio complejas
- [ ] Validaciones cross-field

**Features Adicionales**
- [ ] Paginación en listados
- [ ] Filtros y búsqueda
- [ ] Ordenamiento dinámico
- [ ] Rate limiting
- [ ] Health checks
- [ ] Roles y permisos (iam.Roles, iam.UserRoles)
- [ ] Refresh tokens
- [ ] Email service
- [ ] Logging estructurado (Serilog)
- [ ] Cache distribuida (Redis)
- [ ] Background jobs (Hangfire)

**DevOps**
- [ ] Dockerfile
- [ ] docker-compose.yml
- [ ] CI/CD pipeline (GitHub Actions)
- [ ] Azure deployment scripts
- [ ] Monitoring y métricas (Application Insights)
- [ ] Backup y disaster recovery

---

## Versionado

El proyecto usa [Semantic Versioning](https://semver.org/lang/es/):
- **MAJOR:** Cambios incompatibles con versiones anteriores
- **MINOR:** Nuevas funcionalidades compatibles
- **PATCH:** Correcciones de bugs compatibles

---

## Tipos de Cambios

- **Agregado:** Nuevas funcionalidades
- **Cambiado:** Cambios en funcionalidades existentes
- **Obsoleto:** Funcionalidades que se eliminarán próximamente
- **Eliminado:** Funcionalidades eliminadas
- **Corregido:** Correcciones de bugs
- **Seguridad:** Vulnerabilidades corregidas

---

*Formato basado en [Keep a Changelog](https://keepachangelog.com/)*

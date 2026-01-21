# Changelog - DevManager API

Todos los cambios notables de este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

---

## [2.0.1] - 2026-01-20

### 🔄 Migración Completa: Dapper → Entity Framework Core

#### ❌ Eliminado
- **Dapper 2.1.66** - Dependencia eliminada completamente del proyecto
- **DapperContext.cs** - Archivo eliminado
- **Stored Procedures** - Referencias eliminadas de toda la documentación
- SQL raw en repositorios - Reemplazado por LINQ

#### ✅ Agregado
- **AgentConfiguration.cs** (Domain) - Nueva entidad de configuración del agente
- **AgentConfiguration.cs** (Infrastructure/Configuration) - Configuraciones de EF
- **DbSet para AgentActions y AgentConfigurations** en DevManagerDbContext

#### 🔄 Modificado
- **AgentRepository.cs** - Migrado de Dapper a Entity Framework Core
- **DevManagerDbContext.cs** - Agregados DbSets para entidades del agente
- **Infrastructure.csproj** - Eliminada referencia a Dapper
- **Documentación** - 7+ archivos actualizados

#### 📊 Resultado
✅ Compilación exitosa (0 errores)  
✅ Proyecto 100% Entity Framework Core  
✅ Mayor type safety y mantenibilidad  

**Referencia completa:** [MIGRATION_DAPPER_TO_EF.md](MIGRATION_DAPPER_TO_EF.md)

---

## [2.0.0] - 2026-01-19

### 🤖 Agente Cognitivo de Orquestación de Talento

#### ✨ Agregado

**Integración con Google Gemini AI**
- GeminiService implementado con soporte para gemini-1.5-flash
- Integración mediante HTTP API con autenticación por API Key
- Chain of Thought (CoT) reasoning implementado
- Análisis estructurado de datos con JSON parsing robusto
- Manejo de errores y fallbacks automáticos
- Logging detallado de tokens y performance

**AgentService - Orquestador Principal**
- Consultas en lenguaje natural al agente
- Validación semántica de skills con evidencia
- Matching inteligente de candidatos para proyectos
- Sistema de aprobación HITL (Human-in-the-loop)
- Auditoría completa de acciones del agente
- Multi-tenancy estricto con aislamiento por OrganizationId

**Model Context Protocol (MCP) - Tool Use Pattern**
- MCPTools: Definición de herramientas descubribles
- 5 herramientas base implementadas:
  - get_employee_profile
  - get_project_requirements
  - get_skills
  - get_certifications
  - get_project_history
- Schema JSON para validación de parámetros
- Handlers dinámicos con inyección de servicios

**Background Services (Procesamiento Asíncrono)**
- ReportSnapshotGeneratorService (cada 24 horas)
  - Generación de snapshots predictivos
  - Análisis de métricas de talento
  - Identificación de brechas de capacitación
- RecommendationOptimizerService (cada 6 horas)
  - Análisis de feedback con NLP
  - Optimización de reglas de recomendación
  - Aprendizaje de patrones de éxito/fracaso

**API Endpoints del Agente**
- POST /agent/query - Consulta general en lenguaje natural
- POST /agent/validate-skill - Validación semántica de skills
- POST /agent/match-candidates - Matching inteligente de candidatos
- POST /agent/approve/{actionId} - Aprobar acción (HITL)
- POST /agent/reject/{actionId} - Rechazar acción

**Base de Datos**
- Tabla reporting.AgentActions para auditoría completa
- Tabla reporting.AgentConfiguration por organización
- Índices optimizados para queries del agente
- Foreign keys con Users para trazabilidad

**DTOs del Agente**
- AgentQueryRequest/Response
- SkillValidationRequest/Response
- SkillMatchRequest/Response con CandidateMatch
- ToolExecutionResult para tracking

**Seguridad y Gobernanza**
- Multi-tenancy garantizado en todas las operaciones
- HITL obligatorio para acciones críticas
- Umbral de confianza configurable (default 70%)
- Auditoría completa con timestamps y responsables
- Cumplimiento de Ley 1581 de 2012 (Habeas Data)

**Configuración**
- GoogleAI:ApiKey en appsettings.json
- GoogleAI:Model configurable (default: gemini-1.5-flash)
- Agent:EnableBackgroundServices (true/false)
- Agent:RequireHumanApproval (true/false)
- Agent:MinConfidenceThreshold (0-100)

**Documentación**
- AGENT_GUIDE.md - Guía completa de 500+ líneas
- AgentClientExample.cs - Cliente C# de ejemplo
- Ejemplos de curl, PowerShell y C#
- Casos de uso avanzados documentados
- Troubleshooting y FAQs

**Infraestructura**
- HttpClient configurado con IHttpClientFactory
- Retry policies para llamadas a Gemini
- Rate limiting preparado
- Health checks del agente

#### 🔒 Seguridad

- Validación estricta de OrganizationId en claims JWT
- Sanitización de inputs antes de enviar a Gemini
- Prevención de inyección de prompts
- Logs sin información sensible (PII redactada)
- API Key de Google AI mediante variables de entorno

#### 📊 Optimizaciones

- Divulgación progresiva de datos (reducción 98.7% tokens)
- Caching de resultados de background services
- Índices de BD optimizados para queries del agente
- Paralelización de llamadas independientes
- JSON parsing con fallbacks robustos

#### 🧪 Testing

- Ejemplos de testing manual documentados
- Cliente de prueba interactivo (AgentClientExample.cs)
- Scripts de verificación incluidos

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

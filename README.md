# DevManager API - Sistema de Gestión de Talento y Proyectos

API REST multi-tenant para gestión de talento, habilidades y proyectos construida con .NET 8.0 y Clean Architecture.

## 🏗️ Arquitectura

```
DevManager/
├── API/                    # Presentación (Controllers, Middleware)
├── Application/            # Lógica de aplicación (Services, DTOs, Interfaces)
├── Domain/                 # Núcleo del negocio (Entities, Enums)
└── Infrastructure/         # Acceso a datos (Repositories, Dapper)
```

**Principios Clean Architecture:**
- Domain: Sin dependencias externas
- Application: Solo depende de Domain
- API/Infrastructure: Dependen de Application + Domain

## 📋 Prerequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server 2019+ (LocalDB, Express o Standard)
- Visual Studio 2022 / VS Code / Rider

## ⚙️ Configuración

### 1. Base de Datos

```bash
# Crear base de datos
sqlcmd -S localhost -E -Q "CREATE DATABASE DevManager;"

# Ejecutar DDL (crear tablas)
sqlcmd -S localhost -d DevManager -E -i Infrastructure/Database/DDL/DDL_Dev_Manager.sql

# Ejecutar Stored Procedures
cd Infrastructure/Database/StoredProcedures
sqlcmd -S localhost -d DevManager -E -i 01_IAM_Procedures.sql
```

### 2. Configurar Connection String

Edita `API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DevManager;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Ejecutar la API

```bash
dotnet build
dotnet run --project API/API.csproj
```

La API estará disponible en:
- **HTTPS**: https://localhost:7265
- **HTTP**: http://localhost:5073
- **Swagger UI**: https://localhost:7265/swagger

## 🔐 Autenticación

La API usa **JWT Bearer** con los siguientes claims:

```csharp
- NameIdentifier: User.Id
- Email: User.Email
- Name: User.FirstName + User.LastName
- "OrganizationId": User.OrganizationId (CRÍTICO para multi-tenancy)
- Jti: Token ID único
```

### Flujo de autenticación:

1. **Registrar organización** (sin auth):
```http
POST /auth/register
Content-Type: application/json

{
  "organizationName": "Mi Empresa",
  "firstName": "Admin",
  "lastName": "User",
  "email": "admin@miempresa.com",
  "password": "Password123!"
}
```

2. **Login** (sin auth):
```http
POST /auth/login
Content-Type: application/json

{
  "email": "admin@miempresa.com",
  "password": "Password123!"
}
```

Respuesta:
```json
{
  "success": true,
  "message": "Login exitoso",
  "data": {
    "token": "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "guid",
      "firstName": "Admin",
      "email": "admin@miempresa.com"
    }
  },
  "timestamp": "2026-01-06T..."
}
```

3. **Usar el token** en endpoints protegidos:
```http
GET /users
Authorization: Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9...
```

## 🚀 Endpoints Disponibles

### Auth (Público)
- `POST /auth/login` - Iniciar sesión
- `POST /auth/register` - Registrar organización + admin

### Users (Requiere autenticación)
- `GET /users` - Listar usuarios de la organización
- `GET /users/{id}` - Obtener usuario por ID
- `POST /users` - Crear usuario
- `PUT /users/{id}` - Actualizar usuario
- `DELETE /users/{id}` - Eliminar usuario (soft delete)

## 📦 Tecnologías

- **Framework**: .NET 8.0 (ASP.NET Core Web API)
- **ORM**: Dapper 2.1.66 (micro ORM)
- **Database**: SQL Server 2019+ (Microsoft.Data.SqlClient 6.1.3)
- **Auth**: JWT Bearer 8.0.11
- **API Docs**: Swagger/OpenAPI (Swashbuckle 6.6.2)
- **Security**: HMACSHA512 (password hashing)

## 🏛️ Arquitectura de Datos

### Multi-tenancy
- **Todas** las tablas tienen `OrganizationId`
- **Todos** los queries filtran por `OrganizationId`
- El token JWT incluye `OrganizationId` para aislamiento

### Auditoría
Todas las tablas transaccionales tienen:
```sql
CreatedAt datetime2(3)
UpdatedAt datetime2(3) NULL
CreatedByUserId uniqueidentifier NULL
UpdatedByUserId uniqueidentifier NULL
IsDeleted bit DEFAULT 0
DeletedAt datetime2(3) NULL
DeletedByUserId uniqueidentifier NULL
```

### Soft Delete
- **NO** se eliminan registros físicamente
- Usar `IsDeleted = 1` + `DeletedAt` + `DeletedByUserId`
- Índices filtrados con `WHERE IsDeleted = 0`

## 🔧 Manejo de Errores

La API usa middleware global para manejo estandarizado de errores:

### Respuestas exitosas:
```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": { /* resultado */ },
  "timestamp": "2026-01-06T..."
}
```

### Respuestas de error:
```json
{
  "success": false,
  "message": "Usuario no encontrado",
  "errorCode": "NOT_FOUND",
  "errors": {
    "Id": ["El usuario con ID 'xxx' no existe"]
  },
  "traceId": "0HMVQE...",
  "timestamp": "2026-01-06T..."
}
```

### Códigos HTTP:
- `200 OK` - Éxito
- `201 Created` - Recurso creado
- `400 Bad Request` - Validación fallida
- `401 Unauthorized` - No autenticado
- `403 Forbidden` - Sin permisos
- `404 Not Found` - Recurso no existe
- `409 Conflict` - Conflicto (ej: email duplicado)
- `500 Internal Server Error` - Error del servidor

## 🗄️ Esquema de Base de Datos

### Módulos:

**IAM** (`iam.*`)
- Organizations, Users, Roles, UserRoles

**Talent** (`talent.*`)
- EmployeeProfiles, Skills, EmployeeSkills, Certifications, SkillEvaluations

**Projects** (`projects.*`)
- Projects, ProjectSkillRequirements, ProjectRoles
- ProjectApplications, ProjectAssignments, ProjectParticipation

**Reporting** (`reporting.*`)
- ReportSnapshots, RecommendationRules, RecommendationLogs

Ver DDL completo en: [Infrastructure/Database/DDL/DDL_Dev_Manager.sql](Infrastructure/Database/DDL/DDL_Dev_Manager.sql)

## 📝 Convenciones de Código

### Namespaces:
```csharp
API.Controllers
API.Middleware
API.Extensions
Application.Services
Application.DTOs
Application.Interfaces
Application.Common.Exceptions
Domain
Domain.Entities
Domain.Enums
Domain.Interfaces.Repositories
Infrastructure.Data
Infrastructure.Repositories
Infrastructure.Services
```

### Stored Procedures:
```sql
sp_[Schema]_[Action]

Ejemplos:
- sp_Iam_GetUserByEmail
- sp_Iam_CreateUser
- sp_Talent_GetEmployeeSkills
- sp_Projects_CreateProject
```

### Controllers:
- Heredan de `ControllerBase`
- Rutas: `[Route("[controller]")]`
- Endpoints con `[Authorize]` (excepto Auth)
- Helpers: `GetOrganizationId()`, `GetCurrentUserId()`

## 🧪 Testing

```bash
# Ejecutar prueba manual con Swagger
dotnet run --project API/API.csproj
# Navegar a https://localhost:7265/swagger

# O con curl:
curl -X POST https://localhost:7265/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "organizationName": "Test Org",
    "firstName": "Test",
    "lastName": "User",
    "email": "test@test.com",
    "password": "Test123!"
  }'
```

## 📂 Estructura de Archivos

```
DevManagerAPI/
├── API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── UsersController.cs
│   ├── Middleware/
│   │   └── GlobalExceptionHandlerMiddleware.cs
│   ├── Extensions/
│   │   └── ApplicationServiceExtensions.cs
│   ├── Program.cs
│   └── appsettings.json
├── Application/
│   ├── Services/
│   │   ├── AuthService.cs
│   │   └── UserService.cs
│   ├── DTOs/
│   │   ├── Auth/
│   │   └── Users/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IUserService.cs
│   │   └── ITokenService.cs
│   └── Common/
│       ├── Exceptions/
│       └── Models/
├── Domain/
│   ├── Entities/
│   │   ├── IAM/
│   │   ├── Talent/
│   │   └── Projects/
│   ├── Enums/
│   │   └── *.cs
│   └── Interfaces/
│       └── Repositories/
├── Infrastructure/
│   ├── Data/
│   │   └── DapperContext.cs
│   ├── Repositories/
│   │   ├── AuthRepository.cs
│   │   └── UserRepository.cs
│   ├── Services/
│   │   └── TokenService.cs
│   └── Database/
│       ├── DDL/
│       │   └── DDL_Dev_Manager.sql
│       └── StoredProcedures/
│           └── 01_IAM_Procedures.sql
└── DevManager.sln
```

## ⚠️ Pendientes

- [ ] Implementar módulos Talent (Profiles, Skills)
- [ ] Implementar módulo Projects
- [ ] Agregar validaciones con FluentValidation
- [ ] Tests unitarios (xUnit)
- [ ] Tests de integración
- [ ] Rate limiting
- [ ] Health checks
- [ ] Logging estructurado (Serilog)
- [ ] Cache (Redis)

## 🤝 Contribución

Este proyecto sigue Clean Architecture y SOLID principles. Al agregar nuevas funcionalidades:

1. Crear entidad en `Domain/` (si aplica)
2. Crear DTOs en `Application/DTOs/`
3. Crear interface de repositorio en `Domain/Interfaces/Repositories/`
4. Implementar repositorio en `Infrastructure/Repositories/`
5. Crear interface de servicio en `Application/Interfaces/`
6. Implementar servicio en `Application/Services/`
7. Crear controller en `API/Controllers/`
8. Registrar dependencias en `ApplicationServiceExtensions.cs`
9. Crear stored procedures en `Infrastructure/Database/StoredProcedures/`

## 📄 Licencia

Este proyecto es privado y de uso interno.

---

**Desarrollado con ❤️ usando Clean Architecture + .NET 8.0**

# Capítulo 4: Especificación de la API y Lógica de Negocio

## 4.1 Estructura de la API REST: Controladores y Endpoints

DevManagerAPI expone sus funcionalidades mediante una API REST diseñada según los principios de separación de responsabilidades de la arquitectura limpia. La capa de presentación (`API/`) agrupa trece controladores, cada uno responsable de un dominio específico del sistema. La siguiente tabla describe el inventario completo de controladores, sus rutas base, los métodos HTTP disponibles y la cobertura de requerimientos funcionales (RF) que cada uno satisface.

| # | Controlador | Ruta Base | Métodos HTTP | RF Cubiertos |
|---|-------------|-----------|--------------|--------------|
| 1 | `AuthController` | `/api/auth` | POST `/login`, POST `/register-organization` | RF-001, RF-003 |
| 2 | `UsersController` | `/api/users` | GET `/`, GET `/{id}`, POST `/`, PUT `/{id}`, DELETE `/{id}` | RF-002, RF-004 |
| 3 | `ProjectsController` | `/api/projects` | GET `/`, GET `/{id}`, POST `/`, PUT `/{id}`, POST `/{id}/reqs`, GET `/{id}/reqs` | RF-010, RF-011, RF-012, RF-014 |
| 4 | `ProjectApplicationsController` | `/api` | POST `/projects/{id}/apply`, GET `/projects/{id}/applications`, PUT `/applications/{id}/review` | RF-015, RF-016 |
| 5 | `AssignmentsController` | `/api/assignments` | POST `/` | RF-017 |
| 6 | `AgentController` | `/agent` | POST `/query`, POST `/validate-skill`, POST `/match-candidates`, POST `/approve/{actionId}`, POST `/reject/{actionId}` | RF-009, RF-023, RF-024 |
| 7 | `SkillsController` | `/api/skills` | GET `/`, POST `/`, PUT `/{id}`, DELETE `/{id}` | RF-006 |
| 8 | `EmployeeSkillsController` | `/api/employees` | GET `/{id}/skills`, POST `/skills`, PUT `/skills/{id}/validate`, DELETE `/skills/{id}` | RF-008, RF-009 |
| 9 | `ProfileController` | `/api/profile` | GET `/me`, POST `/me`, PUT `/me`, DELETE `/me` | RF-005 |
| 10 | `CertificationsController` | `/api/certifications` | GET `/me`, GET `/me/{id}`, POST `/me`, PUT `/me/{id}`, DELETE `/me/{id}` | RF-007 |
| 11 | `RolesController` | `/api/roles` | GET `/`, GET `/{id}`, POST `/`, PUT `/{id}`, DELETE `/{id}`, GET `/{id}/permissions`, PUT `/{id}/permissions`, DELETE `/{id}/permissions/{permissionId}`, GET `/user-assignments`, POST `/assign-to-user`, POST `/revoke-from-user` | RF-002 |
| 12 | `PermissionsController` | `/api/permissions` | GET `/`, GET `/grouped`, GET `/{id}`, POST `/`, PUT `/{id}`, DELETE `/{id}`, POST `/assign-to-user`, DELETE `/revoke-from-user/{userId}/{permissionId}`, GET `/user/{userId}/effective`, POST `/validate` | RF-002 |
| 13 | `ConfigController` | `/api/config` | GET `/`, GET `/project-statuses`, GET `/complexity-levels`, GET `/application-statuses`, GET `/assignment-statuses`, GET `/skill-levels`, GET `/contribution-scores`, GET `/evaluation-sources`, GET `/skill-types`, GET `/skill-categories`, GET `/agent-action-types`, GET `/agent-action-statuses`, GET `/seniority-levels` | RF-013, RF-020 |

Todos los controladores, a excepción de `AuthController`, requieren autenticación mediante el atributo `[Authorize]`. La extracción del `OrganizationId` y del `UserId` se realiza directamente a partir de los claims del `ClaimsPrincipal`, garantizando el aislamiento de datos entre organizaciones (multi-tenancy) sin necesidad de parámetros explícitos en las rutas.

---

## 4.2 Matriz de Trazabilidad de Requerimientos Funcionales

La siguiente matriz establece la correspondencia directa entre cada uno de los veinticuatro requerimientos funcionales del sistema y los artefactos técnicos que los implementan: controlador, endpoint, tabla o tablas de base de datos involucradas, y mecanismo de verificación disponible.

| RF-ID | Nombre del Requerimiento | Controlador | Endpoint(s) | Tabla(s) BD | Verificación |
|-------|--------------------------|-------------|-------------|-------------|--------------|
| RF-001 | Registro y administración de organizaciones (multi-tenancy) | `AuthController` | `POST /api/auth/register-organization` | `iam.Organizations`, `iam.Users`, `iam.UserRoles` | Restricción `UQ_Organizations_Nit`; transacción atómica crea organización + usuario administrador + 3 roles por defecto |
| RF-002 | Gestión de usuarios y roles (RBAC) | `UsersController`, `RolesController`, `PermissionsController` | `GET/POST/PUT/DELETE /api/users`, `GET/POST/PUT/DELETE /api/roles`, `GET/POST/PUT/DELETE /api/permissions` | `iam.Users`, `iam.Roles`, `iam.Permissions`, `iam.UserRoles`, `iam.RolePermissions`, `iam.UserPermissions` | CRUD completo con validación de unicidad; roles globales tienen `OrganizationId = NULL` |
| RF-003 | Autenticación segura con hash de contraseñas | `AuthController` | `POST /api/auth/login` | `iam.Users` | `PasswordHash` (varbinary 512) + `PasswordSalt` (varbinary 256) con HMACSHA512; token JWT emitido con validez de 8 horas |
| RF-004 | Activación/desactivación de cuentas (soft delete) | `UsersController` | `DELETE /api/users/{id}` | `iam.Users` | `IsDeleted = true`, `DeletedAt = SYSUTCDATETIME()`; el registro persiste para auditoría; el consumidor de la API recibe `204 No Content` |
| RF-005 | Gestión de perfil profesional y bio | `ProfileController` | `GET /api/profile/me`, `POST /api/profile/me`, `PUT /api/profile/me` | `talent.EmployeeProfiles` | Campos `Bio`, `YearsExperience`, `LinkedInUrl`, `PortfolioUrl`; upsert automático en `PUT` |
| RF-006 | Administración del catálogo de habilidades | `SkillsController` | `GET /api/skills`, `POST /api/skills`, `PUT /api/skills/{id}`, `DELETE /api/skills/{id}` | `talent.Skills` | Campo `SkillType` diferencia skills globales (`OrganizationId = NULL`) de organizacionales; unicidad por nombre dentro de la organización |
| RF-007 | Registro de certificaciones con evidencia | `CertificationsController` | `GET /api/certifications/me`, `POST /api/certifications/me`, `PUT /api/certifications/me/{id}`, `DELETE /api/certifications/me/{id}` | `talent.Certifications` | Campos `EvidenceUrl`, `IssueDate`, `ExpirationDate`; acceso restringido al propietario del registro |
| RF-008 | Autogestión de niveles de habilidad (1–5) | `EmployeeSkillsController` | `POST /api/employees/skills`, `PUT /api/employees/skills/{id}/validate` (nivel) | `talent.EmployeeSkills` | Campo `Level` de tipo `byte` con rango 1–5; operación de upsert: crea si no existe, actualiza si ya existe |
| RF-009 | Validación de competencias por terceros | `EmployeeSkillsController`, `AgentController` | `PUT /api/employees/skills/{id}/validate`, `POST /agent/validate-skill` | `talent.EmployeeSkills` | `ValidatedByUserId` + `LastValidatedAt` registran quién y cuándo validó; `AgentController` proporciona validación semántica adicional mediante IA |
| RF-010 | Gestión del ciclo de vida del proyecto | `ProjectsController` | `POST /api/projects`, `PUT /api/projects/{id}` | `projects.Projects`, `config.ProjectStatuses` | `ProjectStatus` enum: Draft(0), Active(1), OnHold(2), Completed(3), Cancelled(4); campo `AllowsApplications` en catálogo determina postulaciones habilitadas |
| RF-011 | Definición de requerimientos de habilidades | `ProjectsController` | `POST /api/projects/{id}/reqs`, `GET /api/projects/{id}/reqs` | `projects.ProjectSkillRequirements` | Campos `RequiredLevel` (1–5), `IsMandatory` (bit); unicidad por `SkillId + ProjectId` |
| RF-012 | Configuración de roles y vacantes | `ProjectsController` | `POST /api/projects`, `PUT /api/projects/{id}` | `projects.ProjectRoles`, `projects.ProjectAssignments` | `ProjectRoles.NeededCount >= 1`; los roles de proyecto se vinculan con las asignaciones efectivas |
| RF-013 | Cálculo de impacto por complejidad | `AgentController`, `ConfigController` | `POST /agent/match-candidates`, `GET /api/config/complexity-levels` | `config.ProjectComplexityLevels`, `talent.SkillEvaluations` | `ExperienceMultiplier` (0.5–3.0) en catálogo; el `AgentService` utiliza el multiplicador para calcular `DeltaLevel` en evaluaciones de skills |
| RF-014 | Visualización de oportunidades según perfil | `ProjectsController`, `AgentController` | `GET /api/projects?status=1`, `POST /agent/match-candidates` | `projects.Projects`, `projects.ProjectSkillRequirements`, `talent.EmployeeSkills` | Filtro por estado `status=1` (Active); `match-candidates` calcula score de compatibilidad 0–100 por candidato |
| RF-015 | Proceso de postulación con mensaje de motivación | `ProjectApplicationsController` | `POST /api/projects/{id}/apply` | `projects.ProjectApplications` | Campo `Message` (motivación); estado inicial `Pending(0)`; `UserId` y `AppliedAt` establecidos automáticamente desde JWT |
| RF-016 | Gestión de revisiones y retroalimentación | `ProjectApplicationsController` | `PUT /api/applications/{id}/review`, `GET /api/projects/{id}/applications` | `projects.ProjectApplications` | `Status`: Pending(0), Approved(1), Rejected(2); `ReviewNotes` requerido en rechazo; `ReviewedByUserId` + `ReviewedAt` registrados automáticamente |
| RF-017 | Asignación directa de personal | `AssignmentsController` | `POST /api/assignments` | `projects.ProjectAssignments`, `projects.ProjectRoles` | Campos `Role`, `HoursPerWeek`, `StartDate`, `EndDate`; asignación administrativa independiente de postulaciones previas |
| RF-018 | Registro de contribución y feedback cualitativo | No expuesto como endpoint independiente en v1 | (via AssignmentsController al cierre) | `projects.ProjectParticipations` | Campos `ContributionScore` (1–5) y `FeedbackComments` (nvarchar max); registrado automáticamente al completar el ciclo del proyecto |
| RF-019 | Actualización de niveles de habilidad (DeltaLevel) | `AgentController` (orquestado internamente) | `POST /agent/query` (triggered on project close) | `talent.SkillEvaluations` | Campo `DeltaLevel` (smallint −5 a +5); `AgentService` inserta evaluaciones automáticamente al detectar cierre de proyecto |
| RF-020 | Categorización del origen de evaluación | `ConfigController` | `GET /api/config/evaluation-sources` | `config.EvaluationSources`, `talent.SkillEvaluations` | `SkillEvaluationSource` enum: Project(1), Manual(2), SystemRule(3), Certification(4), SelfAssessment(5); campo `Source` en `SkillEvaluations` |
| RF-021 | Generación de instantáneas de datos (snapshots) | Servicio en segundo plano | (no expuesto como endpoint REST) | `reporting.ReportSnapshots` | `ReportSnapshotGeneratorService` (IHostedService) genera snapshots diariamente; índice único `UX_ReportSnapshots_Org_Date` impide duplicados por organización y fecha |
| RF-022 | Motor de reglas de recomendación | Servicio en segundo plano | (no expuesto como endpoint REST) | `reporting.RecommendationRules`, `reporting.RecommendationLogs` | `RecommendationOptimizerService` (IHostedService) evalúa y actualiza reglas cada 6 horas; `AgentService` aplica reglas activas durante la construcción del prompt del sistema |
| RF-023 | Consulta del agente para skill gaps y matching | `AgentController` | `POST /agent/query`, `POST /agent/match-candidates`, `POST /agent/validate-skill` | `talent.EmployeeSkills`, `projects.ProjectSkillRequirements`, `talent.EmployeeProfiles` | Algoritmo de matching: 60% skills obligatorias + 20% skills opcionales + 10% experiencia + 10% excedente de nivel (score 0–100) |
| RF-024 | Auditoría de recomendaciones del agente (HITL) | `AgentController` | `POST /agent/approve/{actionId}`, `POST /agent/reject/{actionId}` | `reporting.AgentActions`, `reporting.RecommendationLogs` | Flujo HITL: estado `PENDING_APPROVAL → APPROVED/REJECTED`; `ApprovedByUserId`, `ApprovedAt`, `RejectedByUserId`, `RejectedAt` y `Reason` registrados para auditoría completa |

---

## 4.3 Seguridad: Autenticación y Autorización mediante JWT

### 4.3.1 Estructura del Token JWT

El sistema implementa autenticación sin estado (stateless) mediante tokens JWT (JSON Web Tokens) firmados con el algoritmo HMACSHA512. Cada token encapsula la identidad y el contexto organizacional del solicitante autenticado en un conjunto de claims verificados por el middleware de autenticación en cada solicitud entrante.

**Claims incluidos en el token:**

| Claim | Tipo en JWT | Descripción | Criticidad |
|-------|-------------|-------------|------------|
| `nameid` | `ClaimTypes.NameIdentifier` | Identificador único del usuario (GUID) | Alta — filtra datos por usuario |
| `email` | `ClaimTypes.Email` | Dirección de correo electrónico | Media — informativa |
| `name` | `ClaimTypes.Name` | Nombre completo del usuario | Media — informativa |
| `OrganizationId` | Claim personalizado | GUID de la organización (multi-tenancy) | Crítica — filtra TODOS los datos |
| `jti` | JWT ID | Identificador único del token (GUID) | Alta — trazabilidad y auditoría |

### 4.3.2 Configuración de Validación

El middleware de autenticación configura los siguientes parámetros de validación del token:

```
Algoritmo de firma:   HMACSHA512
Expiración del token: 8 horas desde la emisión
ClockSkew:            Zero (TimeSpan.Zero) — sin margen de tolerancia
Validaciones activas: Issuer, Audience, Lifetime, IssuerSigningKey
```

La configuración de `ClockSkew = Zero` elimina el margen de tolerancia predeterminado de cinco minutos que incorpora la librería `Microsoft.AspNetCore.Authentication.JwtBearer`, garantizando que los tokens expiren en el tiempo exacto configurado sin ventanas de gracia que pudieran representar un riesgo de seguridad.

### 4.3.3 Aplicación del Multi-tenancy mediante JWT

Cada controlador autorizado extrae el claim `OrganizationId` del `ClaimsPrincipal` antes de invocar los servicios de la capa de aplicación. Este identificador se propaga hacia los repositorios, donde se aplica como predicado de filtrado en todas las consultas (`WHERE OrganizationId = @orgId AND IsDeleted = 0`). Ningún dato de una organización es accesible desde el contexto de autenticación de otra organización.

```
Solicitud HTTP con Bearer token
         │
         ▼
[Middleware JWT] ── Valida firma y expiración ──→ Extrae Claims
         │                                          ├─ nameid     (UserId)
         │                                          ├─ OrganizationId ◄── CRÍTICO
         │                                          ├─ email
         │                                          └─ jti
         ▼
[Controlador] ── User.FindFirst("OrganizationId") ──→ Guid organizationId
         │
         ▼
[Servicio / Repositorio] ── WHERE OrganizationId = @orgId AND IsDeleted = 0
         │
         ▼
[SQL Server] ── Consulta aislada por organización
```

---

## 4.4 Motor de Inteligencia Artificial del Sistema

El motor de inteligencia artificial de DevManagerAPI integra Google Gemini (modelo `gemini-1.5-flash`) mediante una arquitectura de servicios que implementa los patrones Chain-of-Thought (CoT) y Human in the Loop (HITL). Esta integración se materializa en tres componentes principales: `AgentService`, `GeminiService` y `AgentRepository`.

### 4.4.1 Servicio de Agente (AgentService) — Razonamiento Chain-of-Thought

`AgentService` actúa como orquestador principal del flujo de razonamiento. Ante una consulta recibida por `AgentController`, el servicio ejecuta una cadena de pasos secuenciales que recopilan contexto real de la base de datos, construyen el prompt del sistema y delegan el razonamiento final al modelo de lenguaje.

**Etapas del pipeline CoT:**

**Etapa 1 — `GatherContextDataAsync()`**

El servicio determina, a partir del texto de la consulta, qué datos organizacionales son relevantes para responderla. Invoca de forma selectiva los servicios de dominio disponibles:

- `IProfileService` — perfil profesional y años de experiencia del solicitante
- `ISkillService` — catálogo de habilidades de la organización
- `IEmployeeSkillService` — habilidades declaradas y validadas de todos los empleados
- `IProjectService` — proyectos activos y sus requerimientos de skills
- `IUserService` — directorio de usuarios de la organización

El resultado es un diccionario tipado `Dictionary<string, object>` que contiene únicamente los datos pertinentes para la consulta específica.

**Etapa 2 — `BuildSystemPrompt()`**

Construye las instrucciones de comportamiento del agente, incluyendo:

- Definición del rol del agente (orquestador de talento)
- Reglas de recomendación activas recuperadas de `reporting.RecommendationRules`
- Instrucciones de formato de salida (Markdown estructurado)
- Restricción del alcance al `OrganizationId` extraído del JWT

**Etapa 3 — `BuildDataContext()`**

Serializa el diccionario de contexto recopilado en la Etapa 1 hacia una representación textual estructurada (JSON compacto), que se inyecta en el prompt final como sección de datos reales disponibles. Esta serialización garantiza que el modelo de lenguaje opere sobre información real de la organización en lugar de datos generados.

**Etapa 4 — Delegación a `GeminiService`**

El prompt completo (instrucciones del sistema + datos + consulta del usuario) se envía a `GeminiService.QueryWithReasoningAsync()`, que retorna una tupla `(Response, Reasoning)` tras ejecutar el análisis Chain-of-Thought.

**Etapa 5 — `AgentRepository.CreateActionAsync()`**

La respuesta y el razonamiento del modelo se persisten en `reporting.AgentActions` con estado inicial `PENDING_APPROVAL` si el parámetro `requireApproval` es `true`, o con estado `SUCCESS` para consultas de solo lectura. El método retorna el `ActionId` generado, que el consumidor de la API puede utilizar para el flujo HITL.

### 4.4.2 Integración con Google Gemini API (GeminiService)

`GeminiService`, ubicado en la capa de infraestructura (`Infrastructure/Services/AI/`), encapsula toda la lógica de comunicación con la API de Google Gemini.

**Configuración del cliente HTTP:**

```
Modelo:    gemini-1.5-flash (configurable en appsettings)
Endpoint:  https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent
Método:    HTTP POST
Auth:      API Key como parámetro de consulta (?key={apiKey})
```

**Parámetros de generación:**

```
Temperature:      0.7
MaxOutputTokens:  8192
TopP:             0.95
```

**Mecanismo Chain-of-Thought en `QueryWithReasoningAsync()`:**

El método construye un prompt enriquecido que instruye al modelo a razonar de forma explícita antes de generar la respuesta final. El formato de respuesta esperado es JSON estricto:

```json
{
    "reasoning": "Paso a paso del razonamiento del modelo...",
    "response":  "Respuesta final estructurada en Markdown"
}
```

El método aplica limpieza defensiva sobre la respuesta (eliminación de bloques de código Markdown, extracción del fragmento JSON por delimitadores `{` y `}`) antes del proceso de deserialización. En caso de que el parseo falle, se aplica un mecanismo de respaldo (*fallback*) que divide el texto en dos mitades asignando la primera al razonamiento y la segunda a la respuesta.

**Diagrama de secuencia — Pipeline CoT completo:**

```
Consumidor API
      │
      │ POST /agent/query  { "query": "...", "requireApproval": true }
      ▼
AgentController.QueryAsync()
      │
      │ organizationId ← JWT claim
      │ userId         ← JWT claim
      ▼
AgentService.QueryAsync()
      │
      ├─[1] GatherContextDataAsync()
      │         │
      │         ├─→ IProfileService.GetMyProfileAsync()       ──→ talent.EmployeeProfiles
      │         ├─→ ISkillService.GetAllSkillsAsync()         ──→ talent.Skills
      │         ├─→ IEmployeeSkillService.GetSkillsAsync()    ──→ talent.EmployeeSkills
      │         └─→ IProjectService.GetAllProjectsAsync()     ──→ projects.Projects
      │
      ├─[2] BuildSystemPrompt()
      │         │
      │         └─→ Instrucciones del agente + OrganizationId + RecommendationRules
      │
      ├─[3] BuildDataContext()
      │         │
      │         └─→ JSON serialization del diccionario de contexto
      │
      ├─[4] GeminiService.QueryWithReasoningAsync(fullPrompt)
      │         │
      │         │ HTTP POST → generativelanguage.googleapis.com
      │         │             /v1beta/models/gemini-1.5-flash:generateContent
      │         │
      │         │ ← { "reasoning": "...", "response": "..." }
      │         │
      │         └─→ ParseCoTResponse() → (Response, Reasoning)
      │
      └─[5] AgentRepository.CreateActionAsync()
                │
                └─→ INSERT reporting.AgentActions
                         Status = PENDING_APPROVAL | SUCCESS
                         ActionId (GUID) generado

      ← ApiResponse { ActionId, markdown, reasoning, metadata }
```

### 4.4.3 Patrón HITL (Human in the Loop)

El patrón HITL garantiza que las recomendaciones del agente que implican acciones sobre datos (asignaciones de personal, cambios de nivel de habilidad) sean revisadas y aprobadas por un responsable humano antes de considerarse ejecutadas. Este mecanismo se implementa mediante el flujo de estados de la entidad `AgentAction`.

**Ciclo de vida de una `AgentAction`:**

```
Consulta con requireApproval = true
          │
          ▼
   [PENDING_APPROVAL]
   AgentAction creada en reporting.AgentActions
          │
          ├──────────────────────────────────┐
          │                                  │
          ▼                                  ▼
POST /agent/approve/{actionId}    POST /agent/reject/{actionId}
          │                                  │
          ▼                                  ▼
      [APPROVED]                         [REJECTED]
  ApprovedByUserId ← JWT             RejectedByUserId ← JWT
  ApprovedAt ← SYSUTCDATETIME()      RejectedAt ← SYSUTCDATETIME()
                                      Reason ← body del request
```

**Descripción de los estados:**

| Estado | Descripción | Campos Adicionales |
|--------|-------------|-------------------|
| `PENDING_APPROVAL` | Acción generada, pendiente de revisión humana | `ActionId`, `ActionType`, `Payload` (JSON con recomendación) |
| `APPROVED` | Acción revisada y confirmada por un responsable | `ApprovedByUserId`, `ApprovedAt` |
| `REJECTED` | Acción descartada con motivo documentado | `RejectedByUserId`, `RejectedAt`, `Reason` |

La implementación actual registra la decisión con fines de auditoría. La ejecución automática de la acción post-aprobación (por ejemplo, crear una asignación directamente desde la aprobación) constituye una funcionalidad planificada en el trabajo futuro del sistema (véase §4.6).

**Endpoints HITL en `AgentController`:**

```
POST /agent/approve/{actionId:guid}
    Cuerpo: vacío
    Acción: Estado PENDING_APPROVAL → APPROVED; registra ApprovedByUserId y ApprovedAt

POST /agent/reject/{actionId:guid}
    Cuerpo: { "reason": "Motivo del rechazo" }
    Acción: Estado PENDING_APPROVAL → REJECTED; registra RejectedByUserId, RejectedAt, Reason
```

### 4.4.4 Algoritmo de Emparejamiento de Candidatos

El algoritmo de matching, implementado en `AgentService.MatchCandidatesForProjectAsync()`, calcula un índice de compatibilidad normalizado entre 0 y 100 para cada empleado de la organización con respecto a un proyecto dado. El índice pondera cuatro dimensiones:

**Fórmula de puntuación:**

```
Score = (MandatorySkillsScore × 0.60)
      + (OptionalSkillsScore  × 0.20)
      + (ExperienceScore      × 0.10)
      + (LevelSurplusScore    × 0.10)
```

**Dimensión 1 — Skills Obligatorias (60 %)**

Se evalúa si el empleado posee *todas* las habilidades marcadas como `IsMandatory = true` en `projects.ProjectSkillRequirements`, con un nivel igual o superior al `RequiredLevel` especificado. Si el empleado carece de al menos una skill obligatoria, el score máximo alcanzable es 40 % (las tres dimensiones restantes), lo que refleja que el candidato no cumple los requisitos mínimos del proyecto.

**Dimensión 2 — Skills Opcionales (20 %)**

Se evalúa la cobertura de habilidades marcadas como `IsMandatory = false`. La puntuación es proporcional al número de skills opcionales que el empleado posee con nivel suficiente respecto al total de skills opcionales definidas en el proyecto.

**Dimensión 3 — Experiencia (10 %)**

Se considera el campo `YearsExperience` del perfil del empleado (`talent.EmployeeProfiles`). La puntuación se normaliza con base en los años de experiencia del empleado en comparación con el máximo observado en la organización, favoreciendo a los candidatos con mayor trayectoria profesional.

**Dimensión 4 — Excedente de Nivel (10 %)**

Para cada skill que el empleado posee con nivel superior al requerido (`EmployeeSkill.Level > ProjectSkillRequirement.RequiredLevel`), se computa un bono proporcional a la diferencia positiva de niveles. Esta dimensión premia al candidato que supera los requisitos técnicos mínimos del proyecto.

**Ejemplo de interpretación del score:**

| Rango | Interpretación |
|-------|---------------|
| 90–100 | Candidato altamente recomendado — cumple todos los requisitos y los supera |
| 70–89 | Candidato recomendado — cumple requisitos obligatorios con posibles brechas menores |
| 50–69 | Candidato con brechas — no cumple alguna skill obligatoria o de nivel insuficiente |
| < 50 | Candidato no recomendado — déficit significativo en habilidades obligatorias |

El parámetro `minScore` del endpoint `POST /agent/match-candidates` permite al consumidor de la API filtrar únicamente los candidatos que superen un umbral mínimo (valor recomendado: 70 para proyectos de alta criticidad).

---

## 4.5 Servicios en Segundo Plano (Background Services)

DevManagerAPI registra dos servicios en segundo plano implementados mediante la interfaz `IHostedService` de .NET 8, a través de la clase base `BackgroundService`. Estos servicios ejecutan tareas periódicas de forma autónoma, desacopladas del ciclo de vida de las solicitudes HTTP.

### 4.5.1 ReportSnapshotGeneratorService

**Responsabilidad:** Generar y persistir instantáneas periódicas del estado de la organización en la tabla `reporting.ReportSnapshots`.

**Configuración de ejecución:**

```
Intervalo:      24 horas (TimeSpan.FromHours(24))
Reintentos:     En caso de error, reintenta tras 5 minutos
Inicio:         Inmediato al arrancar la aplicación
```

**Comportamiento diseñado (pendiente de implementación completa):**

1. Recuperar todas las organizaciones activas del sistema
2. Para cada organización, analizar:
   - Métricas de cobertura de skills por departamento
   - Tasa de utilización de talento (empleados asignados / empleados totales)
   - Identificación de brechas de capacitación prioritarias
   - Generación de predicciones mediante `IGeminiService`
3. Serializar el resultado como JSON y persisitir en `reporting.ReportSnapshots`

El índice único `UX_ReportSnapshots_Org_Date` (sobre `OrganizationId + SnapshotDate`) garantiza que no existan duplicados para la misma organización en el mismo día.

**Estado actual:** La lógica de generación de snapshots contiene anotaciones `TODO` que documentan los pasos diseñados pero aún no implementados. El servicio registra eventos de inicio y finalización mediante Serilog, pero el cuerpo del método `GenerateSnapshotsAsync()` ejecuta únicamente `Task.CompletedTask` sin efectos sobre la base de datos.

### 4.5.2 RecommendationOptimizerService

**Responsabilidad:** Analizar el historial de retroalimentación de proyectos y actualizar las reglas de recomendación almacenadas en `reporting.RecommendationRules`.

**Configuración de ejecución:**

```
Intervalo:         6 horas (TimeSpan.FromHours(6))
Retardo inicial:   1 hora tras el arranque (evita contención con el inicio del sistema)
Reintentos:        En caso de error, reintenta tras 10 minutos
```

**Comportamiento diseñado (pendiente de implementación completa):**

1. Recuperar retroalimentación reciente de `projects.ProjectParticipations` (campo `ContributionScore` y `FeedbackComments`)
2. Analizar patrones de éxito y fracaso mediante análisis semántico con `IGeminiService`
3. Identificar factores correlacionados con altos `ContributionScore` (skills, nivel de experiencia, tipo de proyecto)
4. Actualizar `ConditionExpr` en `reporting.RecommendationRules`
5. Registrar el resultado del análisis en `reporting.RecommendationLogs`

**Estado actual:** Análogamente al servicio anterior, el método `OptimizeRecommendationRulesAsync()` contiene anotaciones `TODO` y finaliza con `Task.CompletedTask`. El servicio es operacional en cuanto a su ciclo de vida pero sin efectos sobre los datos.

---

## 4.6 Trabajo Futuro

Durante el desarrollo de la versión 1.0 del sistema se identificaron las siguientes áreas de trabajo futuro, derivadas de simplificaciones de implementación o de funcionalidades diseñadas pero no concluidas en el alcance actual:

### 4.6.1 Implementación completa de los servicios en segundo plano

Tanto `ReportSnapshotGeneratorService` como `RecommendationOptimizerService` cuentan con la arquitectura, el registro de dependencias y el esquema de base de datos necesarios, pero sus métodos de ejecución están documentados con anotaciones `TODO` y no producen efectos sobre los datos. La implementación completa requiere desarrollar la lógica de recopilación de métricas, la integración con `IGeminiService` para el análisis predictivo, y la escritura en las tablas `reporting.ReportSnapshots`, `reporting.RecommendationRules` y `reporting.RecommendationLogs`.

### 4.6.2 Ejecución automática de acciones aprobadas en el flujo HITL

Los endpoints `POST /agent/approve/{actionId}` y `POST /agent/reject/{actionId}` registran la decisión del responsable humano en `reporting.AgentActions` con fines de auditoría, pero no ejecutan automáticamente la acción asociada (por ejemplo, crear una asignación, actualizar el nivel de una habilidad o enviar una notificación). La implementación de la ejecución post-aprobación requiere un despachador de acciones que interprete el campo `ActionType` y el `Payload` JSON de la `AgentAction` y materialice el efecto correspondiente en las entidades del dominio.

### 4.6.3 `OrganizationId` codificado de forma fija en `LoginAsync`

El método `AuthService.LoginAsync()` realiza la búsqueda del usuario por email sin discriminar por `OrganizationId`, dado que en la versión actual la asociación email–organización es unívoca por diseño del seeder de datos. En un entorno de producción con múltiples organizaciones que pudieran compartir dominios de correo, este comportamiento podría requerir que el consumidor de la API proporcione un identificador de organización en la solicitud de autenticación.

### 4.6.4 Emisión de claims de rol en el token JWT (RBAC incompleto)

El sistema cuenta con tablas `iam.Roles`, `iam.UserRoles`, `iam.Permissions`, `iam.RolePermissions` y `iam.UserPermissions` completamente implementadas y expuestas a través de `RolesController` y `PermissionsController`. Sin embargo, `TokenService` no emite el claim `role` en el token JWT. Esto implica que la autorización basada en roles (RBAC) no puede aplicarse como política declarativa `[Authorize(Roles = "...")]` en los controladores. Los permisos deben verificarse mediante llamadas explícitas al endpoint `POST /api/permissions/validate`, en lugar de resolverse automáticamente en el middleware de autorización. La emisión del claim `role` en el token requeriría consultar los roles activos del usuario durante el proceso de autenticación.

---

## 4.7 Síntesis del Capítulo

El presente capítulo ha especificado la capa de presentación y la lógica de negocio de DevManagerAPI mediante cuatro dimensiones de análisis.

En primer lugar, se documentaron los trece controladores REST del sistema con sus rutas exactas, métodos HTTP y cobertura de requerimientos funcionales, evidenciando una correspondencia directa entre la arquitectura de la API y los dominios del negocio: identidad y acceso, gestión de talento, proyectos, agente inteligente y configuración.

En segundo lugar, la matriz de trazabilidad de los veinticuatro requerimientos funcionales (RF-001 a RF-024) establece de forma auditables el vínculo entre cada requerimiento del negocio y su implementación técnica concreta en términos de controlador, endpoint y tablas de base de datos involucradas.

En tercer lugar, se describió el mecanismo de seguridad basado en JWT con HMACSHA512, subrayando el rol del claim `OrganizationId` como pivot del aislamiento multi-tenant y la configuración de `ClockSkew = Zero` como medida de refuerzo en la validación de tokens.

Finalmente, se expusieron en detalle los tres componentes del motor de inteligencia artificial: el pipeline Chain-of-Thought de `AgentService` (recopilación de contexto → construcción de prompt → generación → persistencia), la integración HTTP con Google Gemini API a través de `GeminiService`, el patrón HITL con su ciclo de estados `PENDING_APPROVAL → APPROVED/REJECTED`, y el algoritmo de matching de candidatos con su fórmula ponderada de cuatro dimensiones. La identificación de las áreas de trabajo futuro en §4.6 proporciona una hoja de ruta técnica clara para las iteraciones posteriores del sistema.

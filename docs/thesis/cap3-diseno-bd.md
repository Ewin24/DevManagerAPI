# Capítulo 3: Diseño de Base de Datos

## Introducción

El presente capítulo describe el modelo de persistencia adoptado en el sistema DevManagerAPI. Se expone la estrategia general de organización en esquemas, el uso de identificadores únicos globales (GUID) como claves primarias, la clase base de auditoría compartida por todas las entidades, y el catálogo detallado de tablas agrupadas por dominio funcional. Cada sección presenta las tablas del esquema correspondiente con su estructura, restricciones de integridad y propósito dentro del sistema.

---

## 3.1 Estrategia General de Persistencia

### 3.1.1 Organización en Esquemas

El modelo de datos se distribuye en cinco esquemas lógicos de SQL Server, cada uno delimitando un dominio funcional independiente. Esta separación facilita la gestión de permisos a nivel de esquema, el razonamiento sobre dependencias entre módulos y la evolución independiente de cada dominio sin interferir con los demás.

```
┌──────────────────────────────────────────────────────────────┐
│                   DevManager — 5 Schemas                     │
├────────────────┬─────────────────────────────────────────────┤
│   config.*     │  Catálogos del sistema (tablas de referencia)│
│   iam.*        │  Identidad, acceso y organizaciones          │
│   talent.*     │  Perfiles, habilidades y certificaciones     │
│   projects.*   │  Proyectos, postulaciones y asignaciones     │
│   reporting.*  │  Reportería, reglas e inteligencia del agente│
└────────────────┴─────────────────────────────────────────────┘
```

Las tablas del esquema `config.*` se crean primero en el orden de ejecución del DDL, dado que los demás esquemas poseen claves foráneas que apuntan a ellas. Los esquemas `iam.*`, `talent.*`, `projects.*` y `reporting.*` incorporan la columna `OrganizationId` en todas sus tablas operativas, garantizando el aislamiento de datos entre organizaciones (multi-tenancy).

### 3.1.2 Identificadores Únicos Globales como Claves Primarias

La totalidad de las entidades principales utiliza `uniqueidentifier` (UUID v4) como clave primaria, generado con `NEWID()` por defecto en la base de datos. Esta decisión obedece a tres razones técnicas:

1. **Identidad distribuida**: los identificadores pueden generarse en cualquier capa de la aplicación (dominio, servicio, prueba) sin requerir un ciclo de ida a la base de datos para obtener el valor generado.
2. **Ausencia de exposición secuencial**: a diferencia de las claves enteras con `IDENTITY`, los GUIDs no revelan el volumen de registros ni permiten la enumeración trivial de recursos mediante incrementos en la URL.
3. **Compatibilidad multi-tenant**: en un entorno donde múltiples organizaciones comparten la misma instancia de base de datos, los GUIDs eliminan colisiones de identificadores entre organizaciones sin necesidad de claves compuestas artificiales.

Las excepciones se limitan a las tablas de catálogo del esquema `config.*`, que utilizan `tinyint` o `int` con valores semánticos fijos (p. ej., `1 = Borrador`, `2 = Abierto`) para facilitar las referencias a enumeraciones en el código de aplicación.

### 3.1.3 Clase Base de Auditoría: AuditableEntity

Las entidades auditables heredan un conjunto de siete campos comunes que proveen trazabilidad completa sobre el ciclo de vida del registro:

| Campo             | Tipo              | Descripción                                           |
|-------------------|-------------------|-------------------------------------------------------|
| `CreatedAt`       | `datetime2(3)`    | Fecha y hora UTC de creación (valor por defecto: `sysutcdatetime()`) |
| `CreatedByUserId` | `uniqueidentifier`| Usuario que creó el registro (puede ser `NULL` para registros del sistema) |
| `UpdatedAt`       | `datetime2(3)`    | Fecha y hora UTC de la última actualización (`NULL` si no se ha modificado) |
| `UpdatedByUserId` | `uniqueidentifier`| Usuario que realizó la última modificación            |
| `IsDeleted`       | `bit`             | Indicador de eliminación lógica (`0 = activo`, `1 = eliminado`). Valor por defecto: `0` |
| `DeletedAt`       | `datetime2(3)`    | Fecha y hora UTC de la eliminación lógica             |
| `DeletedByUserId` | `uniqueidentifier`| Usuario que ejecutó la eliminación lógica             |

El sistema **nunca ejecuta instrucciones `DELETE` físicas** sobre las entidades auditables. Todas las operaciones de eliminación se traducen en una actualización del campo `IsDeleted = 1`. Los repositorios aplican el filtro `WHERE IsDeleted = 0` en todas las consultas de lectura para garantizar que los datos eliminados sean transparentes para la lógica de negocio.

---

## 3.2 Esquema de Configuración (config.*)

### 3.2.1 Propósito

El esquema `config.*` alberga las tablas de catálogo del sistema: conjuntos de valores de referencia estables que los demás esquemas referencian mediante claves foráneas. Estas tablas no implementan `AuditableEntity` ni el patrón de multi-tenancy, dado que sus valores son compartidos a nivel de sistema o, en todo caso, se gestionan mediante migraciones controladas. Su propósito es evitar "valores mágicos" dispersos en el código fuente y centralizar la semántica de los estados y clasificaciones del dominio.

### 3.2.2 Tablas del Esquema config.*

| Tabla                         | Clave Primaria         | Columnas Clave                              | Propósito                                                                 |
|-------------------------------|------------------------|---------------------------------------------|---------------------------------------------------------------------------|
| `config.ProjectStatuses`      | `Id` (tinyint)         | `Code`, `Name`, `AllowsApplications` (bit)  | Estados del ciclo de vida de un proyecto. `AllowsApplications` controla si se aceptan postulaciones en ese estado. |
| `config.ProjectComplexityLevels` | `Id` (tinyint)      | `Code`, `Name`, `ExperienceMultiplier` (decimal 3,2) | Niveles de complejidad de proyecto (1-3). El multiplicador (rango 0.5–3.0) es utilizado por el agente para calcular el impacto en `SkillEvaluations.DeltaLevel`. |
| `config.ApplicationStatuses`  | `Id` (tinyint)         | `Code`, `Name`, `RequiresReviewNotes` (bit), `IsFinalState` (bit) | Estados del proceso de postulación. `RequiresReviewNotes` obliga al revisor a justificar el rechazo. |
| `config.AssignmentStatuses`   | `Id` (tinyint)         | `Code`, `Name`, `IsFinalState` (bit)        | Estados de una asignación de personal a un proyecto.                      |
| `config.SkillLevels`          | `Id` (tinyint, 1-5)    | `Code`, `Name`, `MinYearsExperience`        | Escala de dominio de habilidades. El check constraint `CK_SkillLevels_Id` restringe los valores al rango 1–5. |
| `config.SkillTypes`           | `Id` (tinyint, IDENTITY) | `Code`, `Name`                            | Clasificación tipológica de habilidades: `Hard`, `Soft`, `Language`.      |
| `config.SkillCategories`      | `Id` (int, IDENTITY)   | `Code`, `Name`, `ParentCategoryId` (int, auto-referenciada) | Taxonomía jerárquica de habilidades. La FK auto-referenciada permite estructuras de árbol. |
| `config.EvaluationSources`    | `Id` (tinyint)         | `Code`, `Name`, `IsAutomated` (bit)         | Origen de una evaluación de habilidad. `IsAutomated` distingue las fuentes automáticas (agente) de las manuales. |
| `config.ContributionScores`   | `Id` (tinyint, 1-5)    | `Code`, `Name`, `ExperienceBonus` (decimal 3,2) | Escala de puntuación de contribución en un proyecto (1–5). El check constraint `CK_ContributionScores_Id` restringe los valores al rango 1–5. |
| `config.AgentActionTypes`     | `Id` (int, IDENTITY)   | `Code`, `Name`, `RequiresApproval` (bit)    | Tipos de acción que puede registrar el agente. `RequiresApproval` determina si la acción requiere validación humana. |
| `config.AgentActionStatuses`  | `Id` (tinyint, IDENTITY) | `Code`, `Name`, `IsFinalState` (bit)      | Estados del flujo HITL (*Human-in-the-Loop*) de las acciones del agente.  |
| `config.SeniorityLevels`      | `Id` (tinyint, IDENTITY) | `Code`, `Name`, `MinYearsExperience`, `MaxYearsExperience` | Niveles de senioridad para clasificación de perfiles profesionales.        |

### 3.2.3 Valores de Estado Relevantes

Los estados críticos del sistema se codifican como valores enteros con semántica fija:

**ProjectStatus** (`config.ProjectStatuses`):

| Id | Code        | Descripción                                      |
|----|-------------|--------------------------------------------------|
| 1  | DRAFT       | Borrador — proyecto en definición inicial        |
| 2  | OPEN        | Abierto — acepta postulaciones (`AllowsApplications = 1`) |
| 3  | IN_PROGRESS | En Progreso — equipo asignado, proyecto en ejecución |
| 4  | CLOSED      | Cerrado — proyecto finalizado con evaluaciones   |
| 5  | CANCELLED   | Cancelado — proyecto interrumpido                |

**ApplicationStatus** (`config.ApplicationStatuses`):

| Id | Code      | Descripción                                         |
|----|-----------|-----------------------------------------------------|
| 1  | APPLIED   | Postulado — solicitud registrada, pendiente de revisión |
| 2  | APPROVED  | Aprobado — postulante aceptado                      |
| 3  | REJECTED  | Rechazado — requiere `ReviewNotes` obligatorio      |
| 4  | WITHDRAWN | Retirado — el candidato retiró su postulación       |

**AssignmentStatus** (`config.AssignmentStatuses`):

| Id | Code      | Descripción                              |
|----|-----------|------------------------------------------|
| 1  | ACTIVE    | Activo — asignación vigente              |
| 2  | COMPLETED | Completado — asignación finalizada       |
| 3  | CANCELLED | Cancelado — asignación interrumpida      |

---

## 3.3 Esquema de Identidad y Acceso (iam.*)

### 3.3.1 Propósito

El esquema `iam.*` (Identity & Access Management) gestiona las entidades de autenticación, autorización y organización. Constituye el núcleo del modelo multi-tenant: cada organización es una unidad aislada de datos, y todos los usuarios, roles y permisos existen dentro del contexto de una organización específica, con excepción de los roles globales del sistema (`OrganizationId = NULL` en `iam.Roles`).

### 3.3.2 Tablas del Esquema iam.*

| Tabla                   | Clave Primaria         | Columnas Clave                                                    | Claves Foráneas                                      | Propósito                                                      |
|-------------------------|------------------------|-------------------------------------------------------------------|------------------------------------------------------|----------------------------------------------------------------|
| `iam.Organizations`     | `Id` (uniqueidentifier) | `Name`, `LegalName`, `Nit`, `IsActive` (bit)                    | —                                                    | Unidad raíz del modelo multi-tenant. Cada tenant es una organización. |
| `iam.Users`             | `Id` (uniqueidentifier) | `OrganizationId`, `FirstName`, `LastName`, `Email`, `PasswordHash` (varbinary(512)), `PasswordSalt` (varbinary(256)), `IsActive` (bit) | `OrganizationId → iam.Organizations` | Credenciales y datos básicos de usuarios. El hash y el salt se almacenan como datos binarios. |
| `iam.Roles`             | `Id` (uniqueidentifier) | `OrganizationId` (nullable), `Name`, `Description`              | `OrganizationId → iam.Organizations`                 | Roles RBAC. `OrganizationId = NULL` indica un rol global del sistema. |
| `iam.Permissions`       | `Id` (uniqueidentifier) | `Code`, `Name`, `Module`                                         | —                                                    | Permisos atómicos de acceso, identificados por código único por módulo. |
| `iam.UserRoles`         | `(UserId, RoleId)` (PK compuesto) | `OrganizationId`, `CreatedAt`                        | `UserId → iam.Users`, `RoleId → iam.Roles`, `OrganizationId → iam.Organizations` | Relación muchos-a-muchos entre usuarios y roles, con contexto de organización. |
| `iam.RolePermissions`   | `(RoleId, PermissionId)` (PK compuesto) | `CreatedAt`                                      | `RoleId → iam.Roles`, `PermissionId → iam.Permissions` | Asignación de permisos a roles.                             |
| `iam.UserPermissions`   | `(UserId, PermissionId)` (PK compuesto) | `OrganizationId`, `IsGranted` (bit)              | `UserId → iam.Users`, `PermissionId → iam.Permissions`, `OrganizationId → iam.Organizations` | Permisos directos por usuario (sobreescritura fina del RBAC). `IsGranted = 0` permite la denegación explícita. |

### 3.3.3 Restricciones e Índices de Relevancia

- **`CONSTRAINT UQ_Organizations_Nit`**: garantiza que no existan dos organizaciones con el mismo número de identificación tributaria (`Nit`). Es una restricción `UNIQUE` sobre la columna `Nit` de `iam.Organizations`.
- **`UX_Users_Org_Email`** (índice único filtrado): garantiza que el correo electrónico sea único dentro de cada organización, excluyendo los registros con `IsDeleted = 1`. Esto permite "reciclar" direcciones de correo si un usuario es eliminado lógicamente.
- **`UX_Roles_Org_Name`** (índice único filtrado): el nombre de un rol es único dentro de una organización para registros activos.
- **Almacenamiento binario de contraseñas**: `PasswordHash` se almacena como `varbinary(512)` y `PasswordSalt` como `varbinary(256)`, producidos por `HMACSHA512`. Este diseño impide la lectura directa de contraseñas desde la base de datos, incluso con acceso directo al motor.

### 3.3.4 Columna OrganizationId (Multi-tenancy)

Todas las tablas operativas del esquema `iam.*` (excepto `iam.Permissions`, que son recursos globales del sistema) contienen la columna `OrganizationId` como clave foránea hacia `iam.Organizations`. Esta columna es el mecanismo de aislamiento de datos: cada consulta generada por los repositorios incluye el predicado `WHERE OrganizationId = @orgId`, con el valor extraído del *claim* JWT correspondiente.

---

## 3.4 Esquema de Talento (talent.*)

### 3.4.1 Propósito

El esquema `talent.*` centraliza el modelo de capital humano de la organización: perfiles profesionales de empleados, catálogo de habilidades, niveles de dominio, certificaciones y el historial de evaluaciones de habilidades generado por el sistema. Es el esquema que provee la mayor parte del contexto que el agente inteligente consume para sus análisis.

### 3.4.2 Tablas del Esquema talent.*

| Tabla                     | Clave Primaria                | Columnas Clave                                                                            | Claves Foráneas                                                                     | Propósito                                                                       |
|---------------------------|-------------------------------|-------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|---------------------------------------------------------------------------------|
| `talent.EmployeeProfiles` | `UserId` (uniqueidentifier)   | `OrganizationId`, `Bio` (nvarchar(800)), `YearsExperience` (int), `LinkedInUrl`, `PortfolioUrl` | `UserId → iam.Users`, `OrganizationId → iam.Organizations`                      | Perfil profesional extendido del empleado. La PK coincide con `iam.Users.Id` (relación 1-a-1). Hereda `AuditableEntity`. |
| `talent.Skills`           | `Id` (uniqueidentifier)       | `OrganizationId` (nullable), `Name`, `Category`, `SkillType` (nvarchar(20): `Hard`/`Soft`/`Language`) | `OrganizationId → iam.Organizations`                                             | Catálogo de habilidades. `OrganizationId = NULL` indica habilidades globales del sistema. Hereda `AuditableEntity`. |
| `talent.EmployeeSkills`   | `Id` (uniqueidentifier)       | `OrganizationId`, `UserId`, `SkillId`, `Level` (tinyint 1-5), `EvidenceUrl`, `ExperienceDescription`, `LastValidatedAt`, `ValidatedByUserId` | `OrganizationId → iam.Organizations`, `UserId → iam.Users`, `SkillId → talent.Skills`, `ValidatedByUserId → iam.Users`, `Level → config.SkillLevels` | Dominio de una habilidad por un empleado. El nivel sigue la escala 1–5. Hereda `AuditableEntity`. |
| `talent.Certifications`   | `Id` (uniqueidentifier)       | `OrganizationId`, `UserId`, `Name`, `Issuer`, `IssueDate` (date), `ExpirationDate` (date), `EvidenceUrl` | `OrganizationId → iam.Organizations`, `UserId → iam.Users`                      | Certificaciones profesionales del empleado con URL de evidencia. Hereda `AuditableEntity`. |
| `talent.SkillEvaluations` | `Id` (uniqueidentifier)       | `OrganizationId`, `UserId`, `SkillId`, `Source` (tinyint), `ProjectId` (nullable), `DeltaLevel` (smallint -5..5), `Reason` | `OrganizationId → iam.Organizations`, `UserId → iam.Users`, `SkillId → talent.Skills`, `ProjectId → projects.Projects`, `Source → config.EvaluationSources` | Registro histórico inmutable de cambios en el nivel de una habilidad. No implementa soft delete ni `UpdatedAt`. |

### 3.4.3 Restricciones Relevantes

- **`UX_EmployeeSkills_Org_User_Skill`** (índice único filtrado): impide que un empleado registre el mismo skill dos veces dentro de la misma organización para registros activos.
- **`CK_SkillEvaluations_DeltaLevel`**: restringe el delta de nivel al rango `[-5, 5]`, definido como `CONSTRAINT CK_SkillEvaluations_DeltaLevel CHECK (DeltaLevel BETWEEN -5 AND 5)`.
- **`Level` en `talent.EmployeeSkills`**: es de tipo `tinyint` con FK a `config.SkillLevels`, cuyo check constraint restringe los valores al rango 1–5.

### 3.4.4 Campo DeltaLevel y Fuentes de Evaluación

`talent.SkillEvaluations.DeltaLevel` es de tipo `smallint` y representa la variación incremental (positiva o negativa) en el nivel de dominio de una habilidad, como consecuencia de un evento evaluativo. El campo `Source` determina el origen de la evaluación y referencia `config.EvaluationSources`:

**SkillEvaluationSource** (`config.EvaluationSources`):

| Id | Code             | Descripción                                                  |
|----|------------------|--------------------------------------------------------------|
| 1  | PROJECT          | Evaluación derivada de la participación en un proyecto       |
| 2  | MANUAL           | Evaluación ingresada manualmente por un administrador        |
| 3  | SYSTEM_RULE      | Evaluación generada por una regla del sistema                |
| 4  | CERTIFICATION    | Evaluación derivada del registro de una certificación        |
| 5  | SELF_ASSESSMENT  | Auto-evaluación registrada por el propio empleado            |

### 3.4.5 Columna OrganizationId (Multi-tenancy)

Todas las tablas del esquema `talent.*` incluyen `OrganizationId` como clave foránea obligatoria hacia `iam.Organizations`, a excepción de `talent.Skills` donde es nullable para permitir skills globales del sistema. Los índices filtrados de cada tabla incorporan la condición `IsDeleted = 0` para optimizar las consultas de lectura operativa.

---

## 3.5 Esquema de Proyectos (projects.*)

### 3.5.1 Propósito

El esquema `projects.*` modela el ciclo de vida completo de un proyecto de desarrollo: su definición, los requerimientos de habilidades, los roles disponibles, el proceso de postulación, las asignaciones de personal y el registro de participaciones con retroalimentación cualitativa. Es el esquema con mayor número de interrelaciones, dado que consolida entidades de `iam.*`, `talent.*` y `config.*`.

### 3.5.2 Tablas del Esquema projects.*

| Tabla                              | Clave Primaria         | Columnas Clave                                                                                            | Claves Foráneas                                                                                              | Propósito                                                                              |
|------------------------------------|------------------------|-----------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------|
| `projects.Projects`                | `Id` (uniqueidentifier) | `OrganizationId`, `Code`, `Name`, `Description`, `StartDate`, `EndDate`, `ComplexityLevel` (tinyint), `Status` (tinyint) | `OrganizationId → iam.Organizations`, `Status → config.ProjectStatuses`, `ComplexityLevel → config.ProjectComplexityLevels` | Entidad raíz del dominio de proyectos. Hereda `AuditableEntity` (sin `DeletedByUserId`). |
| `projects.ProjectSkillRequirements` | `Id` (uniqueidentifier) | `OrganizationId`, `ProjectId`, `SkillId`, `RequiredLevel` (tinyint 1-5), `IsMandatory` (bit)             | `OrganizationId → iam.Organizations`, `ProjectId → projects.Projects`, `SkillId → talent.Skills`, `RequiredLevel → config.SkillLevels` | Requerimientos de habilidades del proyecto. `IsMandatory = 1` (peso 60%) vs `IsMandatory = 0` (peso 20%) en el algoritmo de matching. |
| `projects.ProjectRoles`            | `Id` (uniqueidentifier) | `OrganizationId`, `ProjectId`, `Name`, `NeededCount` (int ≥ 1)                                           | `OrganizationId → iam.Organizations`, `ProjectId → projects.Projects`                                        | Roles y vacantes disponibles en el proyecto. `CK_ProjectRoles_NeededCount` garantiza al menos una vacante. |
| `projects.ProjectApplications`     | `Id` (uniqueidentifier) | `OrganizationId`, `ProjectId`, `UserId`, `Motivation` (nvarchar(800)), `Status` (tinyint), `ReviewedByUserId`, `ReviewedAt`, `ReviewNotes` | `OrganizationId → iam.Organizations`, `ProjectId → projects.Projects`, `UserId → iam.Users`, `ReviewedByUserId → iam.Users`, `Status → config.ApplicationStatuses` | Postulaciones de empleados a proyectos. `ReviewNotes` es obligatorio cuando el estado es `REJECTED`. |
| `projects.ProjectAssignments`      | `Id` (uniqueidentifier) | `OrganizationId`, `ProjectId`, `UserId`, `ProjectRoleId` (nullable), `AssignedByUserId`, `AssignedAt`, `Status` (tinyint), `EndedAt` | `OrganizationId → iam.Organizations`, `ProjectId → projects.Projects`, `UserId → iam.Users`, `ProjectRoleId → projects.ProjectRoles`, `AssignedByUserId → iam.Users`, `Status → config.AssignmentStatuses` | Asignaciones directas de personal al proyecto, vinculadas a un rol específico. |
| `projects.ProjectParticipation`    | `Id` (uniqueidentifier) | `OrganizationId`, `ProjectId`, `UserId`, `RoleName`, `ContributionScore` (tinyint, nullable), `FeedbackComments` (nvarchar(max)), `CompletedAt` | `OrganizationId → iam.Organizations`, `ProjectId → projects.Projects`, `UserId → iam.Users`, `ContributionScore → config.ContributionScores` | Registro histórico de participación con puntuación de contribución y retroalimentación cualitativa. `FeedbackComments` es consumido por el agente para análisis de lenguaje natural. |

### 3.5.3 Restricciones e Índices de Relevancia

- **`UX_Projects_Org_Code`** (índice único filtrado): el código de proyecto es único dentro de una organización para registros no eliminados y no nulos.
- **`UX_ProjectApplications_Org_Project_User`**: un empleado solo puede tener una postulación activa por proyecto y organización.
- **`UX_ProjectAssignments_Org_Project_User_Active`** (índice único filtrado con `Status = 1`): un empleado no puede tener dos asignaciones activas al mismo proyecto dentro de la misma organización.
- **`UX_ProjectSkillRequirements_Org_Project_Skill`**: una habilidad no puede requerirse dos veces en el mismo proyecto.

### 3.5.4 Columna OrganizationId (Multi-tenancy)

Todas las tablas del esquema `projects.*` incluyen `OrganizationId` como clave foránea obligatoria, asegurando el aislamiento total de datos entre organizaciones. Los índices sobre `(OrganizationId, Status)` optimizan los filtros combinados de estado y tenant que son frecuentes en las consultas operativas.

---

## 3.6 Esquema de Reportería e Inteligencia (reporting.*)

### 3.6.1 Propósito

El esquema `reporting.*` alberga las estructuras de datos para la inteligencia de negocio periódica y las trazas de auditoría del agente inteligente. A diferencia de los esquemas operativos, varias de sus tablas almacenan documentos JSON serializados (`nvarchar(max)`) como mecanismo de persistencia de resultados analíticos, lo que otorga flexibilidad al modelo sin requerir migraciones cada vez que cambia la estructura del análisis.

### 3.6.2 Tablas del Esquema reporting.*

| Tabla                           | Clave Primaria         | Columnas Clave                                                                                        | Claves Foráneas                                                              | Propósito                                                                           |
|---------------------------------|------------------------|-------------------------------------------------------------------------------------------------------|------------------------------------------------------------------------------|-------------------------------------------------------------------------------------|
| `reporting.ReportSnapshots`     | `Id` (uniqueidentifier) | `OrganizationId`, `SnapshotDate` (date), `JsonPayload` (nvarchar(max))                               | `OrganizationId → iam.Organizations`                                         | Instantáneas periódicas del estado de talento de la organización en formato JSON. Generadas por `ReportSnapshotGeneratorService`. |
| `reporting.RecommendationRules` | `Id` (uniqueidentifier) | `OrganizationId`, `Name`, `ConditionExpr` (nvarchar(800)), `RecommendationText` (nvarchar(800)), `IsActive` (bit) | `OrganizationId → iam.Organizations`                                      | Reglas de recomendación configurables por organización, evaluadas por el agente. Hereda soft delete parcial (`IsDeleted`). |
| `reporting.RecommendationLogs`  | `Id` (uniqueidentifier) | `OrganizationId`, `GeneratedAt`, `GeneratedByUserId` (nullable), `ResultJson` (nvarchar(max))        | `OrganizationId → iam.Organizations`, `GeneratedByUserId → iam.Users`        | Registro de resultados de recomendaciones generadas, tanto automáticas como manuales. |
| `reporting.AgentActions`        | `Id` (uniqueidentifier) | `OrganizationId`, `ActionType` (nvarchar(80)), `Description`, `InputData` (nvarchar(max)), `OutputData` (nvarchar(max)), `Status` (nvarchar(40)), `ExecutedByUserId` (nullable), `ApprovedByUserId` (nullable), `CreatedAt`, `ApprovedAt` (nullable) | `OrganizationId → iam.Organizations`, `ExecutedByUserId → iam.Users`, `ApprovedByUserId → iam.Users` | Auditoría completa de acciones del agente, incluyendo el flujo HITL. |
| `reporting.AgentConfiguration`  | `OrganizationId` (uniqueidentifier, PK) | `EnableAutoValidation` (bit), `RequireHumanApproval` (bit), `MinConfidenceThreshold` (decimal 5,2), `MaxCandidatesPerMatch` (int), `EnableBackgroundOptimization` (bit), `CreatedAt`, `UpdatedAt` | `OrganizationId → iam.Organizations` | Configuración operativa del agente por organización. La PK es `OrganizationId`, garantizando una única configuración por tenant. |

### 3.6.3 Restricciones e Índices de Relevancia

- **`UX_ReportSnapshots_Org_Date`** (índice único): garantiza que solo exista un snapshot por organización y fecha de instantánea. Esto permite que `ReportSnapshotGeneratorService` ejecute operaciones de tipo *upsert* deterministas sin duplicar registros.
- **`CK_AgentConfiguration_Confidence`**: restringe `MinConfidenceThreshold` al rango `[0, 100]`.
- **`CK_AgentConfiguration_MaxCandidates`**: restringe `MaxCandidatesPerMatch` al rango `[1, 50]`.
- **`IX_AgentActions_Org_Status_Date`**: índice compuesto sobre `(OrganizationId, Status, CreatedAt DESC)` para optimizar las consultas de acciones pendientes de aprobación, que son el caso de uso más frecuente en el flujo HITL.

### 3.6.4 Flujo de Estados del Agente (AgentActionStatus)

Los valores de estado de `reporting.AgentActions.Status` siguen un flujo determinista:

**AgentActionStatus**:

| Valor         | Descripción                                                     |
|---------------|-----------------------------------------------------------------|
| `SUCCESS`     | Acción ejecutada y completada exitosamente de forma automática  |
| `FAILED`      | Acción ejecutada con error                                      |
| `PENDING_APPROVAL` | Acción generada por el agente, en espera de revisión humana |
| `APPROVED`    | Acción aprobada por un usuario con permisos de supervisión      |
| `REJECTED`    | Acción rechazada; `ApprovedByUserId` registra al revisor        |

```
                    ┌─────────────────────┐
   Acción Agente ──►│  PENDING_APPROVAL   │
                    └──────────┬──────────┘
                               │
              ┌────────────────┴────────────────┐
              ▼                                 ▼
        ┌──────────┐                      ┌──────────┐
        │ APPROVED │                      │ REJECTED │
        └──────────┘                      └──────────┘
              │
              ▼
        ┌──────────┐
        │ EXECUTED │  (flujo posterior a aprobación)
        └──────────┘
```

El campo `ApprovedByUserId` en `reporting.AgentActions` registra la identidad del usuario que aprobó o rechazó la acción, y `ApprovedAt` captura el instante de la decisión, proporcionando una traza de auditoría completa del proceso HITL.

---

## 3.7 Entidades del Agente Inteligente

### 3.7.1 Posicionamiento en el Modelo de Datos

Las entidades del agente no poseen un esquema SQL dedicado: se mapean al esquema `reporting.*` por tratarse de elementos de auditoría y configuración operativa, en concordancia con su naturaleza de supervisión del sistema. En el código fuente de la aplicación, estas entidades residen en `Domain/Entities/Agent/` como POCOs puros, y sus configuraciones de EF Core se definen en `Infrastructure/Data/Configuration/AgentConfiguration.cs`.

### 3.7.2 AgentActions (`reporting.AgentActions`)

| Columna              | Tipo              | Descripción                                                     |
|----------------------|-------------------|-----------------------------------------------------------------|
| `Id`                 | uniqueidentifier  | Identificador único de la acción (PK, `NEWID()`)                |
| `OrganizationId`     | uniqueidentifier  | Organización en cuyo contexto se ejecutó la acción (FK)         |
| `ActionType`         | nvarchar(80)      | Tipo semántico: `SKILL_VALIDATION`, `PROJECT_MATCHING`, etc.    |
| `Description`        | nvarchar(500)     | Descripción legible por humanos de la acción realizada          |
| `InputData`          | nvarchar(max)     | Datos de entrada serializados en JSON                           |
| `OutputData`         | nvarchar(max)     | Datos de salida / resultado del agente serializados en JSON     |
| `Status`             | nvarchar(40)      | Estado del flujo HITL (ver sección 3.6.4)                       |
| `ExecutedByUserId`   | uniqueidentifier  | Usuario que solicitó la acción (`NULL` si es automática)        |
| `ApprovedByUserId`   | uniqueidentifier  | Usuario que aprobó o rechazó la acción en el flujo HITL (`NULL` si está pendiente) |
| `CreatedAt`          | datetime2(3)      | Marca de tiempo de creación (UTC)                               |
| `ApprovedAt`         | datetime2(3)      | Marca de tiempo de la decisión de aprobación/rechazo (`NULL` si está pendiente) |

Esta tabla es el registro central de auditoría del agente: toda interacción del sistema de IA con los datos operativos produce un registro en `reporting.AgentActions` antes de ejecutarse, garantizando así la trazabilidad y la supervisión humana mediante el patrón HITL.

### 3.7.3 AgentConfiguration (`reporting.AgentConfiguration`)

La tabla `reporting.AgentConfiguration` almacena los parámetros operativos del agente para cada organización. Su clave primaria es `OrganizationId`, lo que impone una relación uno-a-uno entre organización y configuración del agente:

| Columna                        | Tipo           | Descripción                                                         |
|--------------------------------|----------------|---------------------------------------------------------------------|
| `OrganizationId`               | uniqueidentifier | PK y FK hacia `iam.Organizations`                                 |
| `EnableAutoValidation`         | bit            | Habilita la validación automática de habilidades por el agente      |
| `RequireHumanApproval`         | bit            | Cuando `1`, todas las acciones del agente requieren aprobación HITL |
| `MinConfidenceThreshold`       | decimal(5,2)   | Umbral mínimo de confianza (0–100) para ejecutar una recomendación  |
| `MaxCandidatesPerMatch`        | int            | Número máximo de candidatos devueltos por el algoritmo de matching (1–50) |
| `EnableBackgroundOptimization` | bit            | Activa el servicio de optimización en segundo plano                 |
| `CreatedAt`                    | datetime2(3)   | Fecha de creación de la configuración                               |
| `UpdatedAt`                    | datetime2(3)   | Última actualización de los parámetros                              |

### 3.7.4 AgentTool (entidad en memoria)

La entidad `Domain.Entities.Agent.AgentTool` representa una herramienta disponible para el agente mediante el patrón *MCP Tool Use*. A diferencia de `AgentActions` y `AgentConfiguration`, **esta entidad no persiste en la base de datos**: es instanciada en memoria por `AgentService` durante el ciclo de vida de una consulta. Su estructura incluye `Name`, `Description`, `Schema` (JSON Schema de parámetros) y un delegado `Handler` que referencia la función de ejecución correspondiente.

---

## 3.8 Síntesis del Capítulo

El modelo de persistencia de DevManagerAPI se organiza en cinco esquemas SQL Server con responsabilidades bien delimitadas. La siguiente tabla consolida el inventario completo de las 25 tablas del sistema:

| Esquema        | Tabla                            | Multi-tenant | AuditableEntity | Notas clave                                      |
|----------------|----------------------------------|:------------:|:---------------:|--------------------------------------------------|
| `config`       | `ProjectStatuses`                | No           | No              | tinyint PK, valores fijos semánticos             |
| `config`       | `ProjectComplexityLevels`        | No           | No              | `ExperienceMultiplier` decimal(3,2), rango 0.5–3.0 |
| `config`       | `ApplicationStatuses`            | No           | No              | `RequiresReviewNotes` bit                        |
| `config`       | `AssignmentStatuses`             | No           | No              | `IsFinalState` bit                               |
| `config`       | `SkillLevels`                    | No           | No              | Check constraint Id BETWEEN 1 AND 5              |
| `config`       | `SkillTypes`                     | No           | No              | `Hard`, `Soft`, `Language`                       |
| `config`       | `SkillCategories`                | No           | No              | Auto-referenciada (árbol jerárquico)             |
| `config`       | `EvaluationSources`              | No           | No              | `IsAutomated` bit                                |
| `config`       | `ContributionScores`             | No           | No              | Check constraint Id BETWEEN 1 AND 5              |
| `config`       | `AgentActionTypes`               | No           | No              | `RequiresApproval` bit                           |
| `config`       | `AgentActionStatuses`            | No           | No              | `IsFinalState` bit                               |
| `config`       | `SeniorityLevels`                | No           | No              | `MinYearsExperience` / `MaxYearsExperience`      |
| `iam`          | `Organizations`                  | Raíz         | Sí              | `UQ_Organizations_Nit`, IsActive bit             |
| `iam`          | `Users`                          | Sí           | Sí              | `PasswordHash` varbinary(512), `PasswordSalt` varbinary(256) |
| `iam`          | `Roles`                          | Sí (nullable)| Parcial         | `OrganizationId = NULL` = rol global             |
| `iam`          | `Permissions`                    | No           | Parcial         | Recurso global del sistema                       |
| `iam`          | `UserRoles`                      | Sí           | No              | PK compuesta (UserId, RoleId)                    |
| `iam`          | `RolePermissions`                | No           | No              | PK compuesta (RoleId, PermissionId)              |
| `iam`          | `UserPermissions`                | Sí           | No              | `IsGranted` bit — denegación explícita posible   |
| `talent`       | `EmployeeProfiles`               | Sí           | Sí              | PK = UserId (1-a-1 con iam.Users)                |
| `talent`       | `Skills`                         | Sí (nullable)| Sí              | `OrganizationId = NULL` = skill global           |
| `talent`       | `EmployeeSkills`                 | Sí           | Sí              | Level tinyint 1-5, FK a config.SkillLevels       |
| `talent`       | `Certifications`                 | Sí           | Sí              | `EvidenceUrl` para documentación de evidencia    |
| `talent`       | `SkillEvaluations`               | Sí           | Parcial         | `DeltaLevel` smallint -5..5, sin soft delete completo |
| `projects`     | `Projects`                       | Sí           | Sí (parcial)    | `ComplexityLevel` FK a config, `Status` FK a config |
| `projects`     | `ProjectSkillRequirements`       | Sí           | Parcial         | `IsMandatory` bit — peso 60%/20% en matching     |
| `projects`     | `ProjectRoles`                   | Sí           | Parcial         | `NeededCount >= 1` check constraint              |
| `projects`     | `ProjectApplications`            | Sí           | Parcial         | `ReviewNotes` obligatorio en rechazo             |
| `projects`     | `ProjectAssignments`             | Sí           | Parcial         | Índice único activo por (Org, Project, User)     |
| `projects`     | `ProjectParticipation`           | Sí           | Parcial         | `FeedbackComments` nvarchar(max) para NLP        |
| `reporting`    | `ReportSnapshots`                | Sí           | No              | `UX_ReportSnapshots_Org_Date` índice único       |
| `reporting`    | `RecommendationRules`            | Sí           | Parcial         | `ConditionExpr` nvarchar(800)                    |
| `reporting`    | `RecommendationLogs`             | Sí           | No              | `ResultJson` nvarchar(max)                       |
| `reporting`    | `AgentActions`                   | Sí           | No              | Flujo HITL: PENDING_APPROVAL → APPROVED/REJECTED |
| `reporting`    | `AgentConfiguration`             | Sí (1-a-1)   | No              | PK = OrganizationId, parámetros del agente       |

El diseño adoptado privilegia la coherencia del modelo multi-tenant sobre la simplificación del esquema: la columna `OrganizationId` presente en la práctica totalidad de las tablas operativas garantiza que ninguna consulta pueda devolver datos entre organizaciones, incluso ante errores de programación en la capa de aplicación. La elección de GUIDs como claves primarias y el patrón AuditableEntity de eliminación lógica son decisiones arquitectónicas transversales que impactan en la seguridad, la trazabilidad y la capacidad de evolución del sistema.

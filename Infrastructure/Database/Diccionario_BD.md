***

# 📘 Diccionario de Datos: DevManager

**Versión:** 1.0 (Arquitectura Final)
**Fecha:** Enero 2026
**Motor:** SQL Server
**Enfoque:** Multi-tenant (Multi-organización)

---

## 🏗️ Convenciones Globales y Patrones de Diseño

Antes de detallar las tablas, es fundamental entender los patrones que se repiten en todo el sistema para garantizar la robustez:

1.  **Multi-tenencia (Multi-tenant):**
    *   Todas las tablas (excepto catálogos globales teóricos) tienen la columna **`OrganizationId`**.
    *   **Regla:** Ninguna consulta debe ejecutarse sin filtrar por `OrganizationId`. Esto aísla los datos de cada empresa cliente.

2.  **Identificadores (Primary Keys):**
    *   Se utiliza **`uniqueidentifier` (GUID)** para todas las llaves primarias (`Id`). Esto facilita la replicación, evita colisiones en migraciones y oscurece la cantidad real de registros ante usuarios maliciosos.

3.  **Auditoría y Soft Delete (Borrado Lógico):**
    *   Casi todas las tablas incluyen:
        *   `CreatedAt`, `CreatedByUserId`: Quién y cuándo creó el registro.
        *   `UpdatedAt`, `UpdatedByUserId`: Trazabilidad de cambios.
        *   `IsDeleted`: **Bit**. Si es `1`, el registro está "borrado" para el usuario, pero existe en BD.
        *   `DeletedAt`: Cuándo se borró.

---

## 📂 Esquema: IAM (Identity & Access Management)
*Manejo de acceso, empresas y roles.*

### 1. `iam.Organizations`
La entidad raíz. Representa a la empresa cliente que contrata DevManager.
*   **Campos Clave:**
    *   `Nit`: Identificador tributario (Único).
    *   `IsActive`: Interruptor maestro para bloquear el acceso a toda una empresa.

### 2. `iam.Users`
Usuarios del sistema (Administradores, Gerentes, Empleados).
*   **Campos Clave:**
    *   `Email`: Único por Organización.
    *   `PasswordHash/Salt`: Seguridad criptográfica.
    *   `OrganizationId`: 🔗 FK a Organizations.

### 3. `iam.Roles` y `iam.UserRoles`
Sistema RBAC (Role-Based Access Control).
*   **Lógica:** Un usuario puede tener múltiples roles (ej: "Desarrollador" y "Scrum Master").
*   `iam.Roles.OrganizationId`: Si es `NULL`, es un rol del sistema (Global). Si tiene ID, es un rol personalizado por la empresa.

---

## 📂 Esquema: TALENT (Gestión de Talento)
*El corazón del perfilamiento de empleados.*

### 4. `talent.EmployeeProfiles`
Extensión de la tabla de usuarios con información profesional.
*   **Uso:** Información "estática" o de presentación (Bio, Links, Años de experiencia).

### 5. `talent.Skills`
Catálogo de habilidades (Java, Python, Liderazgo, SQL).
*   **Campos Clave:**
    *   `Name`: Nombre de la habilidad.
    *   `Category`: Agrupación (Frontend, Backend, Metodologías).
    *   `SkillType` ⭐ *(Agregado Experto)*: Valores sugeridos: 'Hard', 'Soft'. Permite al Agente Inteligente diferenciar entre habilidades técnicas y blandas para los reportes.

### 6. `talent.EmployeeSkills`
Tabla pivote que dice "Qué usuario sabe qué habilidad y en qué nivel".
*   **Campos Clave:**
    *   `Level`: Escala 1 a 5 (Novato a Experto).
    *   `LastValidatedAt`: Fecha de última confirmación de la habilidad.
    *   `ValidatedByUserId` ⭐ *(Agregado Experto)*: 🔗 FK a Users. Es vital para auditoría. Si es NULL o un ID especial, indica que fue el **Agente Inteligente** quien validó la habilidad automáticamente tras un proyecto.

### 7. `talent.Certifications`
Validación externa de conocimientos (Diplomas, Certificaciones oficiales).

---

## 📂 Esquema: PROJECTS (Gestión de Proyectos y Asignaciones)
*Donde ocurre la interacción dinámica entre talento y necesidades.*

### 8. `projects.Projects`
La definición del trabajo a realizar.
*   **Campos Clave:**
    *   `Status`: 1 (Borrador) -> 2 (Abierto) -> 3 (En Progreso) -> 4 (Cerrado) -> 5 (Cancelado).
    *   `ComplexityLevel` ⭐ *(Agregado Experto)*: (1: Baja, 2: Media, 3: Alta).
    *   **Lógica del Agente:** Un proyecto de complejidad "Alta" otorgará más puntos de experiencia al finalizar que uno de complejidad "Baja".

### 9. `projects.ProjectSkillRequirements`
Define el "Perfil Ideal" para el proyecto.
*   **Uso:** El motor de búsqueda compara estos registros contra `talent.EmployeeSkills` para recomendar candidatos.
*   `RequiredLevel`: Nivel mínimo necesario (1-5).
*   `IsMandatory`: Si es `true`, el candidato DEBE tenerla. Si es `false`, es un "Plus".

### 10. `projects.ProjectApplications`
Postulaciones iniciadas por los empleados (Autogestión).
*   **Campos Clave:**
    *   `Motivation`: Carta de presentación breve.
    *   `ReviewNotes` ⭐ *(Agregado Experto)*: Feedback del gerente al aprobar/rechazar. Permite al empleado entender por qué no fue seleccionado.

### 11. `projects.ProjectAssignments`
Asignación oficial. El empleado está trabajando activamente en el proyecto.
*   **Restricción:** Un usuario no puede tener dos asignaciones activas (`Status = 1`) para el mismo proyecto simultáneamente.

### 12. `projects.ProjectParticipation`
**Historial.** Cuando una asignación termina, se guarda aquí el registro permanente del desempeño.
*   **Campos Clave:**
    *   `ContributionScore`: Calificación (1-5) del desempeño en ESTE proyecto específico.
    *   `FeedbackComments` ⭐ *(Agregado Experto)*: Texto libre sobre el desempeño.
    *   **Importancia para IA:** El Agente Inteligente procesará este texto (NLP) y el puntaje para recalibrar las habilidades del usuario.

---

## 📂 Esquema: REPORTING (BI & Agente Inteligente)
*Módulos para análisis de datos y automatización de decisiones.*

### 13. `talent.SkillEvaluations`
Libro mayor (Ledger) de cambios en las habilidades.
*   **Propósito:** Responder "¿Cómo llegó este usuario a nivel 5 en React?".
*   **Campos Clave:**
    *   `Source`: (1: Proyecto, 2: Manual/RRHH, 3: Regla del Sistema).
    *   `DeltaLevel`: Cuánto subió o bajó el nivel (ej: +1).
    *   `Reason`: Justificación del cambio.

### 14. `reporting.ReportSnapshots`
Fotos históricas de los datos.
*   **Uso:** Para gráficas de "Evolución de talento en el último año". Guarda JSONs pre-calculados para no saturar la BD transaccional.

### 15. `reporting.RecommendationRules`
Motor de reglas del Agente Inteligente.
*   `ConditionExpr`: Lógica almacenada (ej: "Si Proyecto > 3 Completados AND Promedio > 4.5").
*   `RecommendationText`: La sugerencia (ej: "Promover a Senior").

### 16. `reporting.RecommendationLogs`
Bitácora de lo que el Agente sugirió.
*   Permite auditar si las sugerencias del agente están siendo útiles o ignoradas por los gerentes.

---

## 🔄 Flujo de Datos Crítico (El Ciclo de Vida DevManager)

1.  **Alta:** Se crea **User** y **EmployeeProfile**.
2.  **Skill Inicial:** El usuario carga sus habilidades en **EmployeeSkills** (Nivel declarado).
3.  **Proyecto:** Gerente crea **Project** con **ComplexityLevel** y define **ProjectSkillRequirements**.
4.  **Match:** Empleado aplica (**ProjectApplications**).
5.  **Trabajo:** Gerente aprueba y crea **ProjectAssignments** (Estado Activo).
6.  **Cierre:** El proyecto finaliza. La asignación pasa a **ProjectParticipation**.
7.  **Evaluación (Trigger/Job):**
    *   El gerente califica (`ContributionScore` + `FeedbackComments`).
    *   **El Agente Inteligente actúa:** Lee la complejidad del proyecto y la calificación.
    *   Si el desempeño fue alto + proyecto difícil -> Inserta en **SkillEvaluations** (+1 Nivel) y actualiza **EmployeeSkills**.
8.  **Reporte:** La empresa visualiza el crecimiento del talento en los dashboards alimentados por el esquema **Reporting**.
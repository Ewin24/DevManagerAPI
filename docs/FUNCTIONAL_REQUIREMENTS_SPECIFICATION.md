# 📋 Especificación Formal de Requerimientos Funcionales

## Sistema DevManager - Gestión de Talento con IA

**Versión:** 1.0  
**Fecha:** 2 de Febrero de 2026  
**Estado:** En Revisión

---

## 📑 Índice

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Módulo IAM - Identidad y Gestión de Acceso](#2-módulo-iam---identidad-y-gestión-de-acceso)
3. [Módulo Talent - Gestión de Talento y Perfiles](#3-módulo-talent---gestión-de-talento-y-perfiles)
4. [Módulo Projects - Gestión de Proyectos y Postulaciones](#4-módulo-projects---gestión-de-proyectos-y-postulaciones)
5. [Módulo Reporting - Reportes y Agente Inteligente](#5-módulo-reporting---reportes-y-agente-inteligente)
6. [Requerimientos Transversales - Accesibilidad](#6-requerimientos-transversales---accesibilidad-wcag-22-aa)
7. [Matriz de Trazabilidad](#7-matriz-de-trazabilidad)

---

## 1. Resumen Ejecutivo

### 1.1 Propósito del Documento

Este documento especifica los requerimientos funcionales del sistema DevManager, diseñado para optimizar la gestión del capital humano tecnológico. La especificación sigue el formato **EARS (Easy Approach to Requirements Syntax)** para garantizar claridad y verificabilidad.

### 1.2 Análisis de Deconstrucción

#### Actores Principales

| Actor | Responsabilidades |
|-------|-------------------|
| **Administrador Global** | Gestión de roles base y configuración multi-tenancy |
| **Administrador de Organización** | Gestión de usuarios y políticas internas |
| **Líder de Proyecto** | Definición de requisitos técnicos y evaluación de candidatos |
| **Talento/Empleado** | Gestión de competencias y postulación proactiva |

#### Dominios de Datos (Esquemas SQL)

| Esquema | Propósito |
|---------|-----------|
| `iam.*` | Identidad, acceso y segregación multi-tenant |
| `talent.*` | Repositorio de competencias y evidencias |
| `projects.*` | Ciclo de vida operativo y asignaciones |
| `reporting.*` | Inteligencia de negocios y motor de reglas IA |

### 1.3 Inferencias Críticas

1. **Motor de Validación de Habilidades**: Distinción entre validaciones humanas (`ValidatedByUserId`) y automáticas (reglas del agente).
2. **Impacto de Complejidad**: `ComplexityLevel` (1-3) funciona como multiplicador para actualización de niveles en `talent.EmployeeSkills`.
3. **Segregación Multi-tenant**: Roles globales (sistema) y roles personalizados por empresa con aislamiento de datos.

---

## 2. Módulo IAM - Identidad y Gestión de Acceso

> **Esquema SQL:** `iam.*`  
> **Controladores:** `AuthController`, `UsersController`  
> **Tablas:** `Organizations`, `Users`, `Roles`, `UserRoles`

### 2.1 Descripción del Módulo

Constituye el núcleo de seguridad y multi-tenencia del sistema. Garantiza el aislamiento lógico de los datos empresariales y la correcta gestión de identidades, cumpliendo con la **Ley 1581 de 2012 (Habeas Data)**.

### 2.2 Requerimientos Funcionales

#### RF-101: Registro y Gestión de Organizaciones

| Campo | Valor |
|-------|-------|
| **ID** | RF-101 |
| **Nombre** | Registro y Gestión de Organizaciones |
| **Descripción** | El sistema debe proveer una interfaz para Crear, Leer, Actualizar y Desactivar (CRUD) organizaciones en la tabla `iam.Organizations` para habilitar el aislamiento de datos en el entorno multi-tenant. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tabla Relacionada** | `iam.Organizations` |

---

#### RF-102: Gestión de Cuentas de Usuario

| Campo | Valor |
|-------|-------|
| **ID** | RF-102 |
| **Nombre** | Gestión de Cuentas de Usuario |
| **Descripción** | El sistema debe permitir al Administrador de Organización gestionar el ciclo de vida de los usuarios asociados a su `OrganizationId` para controlar el acceso del personal activo a la plataforma. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tabla Relacionada** | `iam.Users` |

---

#### RF-103: Asignación de Roles Jerárquicos

| Campo | Valor |
|-------|-------|
| **ID** | RF-103 |
| **Nombre** | Asignación de Roles Jerárquicos |
| **Descripción** | El sistema debe permitir al Administrador de Organización asignar roles mediante la tabla `iam.UserRoles`, distinguiendo entre roles globales y específicos de la organización, para garantizar que cada usuario acceda únicamente a las funcionalidades permitidas por su cargo. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tablas Relacionadas** | `iam.Roles`, `iam.UserRoles` |

---

#### RF-104: Control de Acceso Basado en Tenencia

| Campo | Valor |
|-------|-------|
| **ID** | RF-104 |
| **Nombre** | Control de Acceso Basado en Tenencia |
| **Descripción** | El sistema debe validar el `OrganizationId` en cada petición de datos para prevenir el acceso no autorizado a información confidencial de otras empresas dentro de la base de datos compartida. |
| **Prioridad** | Crítica |
| **Estado** | ✅ Implementado |
| **Implementación** | Extracción de `OrganizationId` desde JWT claim en cada Controller |

---

#### RF-105: Trazabilidad y Auditoría de Transacciones

| Campo | Valor |
|-------|-------|
| **ID** | RF-105 |
| **Nombre** | Trazabilidad y Auditoría de Transacciones |
| **Descripción** | El sistema debe registrar automáticamente la marca de tiempo (`CreatedAt`) y el identificador del autor (`UpdatedByUserId`) en cada cambio de estado de los esquemas IAM y Talent para generar un historial de auditoría verificable ante incidentes de seguridad. |
| **Prioridad** | Alta |
| **Estado** | ✅ Implementado |
| **Implementación** | Clase base `AuditableEntity` en Domain |

---

## 3. Módulo Talent - Gestión de Talento y Perfiles

> **Esquema SQL:** `talent.*`  
> **Controladores:** `ProfileController`, `SkillsController`, `EmployeeSkillsController`  
> **Tablas:** `EmployeeProfiles`, `Skills`, `EmployeeSkills`, `Certifications`, `SkillEvaluations`

### 3.1 Descripción del Módulo

Permite la transición de un perfil profesional estático a uno dinámico basado en la ejecución verificable de proyectos y certificaciones. Fundamental para la competitividad empresarial.

### 3.2 Requerimientos Funcionales

#### RF-201: Mantenimiento del Perfil Profesional

| Campo | Valor |
|-------|-------|
| **ID** | RF-201 |
| **Nombre** | Mantenimiento del Perfil Profesional |
| **Descripción** | El sistema debe permitir al Empleado gestionar su biografía, años de experiencia y URLs de portafolio (LinkedIn/GitHub) en la tabla `talent.EmployeeProfiles` para visibilizar su trayectoria ante los líderes de proyecto. |
| **Prioridad** | Media |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tabla Relacionada** | `talent.EmployeeProfiles` |

---

#### RF-202: Clasificación de Competencias

| Campo | Valor |
|-------|-------|
| **ID** | RF-202 |
| **Nombre** | Clasificación de Competencias |
| **Descripción** | El sistema debe permitir la gestión de habilidades categorizadas por tipo (Hard, Soft, Language) mediante `talent.Skills` para facilitar la búsqueda y el filtrado técnico de candidatos idóneos. |
| **Prioridad** | Alta |
| **Estado** | ✅ Implementado |
| **Tabla Relacionada** | `talent.Skills` |
| **Campos Clave** | `Category`, `SkillType` |

---

#### RF-203: Calificación Automática de Habilidades

| Campo | Valor |
|-------|-------|
| **ID** | RF-203 |
| **Nombre** | Calificación Automática de Habilidades |
| **Descripción** | El sistema debe actualizar el nivel de dominio del empleado tras el cierre de un proyecto, calculando el incremento basado en el `ComplexityLevel` (1-3) y el `ContributionScore` (1-5) registrado en `projects.ProjectParticipation`, para asegurar que el perfil del trabajador refleje su crecimiento técnico real. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tablas Relacionadas** | `talent.EmployeeSkills`, `talent.SkillEvaluations`, `projects.ProjectParticipation` |

##### Criterios de Aceptación

| ID | Criterio | Detalle Técnico |
|----|----------|-----------------|
| CA-203.1 | Fuente de Validación | El sistema debe registrar en `talent.SkillEvaluations.Source` si el cambio de nivel fue por cierre de proyecto (1), manual (2) o regla de sistema (3). |
| CA-203.2 | Integridad de Niveles | El sistema debe impedir actualizaciones que superen el nivel 5 o sean inferiores a 1, respetando la restricción `CK_EmployeeSkills_Level`. |
| CA-203.3 | Registro del Validador | En validaciones manuales, el sistema debe poblar `ValidatedByUserId` con el ID del Líder de Proyecto; para validaciones automáticas, debe asignar el ID del Agente de Sistema. |

---

#### RF-204: Validación de Evidencias de Certificación

| Campo | Valor |
|-------|-------|
| **ID** | RF-204 |
| **Nombre** | Validación de Evidencias de Certificación |
| **Descripción** | El sistema debe permitir al Empleado registrar certificaciones adjuntando una `EvidenceUrl` en `talent.Certifications` para sustentar documentalmente las competencias declaradas. |
| **Prioridad** | Media |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tabla Relacionada** | `talent.Certifications` |

---

## 4. Módulo Projects - Gestión de Proyectos y Postulaciones

> **Esquema SQL:** `projects.*`  
> **Controladores:** `ProjectsController`, `ProjectApplicationsController`, `AssignmentsController`  
> **Tablas:** `Projects`, `ProjectSkillRequirements`, `ProjectRoles`, `ProjectApplications`, `ProjectAssignments`, `ProjectParticipation`

### 4.1 Descripción del Módulo

Vincula estratégicamente los requisitos de habilidades de un proyecto con el talento real disponible, optimizando la eficiencia operativa y permitiendo el empoderamiento del empleado mediante la postulación activa.

### 4.2 Requerimientos Funcionales

#### RF-301: Definición de Proyectos por Niveles

| Campo | Valor |
|-------|-------|
| **ID** | RF-301 |
| **Nombre** | Definición de Proyectos por Niveles |
| **Descripción** | El sistema debe permitir al Líder de Proyecto crear iniciativas asignando un `ComplexityLevel` entre 1 y 3 para establecer los parámetros de crecimiento y dificultad de la tarea. |
| **Prioridad** | Alta |
| **Estado** | ✅ Implementado |
| **Tabla Relacionada** | `projects.Projects` |
| **Restricción** | `CK_Projects_Complexity CHECK (ComplexityLevel BETWEEN 1 AND 3)` |

---

#### RF-302: Configuración de Requisitos Técnicos

| Campo | Valor |
|-------|-------|
| **ID** | RF-302 |
| **Nombre** | Configuración de Requisitos Técnicos |
| **Descripción** | El sistema debe permitir definir habilidades obligatorias (`IsMandatory`) y niveles mínimos requeridos (`RequiredLevel`) mediante `projects.ProjectSkillRequirements` para filtrar automáticamente a los candidatos que no cumplen con el perfil base. |
| **Prioridad** | Alta |
| **Estado** | ✅ Implementado |
| **Tabla Relacionada** | `projects.ProjectSkillRequirements` |
| **Endpoints** | `POST /api/projects/{id}/reqs`, `GET /api/projects/{id}/reqs` |

---

#### RF-303: Gestión del Ciclo de Vida del Proyecto

| Campo | Valor |
|-------|-------|
| **ID** | RF-303 |
| **Nombre** | Gestión del Ciclo de Vida del Proyecto |
| **Descripción** | El sistema debe controlar la transición de estados del proyecto (1-Draft, 2-Open, 3-InProgress, 4-Closed, 5-Cancelled) según la restricción `CK_Projects_Status` para asegurar la coherencia en los procesos de postulación y feedback. |
| **Prioridad** | Alta |
| **Estado** | ✅ Implementado |
| **Tabla Relacionada** | `projects.Projects` |
| **Enum** | `ProjectStatus` en Domain/Enums |

##### Estados del Proyecto

```
┌─────────┐    Activar    ┌─────────┐    Iniciar    ┌─────────────┐
│  DRAFT  │──────────────▶│  OPEN   │──────────────▶│ IN_PROGRESS │
│   (1)   │               │   (2)   │               │     (3)     │
└─────────┘               └─────────┘               └─────────────┘
     │                         │                          │
     │                         │                          │ Cerrar
     │                         │                          ▼
     │                         │                    ┌──────────┐
     │                         │                    │  CLOSED  │
     │                         │                    │    (4)   │
     │                         │                    └──────────┘
     │                         │
     │      Cancelar (desde cualquier estado)
     └─────────────────────────┴──────────────────────────┐
                                                          ▼
                                                   ┌───────────┐
                                                   │ CANCELLED │
                                                   │    (5)    │
                                                   └───────────┘
```

---

#### RF-304: Postulación Activa y Feedback

| Campo | Valor |
|-------|-------|
| **ID** | RF-304 |
| **Nombre** | Postulación Activa y Feedback |
| **Descripción** | El sistema debe permitir al Empleado postularse a proyectos con estado "Open" (2), exigiendo una nota de motivación, para fomentar la proactividad en el plan de carrera. |
| **Prioridad** | Media |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tabla Relacionada** | `projects.ProjectApplications` |
| **Campo Requerido** | `Motivation` |

---

#### RF-305: Resolución de Postulaciones

| Campo | Valor |
|-------|-------|
| **ID** | RF-305 |
| **Nombre** | Resolución de Postulaciones |
| **Descripción** | El sistema debe permitir al Líder de Proyecto aprobar o rechazar postulaciones, siendo obligatorio el campo `ReviewNotes` en caso de rechazo, para proporcionar al empleado retroalimentación sobre sus brechas de capacitación. |
| **Prioridad** | Media |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tabla Relacionada** | `projects.ProjectApplications` |
| **Campos Clave** | `Status`, `ReviewNotes`, `ReviewedByUserId`, `ReviewedAt` |

##### Estados de Postulación

| Valor | Estado | Descripción |
|-------|--------|-------------|
| 1 | Applied | Postulación enviada, pendiente de revisión |
| 2 | Approved | Postulación aprobada |
| 3 | Rejected | Postulación rechazada (requiere `ReviewNotes`) |
| 4 | Withdrawn | Postulación retirada por el empleado |

---

## 5. Módulo Reporting - Reportes y Agente Inteligente

> **Esquema SQL:** `reporting.*`  
> **Controlador:** `AgentController`  
> **Tablas:** `ReportSnapshots`, `RecommendationRules`, `RecommendationLogs`, `AgentActions`, `AgentConfiguration`

### 5.1 Descripción del Módulo

Actúa como la capa de **Business Intelligence (BI)**, transformando datos transaccionales en decisiones estratégicas de capacitación y planificación de la fuerza laboral mediante un agente de IA basado en **Google Gemini**.

### 5.2 Requerimientos Funcionales

#### RF-401: Generación de Snapshots Históricos

| Campo | Valor |
|-------|-------|
| **ID** | RF-401 |
| **Nombre** | Generación de Snapshots Históricos |
| **Descripción** | El sistema debe capturar el estado de proyectos y competencias en formato JSON dentro de `reporting.ReportSnapshots` de forma periódica para permitir el análisis de tendencias históricas de la organización. |
| **Prioridad** | Media |
| **Estado** | ✅ Implementado |
| **Tabla Relacionada** | `reporting.ReportSnapshots` |
| **Background Service** | `ReportSnapshotGeneratorService` (cada 24 horas) |

---

#### RF-402: Configuración de Reglas de Recomendación

| Campo | Valor |
|-------|-------|
| **ID** | RF-402 |
| **Nombre** | Configuración de Reglas de Recomendación |
| **Descripción** | El sistema debe permitir al Administrador configurar expresiones lógicas en `reporting.RecommendationRules` para definir umbrales críticos de escasez de habilidades o necesidades de capacitación. |
| **Prioridad** | Media |
| **Estado** | ⬜ Pendiente de Revisión |
| **Tablas Relacionadas** | `reporting.RecommendationRules`, `reporting.RecommendationLogs` |

---

#### RF-403: Ejecución del Agente Inteligente

| Campo | Valor |
|-------|-------|
| **ID** | RF-403 |
| **Nombre** | Ejecución del Agente Inteligente |
| **Descripción** | El sistema debe procesar las reglas activas comparando los requisitos de proyectos futuros con el historial de `ProjectParticipation` para sugerir planes de formación específicos a empleados con potencial destacado. |
| **Prioridad** | Alta |
| **Estado** | ✅ Implementado |
| **Implementación** | `AgentService.cs` con patrón Chain-of-Thought + Tool Use |
| **Background Service** | `RecommendationOptimizerService` (cada 6 horas) |

##### Capacidades del Agente IA

| Endpoint | Funcionalidad |
|----------|---------------|
| `POST /agent/query` | Consultas en lenguaje natural sobre talento |
| `POST /agent/validate-skill` | Validación semántica de skills con evidencia |
| `POST /agent/match-candidates` | Matching inteligente de candidatos (score 0-100) |
| `POST /agent/actions/{id}/approve` | Aprobación HITL de acciones críticas |
| `POST /agent/actions/{id}/reject` | Rechazo HITL de acciones críticas |

---

#### RF-404: Visualización de Patrones de Desempeño

| Campo | Valor |
|-------|-------|
| **ID** | RF-404 |
| **Nombre** | Visualización de Patrones de Desempeño |
| **Descripción** | El sistema debe extraer información semántica de `FeedbackComments` mediante el agente inteligente para alertar a la dirección sobre riesgos en la ejecución de proyectos o talentos excepcionales. |
| **Prioridad** | Baja |
| **Estado** | ⬜ Pendiente de Revisión |
| **Campos Analizados** | `projects.ProjectParticipation.FeedbackComments` |

---

## 6. Requerimientos Transversales - Accesibilidad (WCAG 2.2 AA)

> **Aplicación:** Frontend (cuando se implemente)  
> **Estándar:** WCAG 2.2 Nivel AA  
> **Normativas:** ADA Title III, Ley 1618 de 2013 (Colombia)

### 6.1 Descripción

La accesibilidad en DevManager es un imperativo legal y una ventaja competitiva en usabilidad, garantizando que el sistema sea operable por personas con diversas capacidades.

### 6.2 Requerimientos Funcionales

#### RF-501: Contraste y Redimensionamiento

| Campo | Valor |
|-------|-------|
| **ID** | RF-501 |
| **Nombre** | Contraste y Redimensionamiento |
| **Descripción** | El sistema debe garantizar un contraste de color mínimo de 4.5:1 para texto normal y permitir el redimensionamiento de la fuente hasta un 200% sin pérdida de contenido para cumplir con el nivel AA de percepción visual. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente (Frontend) |
| **Criterio WCAG** | 1.4.3, 1.4.4 |

---

#### RF-502: Navegación Estructural Eficiente

| Campo | Valor |
|-------|-------|
| **ID** | RF-502 |
| **Nombre** | Navegación Estructural Eficiente |
| **Descripción** | El sistema debe incluir un enlace de "Saltar al contenido principal" (Skip to Main Content) y asegurar que el orden de foco del teclado sea lógico en cuadros de mando complejos para facilitar la operabilidad a usuarios con discapacidades motoras. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente (Frontend) |
| **Criterio WCAG** | 2.4.1, 2.4.3 |

---

#### RF-503: Identificación y Prevención de Errores

| Campo | Valor |
|-------|-------|
| **ID** | RF-503 |
| **Nombre** | Identificación y Prevención de Errores |
| **Descripción** | El sistema debe identificar errores de entrada de forma textual y sugerir correcciones en los formularios de postulación para reducir la carga cognitiva y mejorar la accesibilidad a tecnologías asistivas. |
| **Prioridad** | Media |
| **Estado** | ⬜ Pendiente (Frontend) |
| **Criterio WCAG** | 3.3.1, 3.3.3 |

---

#### RF-504: Compatibilidad con Tecnologías Asistivas

| Campo | Valor |
|-------|-------|
| **ID** | RF-504 |
| **Nombre** | Compatibilidad con Tecnologías Asistivas |
| **Descripción** | El sistema debe asegurar que todos los elementos interactivos y estados dinámicos sean anunciados correctamente por lectores de pantalla (JAWS, NVDA, VoiceOver) mediante el uso correcto de atributos ARIA, para garantizar la inclusión total en el entorno laboral. |
| **Prioridad** | Alta |
| **Estado** | ⬜ Pendiente (Frontend) |
| **Criterio WCAG** | 4.1.2 |

### 6.3 Verificación de Calidad (Regla 30/70)

| Tipo | Porcentaje | Herramientas/Método |
|------|------------|---------------------|
| Automática | 30% | axe-core, WAVE, Lighthouse |
| Manual | 70% | Pruebas con lectores de pantalla, navegación por teclado, detección de Keyboard Traps |

---

## 7. Matriz de Trazabilidad

### 7.1 Resumen por Módulo

| Módulo | Total RFs | Implementados | Pendientes |
|--------|-----------|---------------|------------|
| IAM | 5 | 2 | 3 |
| Talent | 4 | 1 | 3 |
| Projects | 5 | 3 | 2 |
| Reporting | 4 | 2 | 2 |
| Accesibilidad | 4 | 0 | 4 |
| **TOTAL** | **22** | **8** | **14** |

### 7.2 Mapeo RF → Tablas SQL

| RF ID | Tablas Afectadas |
|-------|------------------|
| RF-101 | `iam.Organizations` |
| RF-102 | `iam.Users` |
| RF-103 | `iam.Roles`, `iam.UserRoles` |
| RF-104 | Todas (filtro por `OrganizationId`) |
| RF-105 | Todas (campos `AuditableEntity`) |
| RF-201 | `talent.EmployeeProfiles` |
| RF-202 | `talent.Skills` |
| RF-203 | `talent.EmployeeSkills`, `talent.SkillEvaluations`, `projects.ProjectParticipation` |
| RF-204 | `talent.Certifications` |
| RF-301 | `projects.Projects` |
| RF-302 | `projects.ProjectSkillRequirements` |
| RF-303 | `projects.Projects` |
| RF-304 | `projects.ProjectApplications` |
| RF-305 | `projects.ProjectApplications` |
| RF-401 | `reporting.ReportSnapshots` |
| RF-402 | `reporting.RecommendationRules`, `reporting.RecommendationLogs` |
| RF-403 | `reporting.AgentActions`, `reporting.AgentConfiguration` |
| RF-404 | `projects.ProjectParticipation` |

### 7.3 Mapeo RF → Controladores API

| RF ID | Controller | Endpoints |
|-------|------------|-----------|
| RF-102 | `UsersController` | `GET/POST/PUT /api/users` |
| RF-104 | Todos | Validación JWT claim `OrganizationId` |
| RF-201 | `ProfileController` | `GET/PUT /api/profile` |
| RF-202 | `SkillsController` | `GET/POST /api/skills` |
| RF-203 | `EmployeeSkillsController` | `PUT /api/employee-skills/{id}` |
| RF-301 | `ProjectsController` | `POST /api/projects` |
| RF-302 | `ProjectsController` | `POST/GET /api/projects/{id}/reqs` |
| RF-303 | `ProjectsController` | `PUT /api/projects/{id}` |
| RF-304 | `ProjectApplicationsController` | `POST /api/applications` |
| RF-305 | `ProjectApplicationsController` | `PUT /api/applications/{id}` |
| RF-403 | `AgentController` | `POST /agent/*` |

---

## 📝 Registro de Cambios

| Fecha | Versión | Autor | Descripción |
|-------|---------|-------|-------------|
| 2026-02-02 | 1.0 | - | Creación inicial del documento |

---

## 🔗 Referencias

- [AGENT_GUIDE.md](../AGENT_GUIDE.md) - Guía del Agente IA
- [DDL_Dev_Manager.sql](../Infrastructure/Database/DDL/DDL_Dev_Manager.sql) - Esquema de Base de Datos
- [DDL_Agent_Tables.sql](../Infrastructure/Database/DDL/DDL_Agent_Tables.sql) - Tablas del Agente
- [PROJECTS_IMPLEMENTATION_GUIDE.md](./PROJECTS_IMPLEMENTATION_GUIDE.md) - Guía de Implementación de Proyectos

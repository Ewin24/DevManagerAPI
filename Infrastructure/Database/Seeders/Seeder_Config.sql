-- =========================================================
-- DevManager - Seeder de Tablas de Configuración
-- Versión: 1.0
-- Fecha: 9 de Febrero de 2026
-- Idempotente: Verifica existencia antes de insertar
-- =========================================================

USE DevManager;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '==========================================';
PRINT 'DevManager - Seeder de Catálogos';
PRINT '==========================================';
PRINT '';

-- =============================================
-- 1. Estados de Proyecto
-- =============================================
PRINT '1. Poblando config.ProjectStatuses...';

IF NOT EXISTS (SELECT 1 FROM config.ProjectStatuses WHERE Id = 1)
    INSERT INTO config.ProjectStatuses (Id, Code, Name, Description, DisplayOrder, AllowsApplications)
    VALUES (1, 'DRAFT', 'Borrador', 'Proyecto en fase de definición, no visible para postulaciones', 1, 0);

IF NOT EXISTS (SELECT 1 FROM config.ProjectStatuses WHERE Id = 2)
    INSERT INTO config.ProjectStatuses (Id, Code, Name, Description, DisplayOrder, AllowsApplications)
    VALUES (2, 'OPEN', 'Abierto', 'Proyecto publicado y aceptando postulaciones de candidatos', 2, 1);

IF NOT EXISTS (SELECT 1 FROM config.ProjectStatuses WHERE Id = 3)
    INSERT INTO config.ProjectStatuses (Id, Code, Name, Description, DisplayOrder, AllowsApplications)
    VALUES (3, 'IN_PROGRESS', 'En Progreso', 'Proyecto en ejecución activa con equipo asignado', 3, 0);

IF NOT EXISTS (SELECT 1 FROM config.ProjectStatuses WHERE Id = 4)
    INSERT INTO config.ProjectStatuses (Id, Code, Name, Description, DisplayOrder, AllowsApplications)
    VALUES (4, 'CLOSED', 'Cerrado', 'Proyecto completado exitosamente', 4, 0);

IF NOT EXISTS (SELECT 1 FROM config.ProjectStatuses WHERE Id = 5)
    INSERT INTO config.ProjectStatuses (Id, Code, Name, Description, DisplayOrder, AllowsApplications)
    VALUES (5, 'CANCELLED', 'Cancelado', 'Proyecto cancelado antes de completarse', 5, 0);

PRINT '  ✓ 5 estados de proyecto';

-- =============================================
-- 2. Niveles de Complejidad
-- =============================================
PRINT '2. Poblando config.ProjectComplexityLevels...';

IF NOT EXISTS (SELECT 1 FROM config.ProjectComplexityLevels WHERE Id = 1)
    INSERT INTO config.ProjectComplexityLevels (Id, Code, Name, Description, ExperienceMultiplier, DisplayOrder)
    VALUES (1, 'LOW', 'Baja', 'Proyectos de mantenimiento o mejoras menores. Tecnologías conocidas.', 0.75, 1);

IF NOT EXISTS (SELECT 1 FROM config.ProjectComplexityLevels WHERE Id = 2)
    INSERT INTO config.ProjectComplexityLevels (Id, Code, Name, Description, ExperienceMultiplier, DisplayOrder)
    VALUES (2, 'MEDIUM', 'Media', 'Desarrollo de nuevas funcionalidades con arquitectura estándar.', 1.00, 2);

IF NOT EXISTS (SELECT 1 FROM config.ProjectComplexityLevels WHERE Id = 3)
    INSERT INTO config.ProjectComplexityLevels (Id, Code, Name, Description, ExperienceMultiplier, DisplayOrder)
    VALUES (3, 'HIGH', 'Alta', 'Proyectos de alta complejidad: microservicios, IA, sistemas distribuidos.', 1.50, 3);

PRINT '  ✓ 3 niveles de complejidad';

-- =============================================
-- 3. Estados de Postulación
-- =============================================
PRINT '3. Poblando config.ApplicationStatuses...';

IF NOT EXISTS (SELECT 1 FROM config.ApplicationStatuses WHERE Id = 1)
    INSERT INTO config.ApplicationStatuses (Id, Code, Name, Description, RequiresReviewNotes, IsFinalState, DisplayOrder)
    VALUES (1, 'APPLIED', 'Postulado', 'Postulación enviada, pendiente de revisión por el líder', 0, 0, 1);

IF NOT EXISTS (SELECT 1 FROM config.ApplicationStatuses WHERE Id = 2)
    INSERT INTO config.ApplicationStatuses (Id, Code, Name, Description, RequiresReviewNotes, IsFinalState, DisplayOrder)
    VALUES (2, 'APPROVED', 'Aprobado', 'Postulación aprobada, candidato elegible para asignación', 0, 1, 2);

IF NOT EXISTS (SELECT 1 FROM config.ApplicationStatuses WHERE Id = 3)
    INSERT INTO config.ApplicationStatuses (Id, Code, Name, Description, RequiresReviewNotes, IsFinalState, DisplayOrder)
    VALUES (3, 'REJECTED', 'Rechazado', 'Postulación rechazada. Se requiere feedback obligatorio.', 1, 1, 3);

IF NOT EXISTS (SELECT 1 FROM config.ApplicationStatuses WHERE Id = 4)
    INSERT INTO config.ApplicationStatuses (Id, Code, Name, Description, RequiresReviewNotes, IsFinalState, DisplayOrder)
    VALUES (4, 'WITHDRAWN', 'Retirado', 'Postulación retirada voluntariamente por el candidato', 0, 1, 4);

PRINT '  ✓ 4 estados de postulación';

-- =============================================
-- 4. Estados de Asignación
-- =============================================
PRINT '4. Poblando config.AssignmentStatuses...';

IF NOT EXISTS (SELECT 1 FROM config.AssignmentStatuses WHERE Id = 1)
    INSERT INTO config.AssignmentStatuses (Id, Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES (1, 'ACTIVE', 'Activo', 'Empleado actualmente asignado y trabajando en el proyecto', 0, 1);

IF NOT EXISTS (SELECT 1 FROM config.AssignmentStatuses WHERE Id = 2)
    INSERT INTO config.AssignmentStatuses (Id, Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES (2, 'REMOVED', 'Removido', 'Empleado removido del proyecto antes de completarlo', 1, 2);

IF NOT EXISTS (SELECT 1 FROM config.AssignmentStatuses WHERE Id = 3)
    INSERT INTO config.AssignmentStatuses (Id, Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES (3, 'COMPLETED', 'Completado', 'Participación completada exitosamente', 1, 3);

PRINT '  ✓ 3 estados de asignación';

-- =============================================
-- 5. Niveles de Dominio de Habilidades
-- =============================================
PRINT '5. Poblando config.SkillLevels...';

IF NOT EXISTS (SELECT 1 FROM config.SkillLevels WHERE Id = 1)
    INSERT INTO config.SkillLevels (Id, Code, Name, Description, MinYearsExperience, DisplayOrder)
    VALUES (1, 'NOVICE', 'Novato', 'Conocimientos básicos teóricos. Requiere supervisión constante.', 0, 1);

IF NOT EXISTS (SELECT 1 FROM config.SkillLevels WHERE Id = 2)
    INSERT INTO config.SkillLevels (Id, Code, Name, Description, MinYearsExperience, DisplayOrder)
    VALUES (2, 'BEGINNER', 'Principiante', 'Puede realizar tareas simples con guía. Experiencia limitada en proyectos reales.', 1, 2);

IF NOT EXISTS (SELECT 1 FROM config.SkillLevels WHERE Id = 3)
    INSERT INTO config.SkillLevels (Id, Code, Name, Description, MinYearsExperience, DisplayOrder)
    VALUES (3, 'INTERMEDIATE', 'Intermedio', 'Trabaja de forma autónoma en tareas estándar. Resuelve problemas comunes.', 2, 3);

IF NOT EXISTS (SELECT 1 FROM config.SkillLevels WHERE Id = 4)
    INSERT INTO config.SkillLevels (Id, Code, Name, Description, MinYearsExperience, DisplayOrder)
    VALUES (4, 'ADVANCED', 'Avanzado', 'Dominio sólido. Mentoriza a otros y diseña soluciones complejas.', 4, 4);

IF NOT EXISTS (SELECT 1 FROM config.SkillLevels WHERE Id = 5)
    INSERT INTO config.SkillLevels (Id, Code, Name, Description, MinYearsExperience, DisplayOrder)
    VALUES (5, 'EXPERT', 'Experto', 'Referente técnico. Define estándares y arquitecturas. Contribuye a la comunidad.', 6, 5);

PRINT '  ✓ 5 niveles de dominio';

-- =============================================
-- 6. Tipos de Habilidades
-- =============================================
PRINT '6. Poblando config.SkillTypes...';

IF NOT EXISTS (SELECT 1 FROM config.SkillTypes WHERE Code = 'HARD')
    INSERT INTO config.SkillTypes (Code, Name, Description, DisplayOrder)
    VALUES ('HARD', 'Técnica', 'Habilidades técnicas medibles: lenguajes, frameworks, herramientas', 1);

IF NOT EXISTS (SELECT 1 FROM config.SkillTypes WHERE Code = 'SOFT')
    INSERT INTO config.SkillTypes (Code, Name, Description, DisplayOrder)
    VALUES ('SOFT', 'Blanda', 'Habilidades interpersonales: liderazgo, comunicación, trabajo en equipo', 2);

IF NOT EXISTS (SELECT 1 FROM config.SkillTypes WHERE Code = 'LANGUAGE')
    INSERT INTO config.SkillTypes (Code, Name, Description, DisplayOrder)
    VALUES ('LANGUAGE', 'Idioma', 'Competencias lingüísticas: inglés, español, portugués, etc.', 3);

IF NOT EXISTS (SELECT 1 FROM config.SkillTypes WHERE Code = 'CERTIFICATION')
    INSERT INTO config.SkillTypes (Code, Name, Description, DisplayOrder)
    VALUES ('CERTIFICATION', 'Certificación', 'Certificaciones profesionales verificables', 4);

PRINT '  ✓ 4 tipos de habilidades';

-- =============================================
-- 7. Categorías de Habilidades
-- =============================================
PRINT '7. Poblando config.SkillCategories...';

DECLARE @CategoryCounter INT = 0;

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'PROGRAMMING')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('PROGRAMMING', 'Lenguajes de Programación', 'Lenguajes de desarrollo de software', NULL, 1);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'DATABASE')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('DATABASE', 'Bases de Datos', 'Sistemas de gestión de bases de datos', NULL, 2);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'CLOUD')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('CLOUD', 'Cloud Computing', 'Plataformas y servicios en la nube', NULL, 3);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'FRAMEWORK')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('FRAMEWORK', 'Frameworks', 'Frameworks y librerías de desarrollo', NULL, 4);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'DEVOPS')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('DEVOPS', 'DevOps', 'Herramientas de CI/CD, contenedores y automatización', NULL, 5);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'ARCHITECTURE')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('ARCHITECTURE', 'Arquitectura', 'Patrones y estilos arquitectónicos', NULL, 6);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'TESTING')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('TESTING', 'Testing', 'Pruebas de software y QA', NULL, 7);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'SECURITY')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('SECURITY', 'Seguridad', 'Seguridad informática y ciberseguridad', NULL, 8);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'MOBILE')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('MOBILE', 'Desarrollo Móvil', 'Desarrollo de aplicaciones móviles', NULL, 9);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'AI_ML')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('AI_ML', 'IA y Machine Learning', 'Inteligencia artificial y aprendizaje automático', NULL, 10);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'INTERPERSONAL')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('INTERPERSONAL', 'Interpersonal', 'Habilidades de comunicación y trabajo en equipo', NULL, 11);

IF NOT EXISTS (SELECT 1 FROM config.SkillCategories WHERE Code = 'MANAGEMENT')
    INSERT INTO config.SkillCategories (Code, Name, Description, ParentCategoryId, DisplayOrder)
    VALUES ('MANAGEMENT', 'Gestión', 'Gestión de proyectos y equipos', NULL, 12);

PRINT '  ✓ 12 categorías de habilidades';

-- =============================================
-- 8. Fuentes de Evaluación
-- =============================================
PRINT '8. Poblando config.EvaluationSources...';

IF NOT EXISTS (SELECT 1 FROM config.EvaluationSources WHERE Id = 1)
    INSERT INTO config.EvaluationSources (Id, Code, Name, Description, IsAutomated, DisplayOrder)
    VALUES (1, 'PROJECT', 'Proyecto', 'Evaluación derivada del cierre de un proyecto', 1, 1);

IF NOT EXISTS (SELECT 1 FROM config.EvaluationSources WHERE Id = 2)
    INSERT INTO config.EvaluationSources (Id, Code, Name, Description, IsAutomated, DisplayOrder)
    VALUES (2, 'MANUAL', 'Manual', 'Evaluación realizada manualmente por un líder o mentor', 0, 2);

IF NOT EXISTS (SELECT 1 FROM config.EvaluationSources WHERE Id = 3)
    INSERT INTO config.EvaluationSources (Id, Code, Name, Description, IsAutomated, DisplayOrder)
    VALUES (3, 'SYSTEM_RULE', 'Regla del Sistema', 'Evaluación automática basada en reglas del agente IA', 1, 3);

IF NOT EXISTS (SELECT 1 FROM config.EvaluationSources WHERE Id = 4)
    INSERT INTO config.EvaluationSources (Id, Code, Name, Description, IsAutomated, DisplayOrder)
    VALUES (4, 'CERTIFICATION', 'Certificación', 'Evaluación derivada de una certificación verificada', 0, 4);

IF NOT EXISTS (SELECT 1 FROM config.EvaluationSources WHERE Id = 5)
    INSERT INTO config.EvaluationSources (Id, Code, Name, Description, IsAutomated, DisplayOrder)
    VALUES (5, 'SELF_ASSESSMENT', 'Autoevaluación', 'Evaluación declarada por el propio empleado (requiere validación)', 0, 5);

PRINT '  ✓ 5 fuentes de evaluación';

-- =============================================
-- 9. Puntajes de Contribución
-- =============================================
PRINT '9. Poblando config.ContributionScores...';

IF NOT EXISTS (SELECT 1 FROM config.ContributionScores WHERE Id = 1)
    INSERT INTO config.ContributionScores (Id, Code, Name, Description, ExperienceBonus, DisplayOrder)
    VALUES (1, 'MINIMAL', 'Mínima', 'Contribución por debajo de lo esperado. Requiere mejora significativa.', 0.50, 1);

IF NOT EXISTS (SELECT 1 FROM config.ContributionScores WHERE Id = 2)
    INSERT INTO config.ContributionScores (Id, Code, Name, Description, ExperienceBonus, DisplayOrder)
    VALUES (2, 'PARTIAL', 'Parcial', 'Contribución limitada. Cumplió algunas expectativas.', 0.75, 2);

IF NOT EXISTS (SELECT 1 FROM config.ContributionScores WHERE Id = 3)
    INSERT INTO config.ContributionScores (Id, Code, Name, Description, ExperienceBonus, DisplayOrder)
    VALUES (3, 'EXPECTED', 'Esperada', 'Contribución satisfactoria. Cumplió con las expectativas del rol.', 1.00, 3);

IF NOT EXISTS (SELECT 1 FROM config.ContributionScores WHERE Id = 4)
    INSERT INTO config.ContributionScores (Id, Code, Name, Description, ExperienceBonus, DisplayOrder)
    VALUES (4, 'ABOVE', 'Superior', 'Contribución destacada. Superó expectativas en áreas clave.', 1.25, 4);

IF NOT EXISTS (SELECT 1 FROM config.ContributionScores WHERE Id = 5)
    INSERT INTO config.ContributionScores (Id, Code, Name, Description, ExperienceBonus, DisplayOrder)
    VALUES (5, 'EXCEPTIONAL', 'Excepcional', 'Contribución sobresaliente. Impacto significativo en el éxito del proyecto.', 1.50, 5);

PRINT '  ✓ 5 puntajes de contribución';

-- =============================================
-- 10. Tipos de Acciones del Agente
-- =============================================
PRINT '10. Poblando config.AgentActionTypes...';

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'SKILL_VALIDATION')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('SKILL_VALIDATION', 'Validación de Skill', 'Validación semántica de habilidades con análisis de evidencia', 1, 1);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'PROJECT_MATCHING')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('PROJECT_MATCHING', 'Matching de Proyecto', 'Emparejamiento de candidatos con proyectos', 0, 2);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'SKILL_UPGRADE')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('SKILL_UPGRADE', 'Upgrade de Skill', 'Propuesta de incremento de nivel de habilidad', 1, 3);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'TRAINING_RECOMMENDATION')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('TRAINING_RECOMMENDATION', 'Recomendación de Capacitación', 'Sugerencia de plan de formación', 0, 4);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'GENERAL_QUERY')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('GENERAL_QUERY', 'Consulta General', 'Consulta en lenguaje natural sobre el sistema', 0, 5);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'RISK_ALERT')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('RISK_ALERT', 'Alerta de Riesgo', 'Detección de riesgos en proyectos o talento', 0, 6);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionTypes WHERE Code = 'SNAPSHOT_GENERATION')
    INSERT INTO config.AgentActionTypes (Code, Name, Description, RequiresApproval, DisplayOrder)
    VALUES ('SNAPSHOT_GENERATION', 'Generación de Snapshot', 'Captura periódica del estado del sistema', 0, 7);

PRINT '  ✓ 7 tipos de acciones del agente';

-- =============================================
-- 11. Estados de Acciones del Agente
-- =============================================
PRINT '11. Poblando config.AgentActionStatuses...';

IF NOT EXISTS (SELECT 1 FROM config.AgentActionStatuses WHERE Code = 'SUCCESS')
    INSERT INTO config.AgentActionStatuses (Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES ('SUCCESS', 'Exitoso', 'Acción completada exitosamente', 1, 1);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionStatuses WHERE Code = 'FAILED')
    INSERT INTO config.AgentActionStatuses (Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES ('FAILED', 'Fallido', 'Acción falló durante la ejecución', 1, 2);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionStatuses WHERE Code = 'PENDING_APPROVAL')
    INSERT INTO config.AgentActionStatuses (Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES ('PENDING_APPROVAL', 'Pendiente de Aprobación', 'Acción requiere aprobación humana (HITL)', 0, 3);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionStatuses WHERE Code = 'APPROVED')
    INSERT INTO config.AgentActionStatuses (Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES ('APPROVED', 'Aprobado', 'Acción aprobada por un usuario humano', 1, 4);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionStatuses WHERE Code = 'REJECTED')
    INSERT INTO config.AgentActionStatuses (Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES ('REJECTED', 'Rechazado', 'Acción rechazada por un usuario humano', 1, 5);

IF NOT EXISTS (SELECT 1 FROM config.AgentActionStatuses WHERE Code = 'PROCESSING')
    INSERT INTO config.AgentActionStatuses (Code, Name, Description, IsFinalState, DisplayOrder)
    VALUES ('PROCESSING', 'Procesando', 'Acción en proceso de ejecución', 0, 6);

PRINT '  ✓ 6 estados de acciones del agente';

-- =============================================
-- 12. Niveles de Seniority
-- =============================================
PRINT '12. Poblando config.SeniorityLevels...';

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'INTERN')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('INTERN', 'Practicante', 'Estudiante o recién graduado en formación', 0, 1, 1);

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'JUNIOR')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('JUNIOR', 'Junior', 'Profesional con experiencia inicial. Requiere mentoría.', 1, 2, 2);

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'MID')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('MID', 'Mid-Level', 'Profesional autónomo con experiencia sólida.', 2, 5, 3);

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'SENIOR')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('SENIOR', 'Senior', 'Experto técnico. Mentoriza y lidera iniciativas.', 5, 8, 4);

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'LEAD')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('LEAD', 'Tech Lead', 'Líder técnico de equipos. Define estándares y arquitectura.', 7, 12, 5);

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'PRINCIPAL')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('PRINCIPAL', 'Principal', 'Referente organizacional. Influye en la estrategia técnica.', 10, NULL, 6);

IF NOT EXISTS (SELECT 1 FROM config.SeniorityLevels WHERE Code = 'ARCHITECT')
    INSERT INTO config.SeniorityLevels (Code, Name, Description, MinYearsExperience, MaxYearsExperience, DisplayOrder)
    VALUES ('ARCHITECT', 'Arquitecto', 'Define visión técnica a nivel empresarial.', 12, NULL, 7);

PRINT '  ✓ 7 niveles de seniority';

-- =============================================
-- Resumen Final
-- =============================================
PRINT '';
PRINT '==========================================';
PRINT 'SEEDING DE CATÁLOGOS COMPLETADO';
PRINT '==========================================';
PRINT '✓ 5 Estados de Proyecto';
PRINT '✓ 3 Niveles de Complejidad';
PRINT '✓ 4 Estados de Postulación';
PRINT '✓ 3 Estados de Asignación';
PRINT '✓ 5 Niveles de Dominio de Habilidades';
PRINT '✓ 4 Tipos de Habilidades';
PRINT '✓ 12 Categorías de Habilidades';
PRINT '✓ 5 Fuentes de Evaluación';
PRINT '✓ 5 Puntajes de Contribución';
PRINT '✓ 7 Tipos de Acciones del Agente';
PRINT '✓ 6 Estados de Acciones del Agente';
PRINT '✓ 7 Niveles de Seniority';
PRINT '';
PRINT 'TOTAL: 66 registros de configuración';
PRINT '==========================================';
GO

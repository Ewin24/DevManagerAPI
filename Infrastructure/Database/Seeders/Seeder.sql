-- =========================================================
-- DevManager - Seeder de Tablas de Configuración
-- Versión: 1.0
-- Fecha: 9 de Febrero de 2026
-- Idempotente: Verifica existencia antes de insertar
-- =========================================================

USE DevManager;


SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;


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

-- =============================================
-- DevManager Database Seeder - COMPLETO
-- Basado en DDL_Dev_Manager.sql
-- Password: "Password123!"
-- 
-- INCLUYE:
-- - Datos base del sistema (usuarios, roles, skills básicas)
-- - Datos de prueba para el Agente IA
-- =============================================

USE DevManager;


SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;


PRINT '==========================================';
PRINT 'DevManager - Seeder Completo + Datos Agente IA';
PRINT '==========================================';
PRINT '';

-- =============================================
-- 1. IAM: Organizaciones
-- =============================================
PRINT '1. Insertando Organizaciones...';

INSERT INTO [iam].[Organizations] (Id, Name, LegalName, Nit, IsActive, CreatedAt, IsDeleted)
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'TechCorp Solutions', 'TechCorp Solutions S.A.S.', '900123456-7', 1, DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    ('22222222-2222-2222-2222-222222222222', 'InnovateLab', 'InnovateLab Colombia Ltda.', '900987654-3', 1, DATEADD(MONTH, -3, SYSUTCDATETIME()), 0);

PRINT '  ✓ 2 organizaciones insertadas';

-- =============================================
-- 2. IAM: Roles
-- =============================================
PRINT '2. Insertando Roles...';

INSERT INTO [iam].[Roles] (Id, OrganizationId, Name, Description, CreatedAt, IsDeleted)
VALUES 
    ('AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA', NULL, 'Admin', 'Administrador del sistema con acceso completo', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    ('BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB', NULL, 'Manager', 'Gerente de proyectos y equipos', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    ('CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC', NULL, 'Developer', 'Desarrollador de software', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0);

PRINT '  ✓ 3 roles insertados';

-- =============================================
-- 3. IAM: Usuarios
-- Password para todos: "Password123!"
-- Hash generado con: cd PasswordHashGenerator && dotnet run
-- =============================================
PRINT '3. Insertando Usuarios...';

DECLARE @PasswordSalt VARBINARY(512) = 0x73BBB749E8FADB426B239CCCCAC1E1E34D972F1153C8F9ED075746CD22F8A7A2E93B014F23B50C09F22A67223888CC145BA8B242EFF90B6DE40114E96003E52E64B17AC022084DEA2DD4958E74BCB4A59A8E0132FD2B8F4B6366701F8B21397BDB9C19376037D5D197CE43377648AF219BEEFDE214E1FFE0EBD0B1B55F2838F2;
DECLARE @PasswordHash VARBINARY(512) = 0x3E4657E2784DE998AAABD30A65D3581085979A1967FB0349148C005EE4EBA5F8ABB2CECF645DB5A9063463F89E97B7C7120EBB41A916B904FCC98F790F2D16D0;

INSERT INTO [iam].[Users] (Id, OrganizationId, Email, FirstName, LastName, PasswordHash, PasswordSalt, IsActive, CreatedAt, IsDeleted)
VALUES 
    ('11111111-0000-0000-0000-000000000001', '11111111-1111-1111-1111-111111111111', 'admin@techcorp.com', 'Carlos', 'Rodriguez', @PasswordHash, @PasswordSalt, 1, DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    ('11111111-0000-0000-0000-000000000002', '11111111-1111-1111-1111-111111111111', 'maria.garcia@techcorp.com', 'María', 'García', @PasswordHash, @PasswordSalt, 1, DATEADD(MONTH, -5, SYSUTCDATETIME()), 0),
    ('11111111-0000-0000-0000-000000000003', '11111111-1111-1111-1111-111111111111', 'juan.martinez@techcorp.com', 'Juan', 'Martínez', @PasswordHash, @PasswordSalt, 1, DATEADD(MONTH, -4, SYSUTCDATETIME()), 0),
    ('11111111-0000-0000-0000-000000000004', '11111111-1111-1111-1111-111111111111', 'ana.lopez@techcorp.com', 'Ana', 'López', @PasswordHash, @PasswordSalt, 1, DATEADD(MONTH, -3, SYSUTCDATETIME()), 0),
    ('22222222-0000-0000-0000-000000000001', '22222222-2222-2222-2222-222222222222', 'admin@innovatelab.com', 'Pedro', 'Ramírez', @PasswordHash, @PasswordSalt, 1, DATEADD(MONTH, -3, SYSUTCDATETIME()), 0);

PRINT '   ✓ 5 usuarios';

-- =============================================
-- 4. IAM: UserRoles
-- =============================================
PRINT '4. Asignando Roles...';

INSERT INTO [iam].[UserRoles] (UserId, RoleId, OrganizationId)
VALUES 
    ('11111111-0000-0000-0000-000000000001', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA', '11111111-1111-1111-1111-111111111111'), -- Carlos: Admin
    ('11111111-0000-0000-0000-000000000001', 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB', '11111111-1111-1111-1111-111111111111'), -- Carlos: Manager
    ('11111111-0000-0000-0000-000000000002', 'BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB', '11111111-1111-1111-1111-111111111111'), -- María: Manager
    ('11111111-0000-0000-0000-000000000003', 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC', '11111111-1111-1111-1111-111111111111'), -- Juan: Developer
    ('11111111-0000-0000-0000-000000000004', 'CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC', '11111111-1111-1111-1111-111111111111'), -- Ana: Developer
    ('22222222-0000-0000-0000-000000000001', 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA', '22222222-2222-2222-2222-222222222222'); -- Pedro: Admin

PRINT '   ✓ 6 asignaciones de roles';

-- =============================================
-- 5. TALENT: Skills (Catálogo)
-- =============================================
PRINT '5. Insertando Skills...';

-- Skills globales (OrganizationId = NULL)
INSERT INTO [talent].[Skills] (Id, OrganizationId, Name, Category, SkillType, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), NULL, 'C#', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'JavaScript', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Python', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Java', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'TypeScript', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'SQL Server', 'Database', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'PostgreSQL', 'Database', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Azure', 'Cloud', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'AWS', 'Cloud', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, '.NET Core', 'Framework', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Spring Boot', 'Framework', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'React', 'Framework', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Angular', 'Framework', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Docker', 'DevOps', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Kubernetes', 'DevOps', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'CI/CD', 'DevOps', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Microservicios', 'Architecture', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'REST APIs', 'Architecture', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Liderazgo', 'Management', 'Soft', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Comunicación', 'Interpersonal', 'Soft', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Trabajo en Equipo', 'Interpersonal', 'Soft', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0);

-- Skills específicas de organización
INSERT INTO [talent].[Skills] (Id, OrganizationId, Name, Category, SkillType, CreatedAt, IsDeleted)
VALUES
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'Metodología TechCorp', 'Process', 'Hard', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0);

PRINT '  ✓ 22 skills insertadas (21 globales + 1 organizacional)';

-- =============================================
-- 6. TALENT: Employee Profiles
-- =============================================
PRINT '6. Insertando Employee Profiles...';

INSERT INTO [talent].[EmployeeProfiles] (UserId, OrganizationId, Bio, YearsExperience, LinkedInUrl, PortfolioUrl, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId, IsDeleted)
VALUES 
    ('11111111-0000-0000-0000-000000000002', '11111111-1111-1111-1111-111111111111', 
     'Ingeniera de software con 10 años de experiencia liderando equipos de desarrollo.', 
     10, 'https://linkedin.com/in/mariagarcia', 'https://mariagarcia.dev', DATEADD(MONTH, -5, SYSUTCDATETIME()), NULL, NULL, NULL, 0),
    ('11111111-0000-0000-0000-000000000003', '11111111-1111-1111-1111-111111111111', 
     'Desarrollador full stack especializado en .NET y React con 8 años de experiencia.', 
     8, 'https://linkedin.com/in/juanmartinez', 'https://github.com/juandev', DATEADD(MONTH, -4, SYSUTCDATETIME()), NULL, NULL, NULL, 0),
    ('11111111-0000-0000-0000-000000000004', '11111111-1111-1111-1111-111111111111', 
     'Desarrolladora backend con experiencia en arquitecturas de microservicios y cloud.', 
     5, NULL, 'https://github.com/analopez', DATEADD(MONTH, -3, SYSUTCDATETIME()), NULL, NULL, NULL, 0);

PRINT '  ✓ 3 perfiles de empleados insertados';

-- =============================================
-- 7. TALENT: Employee Skills
-- =============================================
PRINT '7. Insertando Employee Skills...';

DECLARE @CsharpId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'C#' AND OrganizationId IS NULL);
DECLARE @JSId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'JavaScript' AND OrganizationId IS NULL);
DECLARE @ReactId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'React' AND OrganizationId IS NULL);
DECLARE @AzureId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Azure' AND OrganizationId IS NULL);
DECLARE @LiderazgoId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Liderazgo' AND OrganizationId IS NULL);
DECLARE @JavaId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Java' AND OrganizationId IS NULL);
DECLARE @SpringBootId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Spring Boot' AND OrganizationId IS NULL);
DECLARE @PostgreSQLId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'PostgreSQL' AND OrganizationId IS NULL);

-- María García (Manager con skills técnicas)
INSERT INTO [talent].[EmployeeSkills] (Id, OrganizationId, UserId, SkillId, Level, EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000002', @CsharpId, 4, 
     'https://github.com/maria/csharp-projects', DATEADD(MONTH, -1, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000001', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000002', @LiderazgoId, 5, 
     NULL, DATEADD(MONTH, -2, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000001', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000002', @AzureId, 4, 
     NULL, DATEADD(MONTH, -1, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000001', DATEADD(MONTH, -4, SYSUTCDATETIME()), 0);

-- Juan Martínez (Full Stack Developer)
INSERT INTO [talent].[EmployeeSkills] (Id, OrganizationId, UserId, SkillId, Level, EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @CsharpId, 5, 
     'https://github.com/juan/dotnet-core', DATEADD(DAY, -15, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -4, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @JSId, 4, 
     'https://github.com/juan/js-projects', DATEADD(DAY, -20, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -4, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @ReactId, 4, 
     NULL, DATEADD(DAY, -10, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @JavaId, 4, 
     'https://github.com/juan/java-microservices', DATEADD(DAY, -30, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @SpringBootId, 3, 
     NULL, DATEADD(DAY, -30, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -2, SYSUTCDATETIME()), 0);

-- Ana López (Backend Developer)
INSERT INTO [talent].[EmployeeSkills] (Id, OrganizationId, UserId, SkillId, Level, EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @CsharpId, 4, 
     'https://github.com/ana/backend-services', DATEADD(DAY, -5, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @AzureId, 3, 
     NULL, DATEADD(DAY, -7, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @JavaId, 5, 
     'https://github.com/ana/java-enterprise', DATEADD(DAY, -10, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @SpringBootId, 5, 
     'https://github.com/ana/springboot-apps', DATEADD(DAY, -10, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @PostgreSQLId, 4, 
     NULL, DATEADD(DAY, -15, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -2, SYSUTCDATETIME()), 0);

PRINT '  ✓ 13 employee skills insertadas';

-- =============================================
-- 8. TALENT: Certifications
-- =============================================
PRINT '8. Insertando Certifications...';

INSERT INTO [talent].[Certifications] (Id, OrganizationId, UserId, Name, Issuer, IssueDate, ExpirationDate, EvidenceUrl, CreatedAt, CreatedByUserId, UpdatedAt, UpdatedByUserId, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', 
     'Microsoft Certified: Azure Developer Associate', 'Microsoft', '2023-06-15', '2025-06-15', 
     'https://learn.microsoft.com/credentials/12345', DATEADD(MONTH, -8, SYSUTCDATETIME()), NULL, NULL, NULL, 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', 
     'AWS Certified Solutions Architect', 'Amazon Web Services', '2024-03-10', '2027-03-10', 
     'https://aws.amazon.com/certification/67890', DATEADD(MONTH, -10, SYSUTCDATETIME()), NULL, NULL, NULL, 0);

PRINT '   ✓ 2 certifications';

-- =============================================
-- 9. PROJECTS: Projects
-- =============================================
PRINT '9. Insertando Projects...';

INSERT INTO [projects].[Projects] (Id, OrganizationId, Code, Name, Description, StartDate, EndDate, ComplexityLevel, Status, CreatedAt, IsDeleted)
VALUES 
    ('AAAAAAAA-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'PROJ-001', 
     'Sistema de Gestión Hospitalaria', 
     'Desarrollo de sistema integral para gestión de historias clínicas y citas médicas', 
     '2025-11-01', '2026-05-31', 3, 3, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0), -- Status=3 (InProgress)
    ('BBBBBBBB-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'PROJ-002', 
     'E-commerce Multitienda', 
     'Plataforma de comercio electrónico con soporte para múltiples tiendas', 
     '2026-02-01', '2026-07-31', 2, 2, DATEADD(MONTH, -1, SYSUTCDATETIME()), 0), -- Status=2 (Open)
    ('CCCCCCCC-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', 'PROJ-003', 
     'App Móvil de Delivery', 
     'Aplicación móvil para pedidos y entregas a domicilio', 
     NULL, NULL, 1, 1, DATEADD(DAY, -10, SYSUTCDATETIME()), 0); -- Status=1 (Draft)

PRINT '   ✓ 3 proyectos';

-- =============================================
-- 10. PROJECTS: Project Skill Requirements
-- =============================================
PRINT '10. Insertando Project Skill Requirements...';

DECLARE @MicroservicesId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Microservicios' AND OrganizationId IS NULL);
DECLARE @DotNetCoreId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = '.NET Core' AND OrganizationId IS NULL);
DECLARE @DockerId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Docker' AND OrganizationId IS NULL);
DECLARE @KubernetesId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Kubernetes' AND OrganizationId IS NULL);

-- Sistema Hospitalario (requiere C# + Azure + .NET Core + Microservicios)
INSERT INTO [projects].[ProjectSkillRequirements] (Id, OrganizationId, ProjectId, SkillId, RequiredLevel, IsMandatory, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', @CsharpId, 4, 1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', @AzureId, 3, 1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', @DotNetCoreId, 4, 1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', @MicroservicesId, 3, 0, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0);

-- E-commerce (requiere React + JavaScript + REST APIs)
INSERT INTO [projects].[ProjectSkillRequirements] (Id, OrganizationId, ProjectId, SkillId, RequiredLevel, IsMandatory, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', @ReactId, 4, 1, DATEADD(MONTH, -1, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', @JSId, 3, 0, DATEADD(MONTH, -1, SYSUTCDATETIME()), 0);

-- App Móvil Delivery (requiere Java + Spring Boot + PostgreSQL + Microservicios)
INSERT INTO [projects].[ProjectSkillRequirements] (Id, OrganizationId, ProjectId, SkillId, RequiredLevel, IsMandatory, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', @JavaId, 4, 1, DATEADD(DAY, -10, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', @SpringBootId, 4, 1, DATEADD(DAY, -10, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', @PostgreSQLId, 3, 1, DATEADD(DAY, -10, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', @MicroservicesId, 3, 0, DATEADD(DAY, -10, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', @DockerId, 3, 0, DATEADD(DAY, -10, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'CCCCCCCC-1111-1111-1111-111111111111', @KubernetesId, 2, 0, DATEADD(DAY, -10, SYSUTCDATETIME()), 0);

PRINT '   ✓ 13 project skill requirements';

-- =============================================
-- 11. PROJECTS: Project Roles
-- =============================================
PRINT '11. Insertando Project Roles...';

INSERT INTO [projects].[ProjectRoles] (Id, OrganizationId, ProjectId, Name, NeededCount, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', 'Tech Lead', 1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', 'Backend Developer', 2, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', 'Full Stack Developer', 3, DATEADD(MONTH, -1, SYSUTCDATETIME()), 0);

PRINT '   ✓ 3 project roles';

-- =============================================
-- 12. PROJECTS: Project Applications
-- =============================================
PRINT '12. Insertando Project Applications...';

INSERT INTO [projects].[ProjectApplications] (Id, OrganizationId, ProjectId, UserId, Motivation, Status, ReviewedByUserId, ReviewNotes, ReviewedAt, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', 
     'Me interesa liderar el desarrollo backend de este proyecto. Cuento con experiencia en arquitecturas de microservicios.', 
     2, '11111111-0000-0000-0000-000000000002', 'Excelente perfil, aprobado para el rol de Tech Lead', 
     DATEADD(DAY, -5, SYSUTCDATETIME()), DATEADD(DAY, -7, SYSUTCDATETIME()), 0), -- Status=2 (Approved)
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', 
     'Tengo experiencia en desarrollo de APIs REST y bases de datos SQL Server.', 
     1, NULL, NULL, NULL, DATEADD(DAY, -3, SYSUTCDATETIME()), 0), -- Status=1 (Applied)
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', 
     'Me gustaría participar en el desarrollo del frontend con React.', 
     1, NULL, NULL, NULL, DATEADD(DAY, -2, SYSUTCDATETIME()), 0); -- Status=1 (Applied)

PRINT '   ✓ 3 project applications';

-- =============================================
-- 13. PROJECTS: Project Assignments
-- =============================================
PRINT '13. Insertando Project Assignments...';

DECLARE @TechLeadRoleId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [projects].[ProjectRoles] 
    WHERE ProjectId = 'AAAAAAAA-1111-1111-1111-111111111111' AND Name = 'Tech Lead');

INSERT INTO [projects].[ProjectAssignments] (Id, OrganizationId, ProjectId, UserId, ProjectRoleId, AssignedByUserId, AssignedAt, Status, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', 
     @TechLeadRoleId, '11111111-0000-0000-0000-000000000002', DATEADD(DAY, -5, SYSUTCDATETIME()), 1, 0); -- Status=1 (Active)

PRINT '   ✓ 1 project assignment';

-- =============================================
-- 14. PROJECTS: Project Participation (Historial)
-- =============================================
PRINT '14. Insertando Project Participation...';

INSERT INTO [projects].[ProjectParticipation] (Id, OrganizationId, ProjectId, UserId, RoleName, ContributionScore, FeedbackComments, CompletedAt, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', 
     'Tech Lead', 5, 'Excelente liderazgo técnico y capacidad para resolver problemas complejos.', 
     NULL, DATEADD(DAY, -5, SYSUTCDATETIME()), 0);

PRINT '   ✓ 1 project participation';

-- =============================================
-- 15. TALENT: Skill Evaluations
-- =============================================
PRINT '15. Insertando Skill Evaluations...';

INSERT INTO [talent].[SkillEvaluations] (Id, OrganizationId, UserId, SkillId, Source, ProjectId, DeltaLevel, Reason, CreatedAt)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @CsharpId, 
     1, 'AAAAAAAA-1111-1111-1111-111111111111', 1, 'Mejora por liderazgo en proyecto hospitalario', DATEADD(DAY, -10, SYSUTCDATETIME())),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @LiderazgoId, 
     1, 'AAAAAAAA-1111-1111-1111-111111111111', 2, 'Excelente gestión de equipo durante la crisis del proyecto', DATEADD(DAY, -8, SYSUTCDATETIME()));

PRINT '   ✓ 2 skill evaluations';

-- =============================================
-- 16. REPORTING: Report Snapshots
-- =============================================
PRINT '16. Insertando Report Snapshots...';

INSERT INTO [reporting].[ReportSnapshots] (Id, OrganizationId, SnapshotDate, JsonPayload, CreatedAt)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', CAST(DATEADD(DAY, -7, SYSUTCDATETIME()) AS DATE), 
     '{"reportType":"TeamSkillsMatrix","totalEmployees":3,"avgSkillLevel":4.1,"topSkills":["C#","React","Azure"],"timestamp":"' + CONVERT(VARCHAR, DATEADD(DAY, -7, SYSUTCDATETIME()), 127) + '"}', 
     DATEADD(DAY, -7, SYSUTCDATETIME())),
    (NEWID(), '11111111-1111-1111-1111-111111111111', CAST(DATEADD(DAY, -1, SYSUTCDATETIME()) AS DATE), 
     '{"reportType":"ProjectStatus","activeProjects":1,"openProjects":1,"draftProjects":1,"totalApplications":3,"timestamp":"' + CONVERT(VARCHAR, DATEADD(DAY, -1, SYSUTCDATETIME()), 127) + '"}', 
     DATEADD(DAY, -1, SYSUTCDATETIME()));

PRINT '   ✓ 2 report snapshots';

-- =============================================
-- 17. REPORTING: Recommendation Rules
-- =============================================
PRINT '17. Insertando Recommendation Rules...';

INSERT INTO [reporting].[RecommendationRules] (Id, OrganizationId, Name, ConditionExpr, RecommendationText, IsActive, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'Sugerir capacitación', 
     'SkillLevel < 3 AND ProjectRequirement >= 4', 
     'Se recomienda capacitación adicional para cumplir con requisitos del proyecto', 
     1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'Alertar sobrecarga', 
     'ActiveProjects > 2', 
     'El empleado está asignado a más de 2 proyectos activos, considerar redistribución', 
     1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0);



PRINT '==========================================';
PRINT 'Seeder RBAC: Permisos y Asignaciones de Roles';
PRINT '==========================================';

-- =============
-- 1) Permisos
-- =============
-- Convención: <module>.<action>

-- IAM / Usuarios / Roles / Permisos
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'users.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'users.read', 'Ver Usuarios', 'Listar y ver usuarios de la organización', 'iam');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'users.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'users.write', 'Administrar Usuarios', 'Crear/editar usuarios', 'iam');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'users.assign_roles')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'users.assign_roles', 'Asignar Roles', 'Asignar/retirar roles a usuarios', 'iam');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'roles.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'roles.read', 'Ver Roles', 'Listar y ver roles', 'iam');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'roles.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'roles.write', 'Administrar Roles', 'Crear/editar roles', 'iam');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'permissions.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'permissions.read', 'Ver Permisos', 'Listar permisos del sistema', 'iam');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'permissions.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'permissions.write', 'Administrar Permisos', 'Crear/editar permisos del sistema', 'iam');

-- Projects & Applications
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'projects.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'projects.read', 'Ver Proyectos', 'Listar y ver proyectos', 'projects');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'projects.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'projects.write', 'Administrar Proyectos', 'Crear/editar proyectos', 'projects');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'projects.publish')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'projects.publish', 'Publicar Proyectos', 'Marcar proyecto como visible/abierto para postulaciones', 'projects');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'projects.assign')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'projects.assign', 'Asignar Recursos', 'Asignar usuarios a proyectos', 'projects');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'projects.apply')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'projects.apply', 'Postular a Proyecto', 'Permite a un usuario aplicar a proyectos', 'projects');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'applications.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'applications.read', 'Ver Postulaciones', 'Ver postulaciones a proyectos', 'projects');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'applications.review')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'applications.review', 'Revisar Postulaciones', 'Aprobar/Rechazar postulaciones', 'projects');

-- Assignments
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'assignments.manage')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'assignments.manage', 'Gestionar Asignaciones', 'Crear/terminar asignaciones en proyectos', 'projects');

-- Talent / Skills / Certifications
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'talent.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'talent.read', 'Ver Talento', 'Ver perfiles y skills', 'talent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'talent.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'talent.write', 'Administrar Talento', 'Modificar perfiles y datos de talento', 'talent');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'skills.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'skills.read', 'Ver Skills', 'Ver catálogo de skills', 'talent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'skills.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'skills.write', 'Administrar Skills', 'Crear/editar skills del catálogo', 'talent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'skills.validate')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'skills.validate', 'Validar Skills', 'Aprobar evidencias y validaciones de skill', 'talent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'skills.self_update')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'skills.self_update', 'Actualizar Mis Skills', 'Permite al usuario actualizar sus propias habilidades', 'talent');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'certifications.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'certifications.read', 'Ver Certificaciones', 'Ver certificaciones de empleados', 'talent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'certifications.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'certifications.write', 'Administrar Certificaciones', 'Agregar/editar certificaciones', 'talent');

-- Reporting & Agent
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'reports.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'reports.read', 'Ver Reportes', 'Acceso a reportes e indicadores', 'reporting');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'reports.generate')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'reports.generate', 'Generar Reportes', 'Generar snapshots y reportes', 'reporting');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'agent.actions.execute')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'agent.actions.execute', 'Ejecutar Acciones del Agente', 'Permite ejecutar acciones automáticas del agente', 'agent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'agent.actions.approve')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'agent.actions.approve', 'Aprobar Acciones del Agente', 'Aprobar acciones en flujo HITL', 'agent');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'agent.actions.view')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'agent.actions.view', 'Ver Acciones del Agente', 'Ver historial y logs del agente', 'agent');

-- Config & System
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'config.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'config.read', 'Ver Configuración', 'Leer parámetros y catálogos del sistema', 'config');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'config.write')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'config.write', 'Administrar Configuración', 'Modificar catálogos y parámetros del sistema', 'config');

IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'audit.read')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'audit.read', 'Ver Auditoría', 'Acceso a registros de auditoría del sistema', 'system');
IF NOT EXISTS(SELECT 1 FROM iam.Permissions WHERE Code = 'system.manage')
    INSERT INTO iam.Permissions (Id, Code, Name, Description, Module) VALUES (NEWID(), 'system.manage', 'Administrar Sistema', 'Permisos críticos / super-admin', 'system');

PRINT '  ✓ Permisos insertados (si no existían)';

-- =============
-- 2) Asignación de Permisos a Roles (RolePermissions)
-- =============
PRINT 'Asignando permisos a roles...';

-- 2.a Admin -> todos los permisos
INSERT INTO iam.RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM iam.Roles r
CROSS JOIN iam.Permissions p
WHERE r.Name = 'Admin'
  AND NOT EXISTS (SELECT 1 FROM iam.RolePermissions rp WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id);

-- 2.b Manager -> permisos operativos y de reporte/agent/config lectura
INSERT INTO iam.RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM iam.Roles r
JOIN iam.Permissions p ON p.Code IN (
    'projects.read','projects.write','projects.assign','projects.publish',
    'applications.read','applications.review',
    'assignments.manage',
    'talent.read','talent.write','skills.read','skills.validate','certifications.read',
    'reports.read','reports.generate',
    'agent.actions.view','config.read','permissions.read','users.read','users.assign_roles'
)
WHERE r.Name = 'Manager'
  AND NOT EXISTS (SELECT 1 FROM iam.RolePermissions rp WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id);

-- 2.c Developer -> acceso de trabajo diario y a auto-servicio
INSERT INTO iam.RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM iam.Roles r
JOIN iam.Permissions p ON p.Code IN (
    'projects.read','projects.apply',
    'applications.apply','applications.read',
    'talent.read','skills.read','skills.self_update','certifications.read',
    'reports.read'
)
WHERE r.Name = 'Developer'
  AND NOT EXISTS (SELECT 1 FROM iam.RolePermissions rp WHERE rp.RoleId = r.Id AND rp.PermissionId = p.Id);

PRINT '  ✓ RolePermissions aplicadas (Admin = all, Manager/Developer = subsets)';

-- =============
-- 3) Ejemplos de UserPermissions (overrides por usuario)
-- =============
PRINT 'Aplicando ejemplos de UserPermissions...';

-- Ejemplo: otorgar a Juan (developer) permiso explícito para crear/editar proyectos (elevación puntual)
INSERT INTO iam.UserPermissions (UserId, PermissionId, OrganizationId, IsGranted, CreatedAt)
SELECT u.Id, p.Id, u.OrganizationId, 1, SYSUTCDATETIME()
FROM iam.Users u
JOIN iam.Permissions p ON p.Code = 'projects.write'
WHERE u.Email = 'juan.martinez@techcorp.com'
  AND NOT EXISTS (SELECT 1 FROM iam.UserPermissions up WHERE up.UserId = u.Id AND up.PermissionId = p.Id);

-- Ejemplo: denegar (IsGranted = 0) permiso a un usuario específico (override negativo)
INSERT INTO iam.UserPermissions (UserId, PermissionId, OrganizationId, IsGranted, CreatedAt)
SELECT u.Id, p.Id, u.OrganizationId, 0, SYSUTCDATETIME()
FROM iam.Users u
JOIN iam.Permissions p ON p.Code = 'projects.assign'
WHERE u.Email = 'maria.garcia@techcorp.com' -- ejemplo: Manager pero revocación puntual
  AND NOT EXISTS (SELECT 1 FROM iam.UserPermissions up WHERE up.UserId = u.Id AND up.PermissionId = p.Id);

PRINT '  ✓ UserPermissions de ejemplo aplicadas';

PRINT '';
PRINT 'Seeder RBAC completado.';
PRINT '  - Permisos totales (estimado): revisar iam.Permissions';
PRINT '  - RolePermissions: Admin = todos, Manager/Developer = subconjuntos realistas';

PRINT '   ✓ 2 recommendation rules';

PRINT '';
PRINT '==========================================';
PRINT 'SEEDING COMPLETADO';
PRINT '==========================================';
PRINT '✓ 2 Organizaciones';
PRINT '✓ 3 Roles';
PRINT '✓ 5 Usuarios';
PRINT '✓ 6 Asignaciones de roles';
PRINT '✓ 22 Skills (21 globales + 1 organizacional)';
PRINT '✓ 3 Employee Profiles';
PRINT '✓ 13 Employee Skills (con Java, Spring Boot, .NET Core)';
PRINT '✓ 2 Certifications';
PRINT '✓ 3 Projects (Sistema Hospitalario, E-commerce, App Móvil)';
PRINT '✓ 13 Project Skill Requirements (incluye Microservicios, Docker, K8s)';
PRINT '✓ 3 Project Roles';
PRINT '✓ 3 Project Applications';
PRINT '✓ 1 Project Assignment';
PRINT '✓ 1 Project Participation';
PRINT '✓ 2 Skill Evaluations';
PRINT '✓ 2 Report Snapshots';
PRINT '✓ 2 Recommendation Rules';
PRINT '';
PRINT 'Cuentas de prueba (Password: "Password123!"):';
PRINT '  • admin@techcorp.com (Admin + Manager)';
PRINT '  • maria.garcia@techcorp.com (Manager) - C#:4, Azure:4, Liderazgo:5';
PRINT '  • juan.martinez@techcorp.com (Developer) - C#:5, JS:4, React:4, Java:4, Spring:3';
PRINT '  • ana.lopez@techcorp.com (Developer) - C#:4, Azure:3, Java:5, Spring:5, PostgreSQL:4';
PRINT '  • admin@innovatelab.com (Admin)';
PRINT '';
PRINT 'Organization ID TechCorp:';
PRINT '  11111111-1111-1111-1111-111111111111';
PRINT '';
PRINT '🤖 DATOS PARA PRUEBAS DEL AGENTE IA:';
PRINT '  ✓ 3 empleados con skills en Java (nivel 4-5)';
PRINT '  ✓ 2 empleados con Spring Boot';
PRINT '  ✓ 1 proyecto con requisitos de Java + Microservicios (App Móvil)';
PRINT '  ✓ Skills validadas con evidencia en GitHub';
PRINT '';
PRINT '📝 QUERIES DE PRUEBA RECOMENDADAS:';
PRINT '  • POST /agent/query: "¿Cuántos desarrolladores tenemos con Java?"';
PRINT '  • POST /agent/match-candidates: projectId del App Móvil Delivery';
PRINT '  • POST /agent/validate-skill: userId de Ana + skillId de Java';
PRINT '==========================================';

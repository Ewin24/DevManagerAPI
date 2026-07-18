-- Script para limpiar datos del seeder


SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT 'Limpiando datos de la base de datos...';

-- Borrar catálogos de config (identities serán reseteados más adelante)
DELETE FROM [config].[SkillTypes];
DELETE FROM [config].[SkillCategories];
DELETE FROM [config].[AgentActionStatuses];
DELETE FROM [config].[AgentActionTypes];
DELETE FROM [config].[SeniorityLevels];

-- Eliminar datos en orden inverso a dependencies
DELETE FROM [reporting].[RecommendationLogs];
DELETE FROM [reporting].[RecommendationRules];
DELETE FROM [reporting].[ReportSnapshots];

DELETE FROM [projects].[ProjectParticipation];
DELETE FROM [projects].[ProjectAssignments];
DELETE FROM [projects].[ProjectApplications];
DELETE FROM [projects].[ProjectRoles];
DELETE FROM [projects].[ProjectSkillRequirements];
DELETE FROM [projects].[Projects];

DELETE FROM [talent].[SkillEvaluations];
DELETE FROM [talent].[Certifications];
DELETE FROM [talent].[EmployeeSkills];
DELETE FROM [talent].[EmployeeProfiles];
DELETE FROM [talent].[Skills];

DELETE FROM [iam].[RolePermissions];
DELETE FROM [iam].[UserRoles];
DELETE FROM [iam].[Users];
DELETE FROM [iam].[Roles];
DELETE FROM [iam].[Organizations];

-- Reseteo de IDENTITIES en tablas que los utilizan
DBCC CHECKIDENT('[config].[SkillTypes]', RESEED, 0);
DBCC CHECKIDENT('[config].[SkillCategories]', RESEED, 0);
DBCC CHECKIDENT('[config].[AgentActionStatuses]', RESEED, 0);
DBCC CHECKIDENT('[config].[AgentActionTypes]', RESEED, 0);
DBCC CHECKIDENT('[config].[SeniorityLevels]', RESEED, 0);

PRINT '✓ Base de datos limpiada exitosamente';
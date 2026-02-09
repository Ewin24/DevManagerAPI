-- Script para limpiar datos del seeder
USE DevManager;

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

PRINT 'Limpiando datos de la base de datos...';

-- Desactivar constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

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

DELETE FROM [iam].[UserRoles];
DELETE FROM [iam].[Users];
DELETE FROM [iam].[Roles];
DELETE FROM [iam].[Organizations];

-- Reactivar constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL';

PRINT '✓ Base de datos limpiada exitosamente';
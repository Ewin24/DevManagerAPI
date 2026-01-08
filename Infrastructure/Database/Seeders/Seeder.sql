-- =============================================
-- DevManager Database Seeder - COMPLETO
-- Basado en DDL_Dev_Manager.sql
-- Password: "Password123!"
-- =============================================

USE DevManager;
GO

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

PRINT '==========================================';
PRINT 'DevManager - Seeder Completo';
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

INSERT INTO [talent].[Skills] (Id, OrganizationId, Name, Category, SkillType, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), NULL, 'C#', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'JavaScript', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Python', 'Programming Language', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'SQL Server', 'Database', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Azure', 'Cloud', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'React', 'Framework', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Docker', 'DevOps', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Kubernetes', 'DevOps', 'Hard', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Liderazgo', 'Management', 'Soft', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), NULL, 'Comunicación', 'Interpersonal', 'Soft', DATEADD(MONTH, -6, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'Metodología TechCorp', 'Process', 'Hard', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0);

PRINT '  ✓ 11 skills insertadas';

-- =============================================
-- 6. TALENT: Employee Profiles
-- =============================================
PRINT '6. Insertando Employee Profiles...';

INSERT INTO [talent].[EmployeeProfiles] (UserId, OrganizationId, Bio, YearsExperience, LinkedInUrl, PortfolioUrl, CreatedAt, IsDeleted)
VALUES 
    ('11111111-0000-0000-0000-000000000002', '11111111-1111-1111-1111-111111111111', 
     'Ingeniera de software con 10 años de experiencia liderando equipos de desarrollo.', 
     10, 'https://linkedin.com/in/mariagarcia', 'https://mariagarcia.dev', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0),
    ('11111111-0000-0000-0000-000000000003', '11111111-1111-1111-1111-111111111111', 
     'Desarrollador full stack especializado en .NET y React con 8 años de experiencia.', 
     8, 'https://linkedin.com/in/juanmartinez', 'https://github.com/juandev', DATEADD(MONTH, -4, SYSUTCDATETIME()), 0),
    ('11111111-0000-0000-0000-000000000004', '11111111-1111-1111-1111-111111111111', 
     'Desarrolladora backend con experiencia en arquitecturas de microservicios y cloud.', 
     5, NULL, 'https://github.com/analopez', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0);

PRINT '  ✓ 3 perfiles de empleados insertados';

-- =============================================
-- 7. TALENT: Employee Skills
-- =============================================
PRINT '7. Insertando Employee Skills...';

DECLARE @CsharpId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'C#');
DECLARE @JSId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'JavaScript');
DECLARE @ReactId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'React');
DECLARE @AzureId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Azure');
DECLARE @LiderazgoId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM [talent].[Skills] WHERE Name = 'Liderazgo');

-- María García
INSERT INTO [talent].[EmployeeSkills] (Id, OrganizationId, UserId, SkillId, Level, EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000002', @CsharpId, 4, 
     'https://github.com/maria/csharp-projects', DATEADD(MONTH, -1, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000001', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000002', @LiderazgoId, 5, 
     NULL, DATEADD(MONTH, -2, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000001', DATEADD(MONTH, -5, SYSUTCDATETIME()), 0);

-- Juan Martínez
INSERT INTO [talent].[EmployeeSkills] (Id, OrganizationId, UserId, SkillId, Level, EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @CsharpId, 5, 
     'https://github.com/juan/dotnet-core', DATEADD(DAY, -15, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -4, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @JSId, 4, 
     'https://github.com/juan/js-projects', DATEADD(DAY, -20, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -4, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', @ReactId, 4, 
     NULL, DATEADD(DAY, -10, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0);

-- Ana López
INSERT INTO [talent].[EmployeeSkills] (Id, OrganizationId, UserId, SkillId, Level, EvidenceUrl, LastValidatedAt, ValidatedByUserId, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @CsharpId, 4, 
     'https://github.com/ana/backend-services', DATEADD(DAY, -5, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -3, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', @AzureId, 3, 
     NULL, DATEADD(DAY, -7, SYSUTCDATETIME()), '11111111-0000-0000-0000-000000000002', DATEADD(MONTH, -2, SYSUTCDATETIME()), 0);

PRINT '  ✓ 7 employee skills insertadas';

-- =============================================
-- 8. TALENT: Certifications
-- =============================================
PRINT '8. Insertando Certifications...';

INSERT INTO [talent].[Certifications] (Id, OrganizationId, UserId, Name, Issuer, IssueDate, ExpirationDate, EvidenceUrl, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000003', 
     'Microsoft Certified: Azure Developer Associate', 'Microsoft', '2023-06-15', '2025-06-15', 
     'https://learn.microsoft.com/credentials/12345', DATEADD(MONTH, -8, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', '11111111-0000-0000-0000-000000000004', 
     'AWS Certified Solutions Architect', 'Amazon Web Services', '2024-03-10', '2027-03-10', 
     'https://aws.amazon.com/certification/67890', DATEADD(MONTH, -10, SYSUTCDATETIME()), 0);

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

INSERT INTO [projects].[ProjectSkillRequirements] (Id, OrganizationId, ProjectId, SkillId, RequiredLevel, IsMandatory, CreatedAt, IsDeleted)
VALUES 
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', @CsharpId, 4, 1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'AAAAAAAA-1111-1111-1111-111111111111', @AzureId, 3, 1, DATEADD(MONTH, -2, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', @ReactId, 4, 1, DATEADD(MONTH, -1, SYSUTCDATETIME()), 0),
    (NEWID(), '11111111-1111-1111-1111-111111111111', 'BBBBBBBB-1111-1111-1111-111111111111', @JSId, 3, 0, DATEADD(MONTH, -1, SYSUTCDATETIME()), 0);

PRINT '   ✓ 4 project skill requirements';

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

PRINT '   ✓ 2 recommendation rules';

PRINT '';
PRINT '==========================================';
PRINT 'SEEDING COMPLETADO';
PRINT '==========================================';
PRINT '✓ 2 Organizaciones';
PRINT '✓ 3 Roles';
PRINT '✓ 5 Usuarios';
PRINT '✓ 6 Asignaciones de roles';
PRINT '✓ 11 Skills';
PRINT '✓ 3 Employee Profiles';
PRINT '✓ 7 Employee Skills';
PRINT '✓ 2 Certifications';
PRINT '✓ 3 Projects';
PRINT '✓ 4 Project Skill Requirements';
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
PRINT '  • maria.garcia@techcorp.com (Manager)';
PRINT '  • juan.martinez@techcorp.com (Developer)';
PRINT '  • ana.lopez@techcorp.com (Developer)';
PRINT '  • admin@innovatelab.com (Admin)';
PRINT '';
PRINT 'Organization ID TechCorp:';
PRINT '  11111111-1111-1111-1111-111111111111';
PRINT '==========================================';
GO

/* =========================================================
   DevManager - SQL Server Database Schema (Multi-tenant)
   ========================================================= */

-- CREATE DATABASE DevManager;
-- GO
-- USE DevManager;
-- GO

/* =========================
   0) Esquemas
   ========================= */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'iam') EXEC('CREATE SCHEMA iam');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'talent') EXEC('CREATE SCHEMA talent');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'projects') EXEC('CREATE SCHEMA projects');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'reporting') EXEC('CREATE SCHEMA reporting');
GO

/* =========================
   1) Tablas IAM (Identity & Access)
   ========================= */

CREATE TABLE iam.Organizations (
    Id                uniqueidentifier NOT NULL CONSTRAINT PK_Organizations PRIMARY KEY,
    Name              nvarchar(160)     NOT NULL,
    LegalName         nvarchar(200)     NULL,
    Nit               nvarchar(30)      NULL,
    IsActive          bit               NOT NULL CONSTRAINT DF_Organizations_IsActive DEFAULT (1),

    CreatedAt         datetime2(3)      NOT NULL CONSTRAINT DF_Organizations_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId   uniqueidentifier  NULL,
    UpdatedAt         datetime2(3)      NULL,
    UpdatedByUserId   uniqueidentifier  NULL,
    IsDeleted         bit               NOT NULL CONSTRAINT DF_Organizations_IsDeleted DEFAULT (0),
    DeletedAt         datetime2(3)      NULL,
    DeletedByUserId   uniqueidentifier  NULL,

    CONSTRAINT UQ_Organizations_Nit UNIQUE (Nit)
);
GO

CREATE TABLE iam.Users (
    Id                uniqueidentifier NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    OrganizationId    uniqueidentifier NOT NULL,
    FirstName         nvarchar(80)     NOT NULL,
    LastName          nvarchar(80)     NOT NULL,
    Email             nvarchar(254)    NOT NULL,
    Phone             nvarchar(30)     NULL,

    PasswordHash      varbinary(512)   NOT NULL,
    PasswordSalt      varbinary(256)   NOT NULL,

    IsActive          bit              NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT (1),
    LastLoginAt       datetime2(3)     NULL,

    CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId   uniqueidentifier NULL,
    UpdatedAt         datetime2(3)     NULL,
    UpdatedByUserId   uniqueidentifier NULL,
    IsDeleted         bit              NOT NULL CONSTRAINT DF_Users_IsDeleted DEFAULT (0),
    DeletedAt         datetime2(3)     NULL,
    DeletedByUserId   uniqueidentifier NULL,

    CONSTRAINT FK_Users_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE UNIQUE INDEX UX_Users_Org_Email
ON iam.Users(OrganizationId, Email)
WHERE IsDeleted = 0;
GO

CREATE INDEX IX_Users_Org_IsActive
ON iam.Users(OrganizationId, IsActive)
INCLUDE (Email, FirstName, LastName);
GO

CREATE TABLE iam.Roles (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    OrganizationId  uniqueidentifier NULL, 
    Name            nvarchar(80)     NOT NULL,
    Description     nvarchar(200)    NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_Roles_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_Roles_IsDeleted DEFAULT (0),

    CONSTRAINT FK_Roles_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE UNIQUE INDEX UX_Roles_Org_Name
ON iam.Roles(OrganizationId, Name)
WHERE IsDeleted = 0;
GO

CREATE TABLE iam.UserRoles (
    UserId          uniqueidentifier NOT NULL,
    RoleId          uniqueidentifier NOT NULL,
    OrganizationId  uniqueidentifier NOT NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_UserRoles_CreatedAt DEFAULT (sysutcdatetime()),

    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES iam.Roles(Id),
    CONSTRAINT FK_UserRoles_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE INDEX IX_UserRoles_Org_User
ON iam.UserRoles(OrganizationId, UserId);
GO

/* =========================
   2) Tablas Talent (Perfil, Skills, Certificaciones)
   ========================= */

CREATE TABLE talent.EmployeeProfiles (
    UserId            uniqueidentifier NOT NULL CONSTRAINT PK_EmployeeProfiles PRIMARY KEY,
    OrganizationId    uniqueidentifier NOT NULL,
    Bio               nvarchar(800)    NULL,
    YearsExperience   int              NULL,
    LinkedInUrl       nvarchar(300)    NULL,
    PortfolioUrl      nvarchar(300)    NULL,

    CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_EmployeeProfiles_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId   uniqueidentifier NULL,
    UpdatedAt         datetime2(3)     NULL,
    UpdatedByUserId   uniqueidentifier NULL,
    IsDeleted         bit              NOT NULL CONSTRAINT DF_EmployeeProfiles_IsDeleted DEFAULT (0),

    CONSTRAINT FK_EmployeeProfiles_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_EmployeeProfiles_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE INDEX IX_EmployeeProfiles_Org
ON talent.EmployeeProfiles(OrganizationId)
WHERE IsDeleted = 0;
GO

CREATE TABLE talent.Skills (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_Skills PRIMARY KEY,
    OrganizationId  uniqueidentifier NULL, 
    Name            nvarchar(120)    NOT NULL,
    Category        nvarchar(80)     NULL,
    SkillType       nvarchar(20)     NULL, -- 'Hard', 'Soft', 'Language' tipo de skill para reportes y análisis

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_Skills_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_Skills_IsDeleted DEFAULT (0),

    CONSTRAINT FK_Skills_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE UNIQUE INDEX UX_Skills_Org_Name
ON talent.Skills(OrganizationId, Name)
WHERE IsDeleted = 0;
GO

CREATE TABLE talent.EmployeeSkills (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_EmployeeSkills PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    SkillId         uniqueidentifier NOT NULL,

    Level           tinyint          NOT NULL, -- 1..5
    EvidenceUrl     nvarchar(400)    NULL,
    LastValidatedAt datetime2(3)     NULL,
    ValidatedByUserId uniqueidentifier NULL, -- Necesario para auditoría: saber si lo validó el Sistema (Agente) o un Humano.
    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_EmployeeSkills_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId uniqueidentifier NULL,
    UpdatedAt       datetime2(3)     NULL,
    UpdatedByUserId uniqueidentifier NULL,
    IsDeleted       bit              NOT NULL CONSTRAINT DF_EmployeeSkills_IsDeleted DEFAULT (0),

    CONSTRAINT FK_EmployeeSkills_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_EmployeeSkills_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_EmployeeSkills_Skills
        FOREIGN KEY (SkillId) REFERENCES talent.Skills(Id),
    CONSTRAINT FK_EmployeeSkills_Validator
        FOREIGN KEY (ValidatedByUserId) REFERENCES iam.Users(Id),
    CONSTRAINT CK_EmployeeSkills_Level CHECK (Level BETWEEN 1 AND 5)
);
GO

CREATE UNIQUE INDEX UX_EmployeeSkills_Org_User_Skill
ON talent.EmployeeSkills(OrganizationId, UserId, SkillId)
WHERE IsDeleted = 0;
GO

CREATE INDEX IX_EmployeeSkills_Org_Skill
ON talent.EmployeeSkills(OrganizationId, SkillId)
INCLUDE (UserId, Level)
WHERE IsDeleted = 0;
GO

CREATE TABLE talent.Certifications (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_Certifications PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    Name            nvarchar(160)    NOT NULL,
    Issuer          nvarchar(120)    NULL,
    IssueDate       date             NULL,
    ExpirationDate  date             NULL,
    EvidenceUrl     nvarchar(400)    NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_Certifications_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId uniqueidentifier NULL,
    UpdatedAt       datetime2(3)     NULL,
    UpdatedByUserId uniqueidentifier NULL,
    IsDeleted       bit              NOT NULL CONSTRAINT DF_Certifications_IsDeleted DEFAULT (0),

    CONSTRAINT FK_Certifications_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_Certifications_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id)
);
GO

CREATE INDEX IX_Certifications_Org_User
ON talent.Certifications(OrganizationId, UserId)
WHERE IsDeleted = 0;
GO

/* =========================
   3) Tablas Projects
   ========================= */

CREATE TABLE projects.Projects (
    Id               uniqueidentifier NOT NULL CONSTRAINT PK_Projects PRIMARY KEY,
    OrganizationId   uniqueidentifier NOT NULL,

    Code             nvarchar(40)     NULL,
    Name             nvarchar(160)    NOT NULL,
    Description      nvarchar(max)    NULL,

    StartDate        date             NULL,
    EndDate          date             NULL,

    ComplexityLevel  tinyint          NOT NULL CONSTRAINT DF_Projects_Complexity DEFAULT(1), -- El sistema debe saber la dificultad del proyecto para calcular cuánta experiencia ganan los empleados. 
    Status           tinyint          NOT NULL, -- 1 Draft, 2 Open, 3 InProgress, 4 Closed, 5 Cancelled

    CreatedAt        datetime2(3)     NOT NULL CONSTRAINT DF_Projects_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId  uniqueidentifier NULL,
    UpdatedAt        datetime2(3)     NULL,
    UpdatedByUserId  uniqueidentifier NULL,
    IsDeleted        bit              NOT NULL CONSTRAINT DF_Projects_IsDeleted DEFAULT (0),

    CONSTRAINT FK_Projects_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT CK_Projects_Status CHECK (Status BETWEEN 1 AND 5),
    CONSTRAINT CK_Projects_Complexity CHECK (ComplexityLevel BETWEEN 1 AND 3)
);
GO

CREATE INDEX IX_Projects_Org_Status
ON projects.Projects(OrganizationId, Status)
INCLUDE (Name, StartDate, EndDate)
WHERE IsDeleted = 0;
GO

CREATE UNIQUE INDEX UX_Projects_Org_Code
ON projects.Projects(OrganizationId, Code)
WHERE Code IS NOT NULL AND IsDeleted = 0;
GO

CREATE TABLE projects.ProjectSkillRequirements (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectSkillRequirements PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    SkillId         uniqueidentifier NOT NULL,

    RequiredLevel   tinyint          NOT NULL, -- 1..5
    IsMandatory     bit              NOT NULL CONSTRAINT DF_ProjectSkillRequirements_IsMandatory DEFAULT (1),

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectSkillRequirements_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectSkillRequirements_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectSkillRequirements_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectSkillRequirements_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT FK_ProjectSkillRequirements_Skills
        FOREIGN KEY (SkillId) REFERENCES talent.Skills(Id),
    CONSTRAINT CK_ProjectSkillRequirements_Level CHECK (RequiredLevel BETWEEN 1 AND 5)
);
GO

CREATE UNIQUE INDEX UX_ProjectSkillRequirements_Org_Project_Skill
ON projects.ProjectSkillRequirements(OrganizationId, ProjectId, SkillId)
WHERE IsDeleted = 0;
GO

CREATE INDEX IX_ProjectSkillRequirements_Org_Skill
ON projects.ProjectSkillRequirements(OrganizationId, SkillId)
INCLUDE (ProjectId, RequiredLevel, IsMandatory)
WHERE IsDeleted = 0;
GO

CREATE TABLE projects.ProjectRoles (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectRoles PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,

    Name            nvarchar(80)     NOT NULL, 
    NeededCount     int              NOT NULL CONSTRAINT DF_ProjectRoles_NeededCount DEFAULT (1),

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectRoles_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectRoles_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectRoles_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectRoles_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT CK_ProjectRoles_NeededCount CHECK (NeededCount >= 1)
);
GO

CREATE UNIQUE INDEX UX_ProjectRoles_Org_Project_Name
ON projects.ProjectRoles(OrganizationId, ProjectId, Name)
WHERE IsDeleted = 0;
GO

CREATE TABLE projects.ProjectApplications (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectApplications PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,

    Motivation      nvarchar(800)    NULL,
    Status          tinyint          NOT NULL, -- 1 Applied, 2 Approved, 3 Rejected, 4 Withdrawn

    ReviewedByUserId uniqueidentifier NULL,
    ReviewedAt       datetime2(3)     NULL,
    
    ReviewNotes     nvarchar(500)    NULL, -- Feedback de rechazo o aprobación. Útil para que el empleado sepa en qué mejorar.

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectApplications_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectApplications_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectApplications_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectApplications_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT FK_ProjectApplications_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_ProjectApplications_ReviewedByUser
        FOREIGN KEY (ReviewedByUserId) REFERENCES iam.Users(Id),
    CONSTRAINT CK_ProjectApplications_Status CHECK (Status BETWEEN 1 AND 4)
);
GO

CREATE UNIQUE INDEX UX_ProjectApplications_Org_Project_User
ON projects.ProjectApplications(OrganizationId, ProjectId, UserId)
WHERE IsDeleted = 0;
GO

CREATE INDEX IX_ProjectApplications_Org_Project_Status
ON projects.ProjectApplications(OrganizationId, ProjectId, Status)
INCLUDE (UserId, ReviewedAt)
WHERE IsDeleted = 0;
GO

CREATE TABLE projects.ProjectAssignments (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectAssignments PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    ProjectRoleId   uniqueidentifier NULL,

    AssignedByUserId uniqueidentifier NOT NULL,
    AssignedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectAssignments_AssignedAt DEFAULT (sysutcdatetime()),

    Status          tinyint          NOT NULL, -- 1 Active, 2 Removed, 3 Completed
    EndedAt         datetime2(3)     NULL,

    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectAssignments_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectAssignments_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectAssignments_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT FK_ProjectAssignments_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_ProjectAssignments_ProjectRoles
        FOREIGN KEY (ProjectRoleId) REFERENCES projects.ProjectRoles(Id),
    CONSTRAINT FK_ProjectAssignments_AssignedByUser
        FOREIGN KEY (AssignedByUserId) REFERENCES iam.Users(Id),
    CONSTRAINT CK_ProjectAssignments_Status CHECK (Status BETWEEN 1 AND 3)
);
GO

CREATE UNIQUE INDEX UX_ProjectAssignments_Org_Project_User_Active
ON projects.ProjectAssignments(OrganizationId, ProjectId, UserId)
WHERE IsDeleted = 0 AND Status = 1;
GO

CREATE INDEX IX_ProjectAssignments_Org_Project_Status
ON projects.ProjectAssignments(OrganizationId, ProjectId, Status)
INCLUDE (UserId, ProjectRoleId, AssignedAt)
WHERE IsDeleted = 0;
GO

/* =========================
   4) Historial / Evaluación de habilidades
   ========================= */

CREATE TABLE projects.ProjectParticipation (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectParticipation PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,

    RoleName        nvarchar(80)     NULL,
    ContributionScore tinyint        NULL, -- 1..5
    
    FeedbackComments nvarchar(max)   NULL,  -- Texto fundamental para el "Natural Language Processing" del Agente Inteligente. Sin texto, el agente es ciego a los matices.

    CompletedAt     datetime2(3)     NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectParticipation_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectParticipation_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectParticipation_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectParticipation_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT FK_ProjectParticipation_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT CK_ProjectParticipation_ContributionScore CHECK (ContributionScore IS NULL OR ContributionScore BETWEEN 1 AND 5)
);
GO

CREATE UNIQUE INDEX UX_ProjectParticipation_Org_Project_User
ON projects.ProjectParticipation(OrganizationId, ProjectId, UserId)
WHERE IsDeleted = 0;
GO

CREATE TABLE talent.SkillEvaluations (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_SkillEvaluations PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    SkillId         uniqueidentifier NOT NULL,

    Source          tinyint          NOT NULL, -- 1 Project, 2 Manual, 3 SystemRule
    ProjectId       uniqueidentifier NULL,

    DeltaLevel      smallint         NOT NULL, -- ej: +1, -1
    Reason          nvarchar(400)    NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_SkillEvaluations_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId uniqueidentifier NULL,

    CONSTRAINT FK_SkillEvaluations_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_SkillEvaluations_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_SkillEvaluations_Skills
        FOREIGN KEY (SkillId) REFERENCES talent.Skills(Id),
    CONSTRAINT FK_SkillEvaluations_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT CK_SkillEvaluations_Source CHECK (Source BETWEEN 1 AND 3),
    CONSTRAINT CK_SkillEvaluations_DeltaLevel CHECK (DeltaLevel BETWEEN -5 AND 5)
);
GO

CREATE INDEX IX_SkillEvaluations_Org_User_Skill_Date
ON talent.SkillEvaluations(OrganizationId, UserId, SkillId, CreatedAt DESC);
GO

/* =========================
   5) Reporting (BI básico & Agente)
   ========================= */

CREATE TABLE reporting.ReportSnapshots (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ReportSnapshots PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    SnapshotDate    date             NOT NULL,
    JsonPayload     nvarchar(max)    NOT NULL, 

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ReportSnapshots_CreatedAt DEFAULT (sysutcdatetime()),

    CONSTRAINT FK_ReportSnapshots_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE UNIQUE INDEX UX_ReportSnapshots_Org_Date
ON reporting.ReportSnapshots(OrganizationId, SnapshotDate);
GO

CREATE TABLE reporting.RecommendationRules (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_RecommendationRules PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    Name            nvarchar(120)    NOT NULL,
    ConditionExpr   nvarchar(800)    NOT NULL, -- expresión simple (rule engine)
    RecommendationText nvarchar(800) NOT NULL,
    IsActive        bit              NOT NULL CONSTRAINT DF_RecommendationRules_IsActive DEFAULT (1),

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_RecommendationRules_CreatedAt DEFAULT (sysutcdatetime()),
    UpdatedAt       datetime2(3)     NULL,
    IsDeleted       bit              NOT NULL CONSTRAINT DF_RecommendationRules_IsDeleted DEFAULT (0),

    CONSTRAINT FK_RecommendationRules_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);
GO

CREATE INDEX IX_RecommendationRules_Org_Active
ON reporting.RecommendationRules(OrganizationId, IsActive)
WHERE IsDeleted = 0;
GO

CREATE TABLE reporting.RecommendationLogs (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_RecommendationLogs PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    GeneratedAt     datetime2(3)     NOT NULL CONSTRAINT DF_RecommendationLogs_GeneratedAt DEFAULT (sysutcdatetime()),
    GeneratedByUserId uniqueidentifier NULL,

    ResultJson      nvarchar(max)    NOT NULL,

    CONSTRAINT FK_RecommendationLogs_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_RecommendationLogs_GeneratedByUser
        FOREIGN KEY (GeneratedByUserId) REFERENCES iam.Users(Id)
);
GO

CREATE INDEX IX_RecommendationLogs_Org_Date
ON reporting.RecommendationLogs(OrganizationId, GeneratedAt DESC);
GO

PRINT 'DevManager schema created successfully (with Expert Enhancements).';
GO
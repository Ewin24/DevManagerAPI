/* =========================================================
   DevManager - DDL Completo (Esquema Unificado)
   SQL Server | Multi-tenant | 5 Schemas
   Fecha: 9 de Febrero de 2026
   =========================================================
   Orden de ejecución:
     0) Esquemas
     1) Config (catálogos/parámetros — sin FKs externas)
     2) IAM (Identity & Access)
     3) Talent (Perfiles, Skills, Certificaciones)
     4) Projects (Proyectos, Postulaciones, Asignaciones)
     5) Historial / Evaluación de Habilidades
     6) Reporting (BI básico & Agente)
   ========================================================= */

-- CREATE DATABASE DevManager;
-- 
-- USE DevManager;
-- 


/* ==========================================================
   0) ESQUEMAS
   ========================================================== */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'config')    EXEC('CREATE SCHEMA config');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'iam')       EXEC('CREATE SCHEMA iam');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'talent')    EXEC('CREATE SCHEMA talent');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'projects')  EXEC('CREATE SCHEMA projects');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'reporting') EXEC('CREATE SCHEMA reporting');


PRINT '==========================================';
PRINT '0) Esquemas creados';
PRINT '==========================================';


/* ==========================================================
   1) TABLAS DE CONFIGURACIÓN / CATÁLOGOS  (schema: config)
      Se crean primero porque IAM, Talent y Projects
      tienen FKs apuntando a estas tablas.
   ========================================================== */

-- 1.1 Estados de Proyecto
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[ProjectStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.ProjectStatuses (
        Id              tinyint         NOT NULL CONSTRAINT PK_ProjectStatuses PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_ProjectStatuses_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_ProjectStatuses_IsActive DEFAULT (1),
        AllowsApplications bit          NOT NULL CONSTRAINT DF_ProjectStatuses_AllowsApps DEFAULT (0),

        CONSTRAINT UQ_ProjectStatuses_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.ProjectStatuses';
END


-- 1.2 Niveles de Complejidad de Proyecto
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[ProjectComplexityLevels]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.ProjectComplexityLevels (
        Id              tinyint         NOT NULL CONSTRAINT PK_ProjectComplexityLevels PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        ExperienceMultiplier decimal(3,2) NOT NULL CONSTRAINT DF_ComplexityLevels_Multiplier DEFAULT (1.0),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_ComplexityLevels_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_ComplexityLevels_IsActive DEFAULT (1),

        CONSTRAINT UQ_ProjectComplexityLevels_Code UNIQUE (Code),
        CONSTRAINT CK_ComplexityLevels_Multiplier CHECK (ExperienceMultiplier BETWEEN 0.5 AND 3.0)
    );
    PRINT '  ✓ config.ProjectComplexityLevels';
END


-- 1.3 Estados de Postulación
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[ApplicationStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.ApplicationStatuses (
        Id              tinyint         NOT NULL CONSTRAINT PK_ApplicationStatuses PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        RequiresReviewNotes bit         NOT NULL CONSTRAINT DF_AppStatuses_RequiresNotes DEFAULT (0),
        IsFinalState    bit             NOT NULL CONSTRAINT DF_AppStatuses_IsFinal DEFAULT (0),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_AppStatuses_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_AppStatuses_IsActive DEFAULT (1),

        CONSTRAINT UQ_ApplicationStatuses_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.ApplicationStatuses';
END


-- 1.4 Estados de Asignación
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[AssignmentStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.AssignmentStatuses (
        Id              tinyint         NOT NULL CONSTRAINT PK_AssignmentStatuses PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        IsFinalState    bit             NOT NULL CONSTRAINT DF_AssignStatuses_IsFinal DEFAULT (0),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_AssignStatuses_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_AssignStatuses_IsActive DEFAULT (1),

        CONSTRAINT UQ_AssignmentStatuses_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.AssignmentStatuses';
END


-- 1.5 Niveles de Dominio de Habilidades
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[SkillLevels]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.SkillLevels (
        Id              tinyint         NOT NULL CONSTRAINT PK_SkillLevels PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(400)   NULL,
        MinYearsExperience tinyint      NULL,
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_SkillLevels_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_SkillLevels_IsActive DEFAULT (1),

        CONSTRAINT UQ_SkillLevels_Code UNIQUE (Code),
        CONSTRAINT CK_SkillLevels_Id CHECK (Id BETWEEN 1 AND 5)
    );
    PRINT '  ✓ config.SkillLevels';
END


-- 1.6 Tipos de Habilidades
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[SkillTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.SkillTypes (
        Id              tinyint         NOT NULL CONSTRAINT PK_SkillTypes PRIMARY KEY IDENTITY(1,1),
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_SkillTypes_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_SkillTypes_IsActive DEFAULT (1),

        CONSTRAINT UQ_SkillTypes_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.SkillTypes';
END


-- 1.7 Categorías de Habilidades (jerárquica — self-referencing FK)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[SkillCategories]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.SkillCategories (
        Id              int             NOT NULL CONSTRAINT PK_SkillCategories PRIMARY KEY IDENTITY(1,1),
        Code            nvarchar(40)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        ParentCategoryId int            NULL,
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_SkillCategories_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_SkillCategories_IsActive DEFAULT (1),

        CONSTRAINT UQ_SkillCategories_Code UNIQUE (Code),
        CONSTRAINT FK_SkillCategories_Parent
            FOREIGN KEY (ParentCategoryId) REFERENCES config.SkillCategories(Id)
    );
    PRINT '  ✓ config.SkillCategories';
END


-- 1.8 Fuentes de Evaluación
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[EvaluationSources]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.EvaluationSources (
        Id              tinyint         NOT NULL CONSTRAINT PK_EvaluationSources PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        IsAutomated     bit             NOT NULL CONSTRAINT DF_EvalSources_IsAutomated DEFAULT (0),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_EvalSources_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_EvalSources_IsActive DEFAULT (1),

        CONSTRAINT UQ_EvaluationSources_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.EvaluationSources';
END


-- 1.9 Puntajes de Contribución
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[ContributionScores]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.ContributionScores (
        Id              tinyint         NOT NULL CONSTRAINT PK_ContributionScores PRIMARY KEY,
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        ExperienceBonus decimal(3,2)    NOT NULL CONSTRAINT DF_ContribScores_Bonus DEFAULT (0.0),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_ContribScores_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_ContribScores_IsActive DEFAULT (1),

        CONSTRAINT UQ_ContributionScores_Code UNIQUE (Code),
        CONSTRAINT CK_ContributionScores_Id CHECK (Id BETWEEN 1 AND 5)
    );
    PRINT '  ✓ config.ContributionScores';
END


-- 1.10 Tipos de Acciones del Agente
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[AgentActionTypes]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.AgentActionTypes (
        Id              int             NOT NULL CONSTRAINT PK_AgentActionTypes PRIMARY KEY IDENTITY(1,1),
        Code            nvarchar(40)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        RequiresApproval bit            NOT NULL CONSTRAINT DF_AgentActionTypes_RequiresApproval DEFAULT (1),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_AgentActionTypes_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_AgentActionTypes_IsActive DEFAULT (1),

        CONSTRAINT UQ_AgentActionTypes_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.AgentActionTypes';
END


-- 1.11 Estados de Acciones del Agente
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[AgentActionStatuses]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.AgentActionStatuses (
        Id              tinyint         NOT NULL CONSTRAINT PK_AgentActionStatuses PRIMARY KEY IDENTITY(1,1),
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        IsFinalState    bit             NOT NULL CONSTRAINT DF_AgentActStatuses_IsFinal DEFAULT (0),
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_AgentActStatuses_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_AgentActStatuses_IsActive DEFAULT (1),

        CONSTRAINT UQ_AgentActionStatuses_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.AgentActionStatuses';
END


-- 1.12 Niveles de Seniority
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[config].[SeniorityLevels]') AND type in (N'U'))
BEGIN
    CREATE TABLE config.SeniorityLevels (
        Id              tinyint         NOT NULL CONSTRAINT PK_SeniorityLevels PRIMARY KEY IDENTITY(1,1),
        Code            nvarchar(20)    NOT NULL,
        Name            nvarchar(80)    NOT NULL,
        Description     nvarchar(200)   NULL,
        MinYearsExperience tinyint      NOT NULL CONSTRAINT DF_SeniorityLevels_MinYears DEFAULT (0),
        MaxYearsExperience tinyint      NULL,
        DisplayOrder    tinyint         NOT NULL CONSTRAINT DF_SeniorityLevels_DisplayOrder DEFAULT (0),
        IsActive        bit             NOT NULL CONSTRAINT DF_SeniorityLevels_IsActive DEFAULT (1),

        CONSTRAINT UQ_SeniorityLevels_Code UNIQUE (Code)
    );
    PRINT '  ✓ config.SeniorityLevels';
END


PRINT '';
PRINT '==========================================';
PRINT '1) Tablas de Configuración creadas';
PRINT '==========================================';


/* ==========================================================
   2) TABLAS IAM  (Identity & Access Management)
   ========================================================== */

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

CREATE UNIQUE INDEX UX_Users_Org_Email
ON iam.Users(OrganizationId, Email)
WHERE IsDeleted = 0;

CREATE INDEX IX_Users_Org_IsActive
ON iam.Users(OrganizationId, IsActive)
INCLUDE (Email, FirstName, LastName);


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

CREATE UNIQUE INDEX UX_Roles_Org_Name
ON iam.Roles(OrganizationId, Name)
WHERE IsDeleted = 0;


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

CREATE INDEX IX_UserRoles_Org_User
ON iam.UserRoles(OrganizationId, UserId);

PRINT '==========================================';
PRINT '2) Tablas IAM creadas';
PRINT '==========================================';


/* ==========================================================
   3) TABLAS TALENT  (Perfiles, Skills, Certificaciones)
   ========================================================== */

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

CREATE INDEX IX_EmployeeProfiles_Org
ON talent.EmployeeProfiles(OrganizationId)
WHERE IsDeleted = 0;


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

CREATE UNIQUE INDEX UX_Skills_Org_Name
ON talent.Skills(OrganizationId, Name)
WHERE IsDeleted = 0;


CREATE TABLE talent.EmployeeSkills (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_EmployeeSkills PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    SkillId         uniqueidentifier NOT NULL,

    Level           tinyint          NOT NULL, -- FK a config.SkillLevels
    EvidenceUrl     nvarchar(400)    NULL,
    LastValidatedAt datetime2(3)     NULL,
    ValidatedByUserId uniqueidentifier NULL, -- Auditoría: si lo validó el Sistema (Agente) o un Humano
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
    CONSTRAINT FK_EmployeeSkills_Level
        FOREIGN KEY (Level) REFERENCES config.SkillLevels(Id)
);

CREATE UNIQUE INDEX UX_EmployeeSkills_Org_User_Skill
ON talent.EmployeeSkills(OrganizationId, UserId, SkillId)
WHERE IsDeleted = 0;

CREATE INDEX IX_EmployeeSkills_Org_Skill
ON talent.EmployeeSkills(OrganizationId, SkillId)
INCLUDE (UserId, Level)
WHERE IsDeleted = 0;


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

CREATE INDEX IX_Certifications_Org_User
ON talent.Certifications(OrganizationId, UserId)
WHERE IsDeleted = 0;

PRINT '==========================================';
PRINT '3) Tablas Talent creadas';
PRINT '==========================================';


/* ==========================================================
   4) TABLAS PROJECTS
   ========================================================== */

CREATE TABLE projects.Projects (
    Id               uniqueidentifier NOT NULL CONSTRAINT PK_Projects PRIMARY KEY,
    OrganizationId   uniqueidentifier NOT NULL,

    Code             nvarchar(40)     NULL,
    Name             nvarchar(160)    NOT NULL,
    Description      nvarchar(max)    NULL,

    StartDate        date             NULL,
    EndDate          date             NULL,

    ComplexityLevel  tinyint          NOT NULL CONSTRAINT DF_Projects_Complexity DEFAULT(1), -- Dificultad para cálculo de experiencia
    Status           tinyint          NOT NULL, -- FK a config.ProjectStatuses

    CreatedAt        datetime2(3)     NOT NULL CONSTRAINT DF_Projects_CreatedAt DEFAULT (sysutcdatetime()),
    CreatedByUserId  uniqueidentifier NULL,
    UpdatedAt        datetime2(3)     NULL,
    UpdatedByUserId  uniqueidentifier NULL,
    IsDeleted        bit              NOT NULL CONSTRAINT DF_Projects_IsDeleted DEFAULT (0),

    CONSTRAINT FK_Projects_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_Projects_Status
        FOREIGN KEY (Status) REFERENCES config.ProjectStatuses(Id),
    CONSTRAINT FK_Projects_Complexity
        FOREIGN KEY (ComplexityLevel) REFERENCES config.ProjectComplexityLevels(Id)
);

CREATE INDEX IX_Projects_Org_Status
ON projects.Projects(OrganizationId, Status)
INCLUDE (Name, StartDate, EndDate)
WHERE IsDeleted = 0;

CREATE UNIQUE INDEX UX_Projects_Org_Code
ON projects.Projects(OrganizationId, Code)
WHERE Code IS NOT NULL AND IsDeleted = 0;


CREATE TABLE projects.ProjectSkillRequirements (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectSkillRequirements PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    SkillId         uniqueidentifier NOT NULL,

    RequiredLevel   tinyint          NOT NULL, -- FK a config.SkillLevels
    IsMandatory     bit              NOT NULL CONSTRAINT DF_ProjectSkillRequirements_IsMandatory DEFAULT (1),

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectSkillRequirements_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectSkillRequirements_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectSkillRequirements_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectSkillRequirements_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT FK_ProjectSkillRequirements_Skills
        FOREIGN KEY (SkillId) REFERENCES talent.Skills(Id),
    CONSTRAINT FK_ProjectSkillRequirements_Level
        FOREIGN KEY (RequiredLevel) REFERENCES config.SkillLevels(Id)
);

CREATE UNIQUE INDEX UX_ProjectSkillRequirements_Org_Project_Skill
ON projects.ProjectSkillRequirements(OrganizationId, ProjectId, SkillId)
WHERE IsDeleted = 0;

CREATE INDEX IX_ProjectSkillRequirements_Org_Skill
ON projects.ProjectSkillRequirements(OrganizationId, SkillId)
INCLUDE (ProjectId, RequiredLevel, IsMandatory)
WHERE IsDeleted = 0;


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

CREATE UNIQUE INDEX UX_ProjectRoles_Org_Project_Name
ON projects.ProjectRoles(OrganizationId, ProjectId, Name)
WHERE IsDeleted = 0;


CREATE TABLE projects.ProjectApplications (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectApplications PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,

    Motivation      nvarchar(800)    NULL,
    Status          tinyint          NOT NULL, -- FK a config.ApplicationStatuses

    ReviewedByUserId uniqueidentifier NULL,
    ReviewedAt       datetime2(3)     NULL,

    ReviewNotes     nvarchar(500)    NULL, -- Feedback de rechazo o aprobación

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
    CONSTRAINT FK_ProjectApplications_Status
        FOREIGN KEY (Status) REFERENCES config.ApplicationStatuses(Id)
);

CREATE UNIQUE INDEX UX_ProjectApplications_Org_Project_User
ON projects.ProjectApplications(OrganizationId, ProjectId, UserId)
WHERE IsDeleted = 0;

CREATE INDEX IX_ProjectApplications_Org_Project_Status
ON projects.ProjectApplications(OrganizationId, ProjectId, Status)
INCLUDE (UserId, ReviewedAt)
WHERE IsDeleted = 0;


CREATE TABLE projects.ProjectAssignments (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectAssignments PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    ProjectRoleId   uniqueidentifier NULL,

    AssignedByUserId uniqueidentifier NOT NULL,
    AssignedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectAssignments_AssignedAt DEFAULT (sysutcdatetime()),

    Status          tinyint          NOT NULL, -- FK a config.AssignmentStatuses
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
    CONSTRAINT FK_ProjectAssignments_Status
        FOREIGN KEY (Status) REFERENCES config.AssignmentStatuses(Id)
);

CREATE UNIQUE INDEX UX_ProjectAssignments_Org_Project_User_Active
ON projects.ProjectAssignments(OrganizationId, ProjectId, UserId)
WHERE IsDeleted = 0 AND Status = 1;

CREATE INDEX IX_ProjectAssignments_Org_Project_Status
ON projects.ProjectAssignments(OrganizationId, ProjectId, Status)
INCLUDE (UserId, ProjectRoleId, AssignedAt)
WHERE IsDeleted = 0;

PRINT '==========================================';
PRINT '4) Tablas Projects creadas';
PRINT '==========================================';


/* ==========================================================
   5) HISTORIAL / EVALUACIÓN DE HABILIDADES
   ========================================================== */

CREATE TABLE projects.ProjectParticipation (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ProjectParticipation PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    ProjectId       uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,

    RoleName        nvarchar(80)     NULL,
    ContributionScore tinyint        NULL, -- FK a config.ContributionScores

    FeedbackComments nvarchar(max)   NULL, -- Texto para NLP del Agente Inteligente

    CompletedAt     datetime2(3)     NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ProjectParticipation_CreatedAt DEFAULT (sysutcdatetime()),
    IsDeleted       bit              NOT NULL CONSTRAINT DF_ProjectParticipation_IsDeleted DEFAULT (0),

    CONSTRAINT FK_ProjectParticipation_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
    CONSTRAINT FK_ProjectParticipation_Projects
        FOREIGN KEY (ProjectId) REFERENCES projects.Projects(Id),
    CONSTRAINT FK_ProjectParticipation_Users
        FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
    CONSTRAINT FK_ProjectParticipation_ContributionScore
        FOREIGN KEY (ContributionScore) REFERENCES config.ContributionScores(Id)
);

CREATE UNIQUE INDEX UX_ProjectParticipation_Org_Project_User
ON projects.ProjectParticipation(OrganizationId, ProjectId, UserId)
WHERE IsDeleted = 0;


CREATE TABLE talent.SkillEvaluations (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_SkillEvaluations PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    UserId          uniqueidentifier NOT NULL,
    SkillId         uniqueidentifier NOT NULL,

    Source          tinyint          NOT NULL, -- FK a config.EvaluationSources
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
    CONSTRAINT FK_SkillEvaluations_Source
        FOREIGN KEY (Source) REFERENCES config.EvaluationSources(Id),
    CONSTRAINT CK_SkillEvaluations_DeltaLevel CHECK (DeltaLevel BETWEEN -5 AND 5)
);

CREATE INDEX IX_SkillEvaluations_Org_User_Skill_Date
ON talent.SkillEvaluations(OrganizationId, UserId, SkillId, CreatedAt DESC);

PRINT '==========================================';
PRINT '5) Tablas de Historial/Evaluación creadas';
PRINT '==========================================';


/* ==========================================================
   6) REPORTING  (BI básico & Agente AI)
   ========================================================== */

CREATE TABLE reporting.ReportSnapshots (
    Id              uniqueidentifier NOT NULL CONSTRAINT PK_ReportSnapshots PRIMARY KEY,
    OrganizationId  uniqueidentifier NOT NULL,
    SnapshotDate    date             NOT NULL,
    JsonPayload     nvarchar(max)    NOT NULL,

    CreatedAt       datetime2(3)     NOT NULL CONSTRAINT DF_ReportSnapshots_CreatedAt DEFAULT (sysutcdatetime()),

    CONSTRAINT FK_ReportSnapshots_Organizations
        FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
);

CREATE UNIQUE INDEX UX_ReportSnapshots_Org_Date
ON reporting.ReportSnapshots(OrganizationId, SnapshotDate);


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

CREATE INDEX IX_RecommendationRules_Org_Active
ON reporting.RecommendationRules(OrganizationId, IsActive)
WHERE IsDeleted = 0;


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

CREATE INDEX IX_RecommendationLogs_Org_Date
ON reporting.RecommendationLogs(OrganizationId, GeneratedAt DESC);


-- Auditoría de acciones del agente (HITL - Human In The Loop)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[reporting].[AgentActions]') AND type in (N'U'))
BEGIN
    CREATE TABLE reporting.AgentActions (
        Id                  uniqueidentifier NOT NULL CONSTRAINT PK_AgentActions PRIMARY KEY,
        OrganizationId      uniqueidentifier NOT NULL,
        ActionType          nvarchar(80)     NOT NULL, -- SKILL_VALIDATION, PROJECT_MATCHING, etc.
        Description         nvarchar(500)    NOT NULL,
        InputData           nvarchar(max)    NOT NULL, -- JSON
        OutputData          nvarchar(max)    NOT NULL, -- JSON
        Status              nvarchar(40)     NOT NULL, -- SUCCESS, FAILED, PENDING_APPROVAL, APPROVED, REJECTED
        ExecutedByUserId    uniqueidentifier NULL,     -- NULL si es automático
        ApprovedByUserId    uniqueidentifier NULL,     -- HITL: usuario que aprobó/rechazó
        CreatedAt           datetime2(3)     NOT NULL CONSTRAINT DF_AgentActions_CreatedAt DEFAULT (sysutcdatetime()),
        ApprovedAt          datetime2(3)     NULL,

        CONSTRAINT FK_AgentActions_Organizations
            FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT FK_AgentActions_ExecutedBy
            FOREIGN KEY (ExecutedByUserId) REFERENCES iam.Users(Id),
        CONSTRAINT FK_AgentActions_ApprovedBy
            FOREIGN KEY (ApprovedByUserId) REFERENCES iam.Users(Id)
    );

    CREATE INDEX IX_AgentActions_Org_Status_Date
    ON reporting.AgentActions(OrganizationId, Status, CreatedAt DESC);

    CREATE INDEX IX_AgentActions_Org_Type
    ON reporting.AgentActions(OrganizationId, ActionType)
    INCLUDE (Status, CreatedAt);

    PRINT '  ✓ reporting.AgentActions';
END



-- Configuración del agente por organización
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[reporting].[AgentConfiguration]') AND type in (N'U'))
BEGIN
    CREATE TABLE reporting.AgentConfiguration (
        OrganizationId              uniqueidentifier NOT NULL CONSTRAINT PK_AgentConfiguration PRIMARY KEY,
        EnableAutoValidation        bit              NOT NULL CONSTRAINT DF_AgentConfiguration_EnableAutoValidation DEFAULT (1),
        RequireHumanApproval        bit              NOT NULL CONSTRAINT DF_AgentConfiguration_RequireHumanApproval DEFAULT (1),
        MinConfidenceThreshold      decimal(5,2)     NOT NULL CONSTRAINT DF_AgentConfiguration_MinConfidence DEFAULT (70.0),
        MaxCandidatesPerMatch       int              NOT NULL CONSTRAINT DF_AgentConfiguration_MaxCandidates DEFAULT (10),
        EnableBackgroundOptimization bit             NOT NULL CONSTRAINT DF_AgentConfiguration_EnableBgOptimization DEFAULT (1),

        CreatedAt                   datetime2(3)     NOT NULL CONSTRAINT DF_AgentConfiguration_CreatedAt DEFAULT (sysutcdatetime()),
        UpdatedAt                   datetime2(3)     NULL,

        CONSTRAINT FK_AgentConfiguration_Organizations
            FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT CK_AgentConfiguration_Confidence
            CHECK (MinConfidenceThreshold BETWEEN 0 AND 100),
        CONSTRAINT CK_AgentConfiguration_MaxCandidates
            CHECK (MaxCandidatesPerMatch BETWEEN 1 AND 50)
    );
    PRINT '  ✓ reporting.AgentConfiguration';
END


PRINT '';
PRINT '==========================================';
PRINT '6) Tablas Reporting/Agent creadas';
PRINT '==========================================';
PRINT '';
PRINT '══════════════════════════════════════════';
PRINT ' DevManager DDL completo ejecutado OK';
PRINT ' Schemas: config, iam, talent, projects, reporting';
PRINT ' Total tablas: 25';
PRINT '══════════════════════════════════════════';

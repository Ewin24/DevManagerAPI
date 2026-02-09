-- =========================================================
-- DevManager - Tablas de Configuración/Catálogo (config schema)
-- Versión: 1.0
-- Fecha: 9 de Febrero de 2026
-- =========================================================

USE DevManager;
GO

-- Crear esquema config si no existe
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'config')
    EXEC('CREATE SCHEMA config');
GO

PRINT '==========================================';
PRINT 'Creando Tablas de Configuración (Catálogos)';
PRINT '==========================================';

-- =============================================
-- 1. Estados de Proyecto
-- =============================================
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
    PRINT '  ✓ Tabla config.ProjectStatuses creada';
END
GO

-- =============================================
-- 2. Niveles de Complejidad de Proyecto
-- =============================================
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
    PRINT '  ✓ Tabla config.ProjectComplexityLevels creada';
END
GO

-- =============================================
-- 3. Estados de Postulación
-- =============================================
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
    PRINT '  ✓ Tabla config.ApplicationStatuses creada';
END
GO

-- =============================================
-- 4. Estados de Asignación
-- =============================================
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
    PRINT '  ✓ Tabla config.AssignmentStatuses creada';
END
GO

-- =============================================
-- 5. Niveles de Dominio de Habilidades
-- =============================================
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
    PRINT '  ✓ Tabla config.SkillLevels creada';
END
GO

-- =============================================
-- 6. Tipos de Habilidades
-- =============================================
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
    PRINT '  ✓ Tabla config.SkillTypes creada';
END
GO

-- =============================================
-- 7. Categorías de Habilidades
-- =============================================
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
    PRINT '  ✓ Tabla config.SkillCategories creada';
END
GO

-- =============================================
-- 8. Fuentes de Evaluación
-- =============================================
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
    PRINT '  ✓ Tabla config.EvaluationSources creada';
END
GO

-- =============================================
-- 9. Puntajes de Contribución
-- =============================================
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
    PRINT '  ✓ Tabla config.ContributionScores creada';
END
GO

-- =============================================
-- 10. Tipos de Acciones del Agente
-- =============================================
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
    PRINT '  ✓ Tabla config.AgentActionTypes creada';
END
GO

-- =============================================
-- 11. Estados de Acciones del Agente
-- =============================================
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
    PRINT '  ✓ Tabla config.AgentActionStatuses creada';
END
GO

-- =============================================
-- 12. Niveles de Seniority
-- =============================================
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
    PRINT '  ✓ Tabla config.SeniorityLevels creada';
END
GO

PRINT '';
PRINT '==========================================';
PRINT 'Tablas de Configuración creadas exitosamente';
PRINT '==========================================';

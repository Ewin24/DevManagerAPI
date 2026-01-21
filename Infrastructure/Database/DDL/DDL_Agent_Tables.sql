-- =========================================================
-- DevManager - Agent Tables (Reporting Schema)
-- =========================================================

USE DevManager;


-- Tabla para auditoría de acciones del agente (HITL - Human In The Loop)
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

    PRINT 'Tabla reporting.AgentActions creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Tabla reporting.AgentActions ya existe.';
END


-- Tabla para configuración del agente por organización
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

    PRINT 'Tabla reporting.AgentConfiguration creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'Tabla reporting.AgentConfiguration ya existe.';
END
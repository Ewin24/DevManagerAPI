/* =========================================================
   DevManager - Update 002: Cooperative education
   solidarity.EducationPrograms / EducationEnrollments.
   Requires Update 001 (schema solidarity + Associates).
   Idempotent: every statement is guarded. Re-run is a no-op.
   Rollback: 002_cooperative_education_rollback.sql
   ========================================================= */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF OBJECT_ID('solidarity.Associates', 'U') IS NULL
BEGIN
    RAISERROR('Apply 001_solidarity_core.sql first.', 16, 1);
    SET NOEXEC ON;
END
GO

-- ---------- solidarity.EducationPrograms ----------
IF OBJECT_ID('solidarity.EducationPrograms', 'U') IS NULL
BEGIN
    CREATE TABLE solidarity.EducationPrograms (
        Id                uniqueidentifier NOT NULL CONSTRAINT PK_EducationPrograms PRIMARY KEY,
        OrganizationId    uniqueidentifier NOT NULL,
        Name              nvarchar(160)    NOT NULL,
        [Type]            tinyint          NOT NULL, -- 1 Basic, 2 Permanent, 3 Governance, 4 Informal
        RequiredHours     decimal(5,1)     NOT NULL,
        StartDate         date             NOT NULL,
        EndDate           date             NULL,

        CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_EducationPrograms_CreatedAt DEFAULT (sysutcdatetime()),
        CreatedByUserId   uniqueidentifier NULL,
        UpdatedAt         datetime2(3)     NULL,
        UpdatedByUserId   uniqueidentifier NULL,
        IsDeleted         bit              NOT NULL CONSTRAINT DF_EducationPrograms_IsDeleted DEFAULT (0),
        DeletedAt         datetime2(3)     NULL,
        DeletedByUserId   uniqueidentifier NULL,

        CONSTRAINT FK_EducationPrograms_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT CK_EducationPrograms_Type CHECK ([Type] BETWEEN 1 AND 4),
        CONSTRAINT CK_EducationPrograms_Hours CHECK (RequiredHours > 0),
        CONSTRAINT CK_EducationPrograms_Dates CHECK (EndDate IS NULL OR EndDate >= StartDate)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EducationPrograms_Org' AND object_id = OBJECT_ID('solidarity.EducationPrograms'))
    CREATE INDEX IX_EducationPrograms_Org ON solidarity.EducationPrograms(OrganizationId);
GO

-- ---------- solidarity.EducationEnrollments ----------
IF OBJECT_ID('solidarity.EducationEnrollments', 'U') IS NULL
BEGIN
    CREATE TABLE solidarity.EducationEnrollments (
        Id                uniqueidentifier NOT NULL CONSTRAINT PK_EducationEnrollments PRIMARY KEY,
        OrganizationId    uniqueidentifier NOT NULL,
        ProgramId         uniqueidentifier NOT NULL,
        UserId            uniqueidentifier NOT NULL,
        Status            tinyint          NOT NULL, -- 1 Enrolled, 2 Completed, 3 Dropped
        HoursCompleted    decimal(5,1)     NOT NULL CONSTRAINT DF_EducationEnrollments_Hours DEFAULT (0),
        CompletionDate    date             NULL,

        CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_EducationEnrollments_CreatedAt DEFAULT (sysutcdatetime()),
        CreatedByUserId   uniqueidentifier NULL,
        UpdatedAt         datetime2(3)     NULL,
        UpdatedByUserId   uniqueidentifier NULL,
        IsDeleted         bit              NOT NULL CONSTRAINT DF_EducationEnrollments_IsDeleted DEFAULT (0),
        DeletedAt         datetime2(3)     NULL,
        DeletedByUserId   uniqueidentifier NULL,

        CONSTRAINT FK_EducationEnrollments_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT FK_EducationEnrollments_Programs FOREIGN KEY (ProgramId) REFERENCES solidarity.EducationPrograms(Id),
        CONSTRAINT FK_EducationEnrollments_Associates FOREIGN KEY (UserId) REFERENCES solidarity.Associates(UserId),
        CONSTRAINT CK_EducationEnrollments_Status CHECK (Status BETWEEN 1 AND 3),
        CONSTRAINT CK_EducationEnrollments_Hours CHECK (HoursCompleted >= 0),
        CONSTRAINT CK_EducationEnrollments_Completed CHECK (Status <> 2 OR CompletionDate IS NOT NULL)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_EducationEnrollments_Org_Program_User' AND object_id = OBJECT_ID('solidarity.EducationEnrollments'))
    EXEC('CREATE UNIQUE INDEX UX_EducationEnrollments_Org_Program_User
          ON solidarity.EducationEnrollments(OrganizationId, ProgramId, UserId)
          WHERE IsDeleted = 0');
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EducationEnrollments_Org_User' AND object_id = OBJECT_ID('solidarity.EducationEnrollments'))
    CREATE INDEX IX_EducationEnrollments_Org_User ON solidarity.EducationEnrollments(OrganizationId, UserId);
GO

SET NOEXEC OFF;
GO
PRINT 'Update 002_cooperative_education applied.';
GO

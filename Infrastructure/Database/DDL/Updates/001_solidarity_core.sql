/* =========================================================
   DevManager - Update 001: Solidarity core
   Organizations.EntityType, Users document/IsEmployee,
   solidarity.Associates / MembershipPeriods /
   DataPolicyVersions / DataConsents.
   Idempotent: every statement is guarded. Re-run is a no-op.
   Rollback: 001_solidarity_core_rollback.sql
   ========================================================= */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'solidarity') EXEC('CREATE SCHEMA solidarity');
GO

-- ---------- iam.Organizations.EntityType ----------
IF COL_LENGTH('iam.Organizations', 'EntityType') IS NULL
    ALTER TABLE iam.Organizations ADD EntityType tinyint NULL;
GO
IF OBJECT_ID('iam.CK_Organizations_EntityType', 'C') IS NULL
    ALTER TABLE iam.Organizations ADD CONSTRAINT CK_Organizations_EntityType
        CHECK (EntityType IS NULL OR EntityType BETWEEN 1 AND 6);
GO

-- ---------- iam.Users document + employee flag ----------
IF COL_LENGTH('iam.Users', 'DocumentType') IS NULL
    ALTER TABLE iam.Users ADD DocumentType tinyint NULL;
GO
IF COL_LENGTH('iam.Users', 'DocumentNumber') IS NULL
    ALTER TABLE iam.Users ADD DocumentNumber nvarchar(20) NULL;
GO
IF COL_LENGTH('iam.Users', 'IsEmployee') IS NULL
    ALTER TABLE iam.Users ADD IsEmployee bit NOT NULL CONSTRAINT DF_Users_IsEmployee DEFAULT (1);
GO
IF OBJECT_ID('iam.CK_Users_DocumentType', 'C') IS NULL
    ALTER TABLE iam.Users ADD CONSTRAINT CK_Users_DocumentType
        CHECK (DocumentType IS NULL OR DocumentType BETWEEN 1 AND 6);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Users_Org_Document' AND object_id = OBJECT_ID('iam.Users'))
    EXEC('CREATE UNIQUE INDEX UX_Users_Org_Document
          ON iam.Users(OrganizationId, DocumentType, DocumentNumber)
          WHERE DocumentNumber IS NOT NULL AND IsDeleted = 0');
GO

-- ---------- solidarity.Associates ----------
IF OBJECT_ID('solidarity.Associates', 'U') IS NULL
BEGIN
    CREATE TABLE solidarity.Associates (
        UserId            uniqueidentifier NOT NULL CONSTRAINT PK_Associates PRIMARY KEY,
        OrganizationId    uniqueidentifier NOT NULL,
        Status            tinyint          NOT NULL, -- 1 Applicant, 2 Active, 3 Suspended, 4 Withdrawn

        CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_Associates_CreatedAt DEFAULT (sysutcdatetime()),
        CreatedByUserId   uniqueidentifier NULL,
        UpdatedAt         datetime2(3)     NULL,
        UpdatedByUserId   uniqueidentifier NULL,
        IsDeleted         bit              NOT NULL CONSTRAINT DF_Associates_IsDeleted DEFAULT (0),
        DeletedAt         datetime2(3)     NULL,
        DeletedByUserId   uniqueidentifier NULL,

        CONSTRAINT FK_Associates_Users FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
        CONSTRAINT FK_Associates_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT CK_Associates_Status CHECK (Status BETWEEN 1 AND 4)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Associates_Org_Status' AND object_id = OBJECT_ID('solidarity.Associates'))
    CREATE INDEX IX_Associates_Org_Status ON solidarity.Associates(OrganizationId, Status);
GO

-- ---------- solidarity.MembershipPeriods ----------
IF OBJECT_ID('solidarity.MembershipPeriods', 'U') IS NULL
BEGIN
    CREATE TABLE solidarity.MembershipPeriods (
        Id                uniqueidentifier NOT NULL CONSTRAINT PK_MembershipPeriods PRIMARY KEY,
        OrganizationId    uniqueidentifier NOT NULL,
        UserId            uniqueidentifier NOT NULL,
        ApplicationDate   date             NOT NULL,
        AdmissionDate     date             NULL,
        WithdrawalDate    date             NULL,
        WithdrawalReason  nvarchar(400)    NULL,

        CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_MembershipPeriods_CreatedAt DEFAULT (sysutcdatetime()),
        CreatedByUserId   uniqueidentifier NULL,
        UpdatedAt         datetime2(3)     NULL,
        UpdatedByUserId   uniqueidentifier NULL,
        IsDeleted         bit              NOT NULL CONSTRAINT DF_MembershipPeriods_IsDeleted DEFAULT (0),
        DeletedAt         datetime2(3)     NULL,
        DeletedByUserId   uniqueidentifier NULL,

        CONSTRAINT FK_MembershipPeriods_Associates FOREIGN KEY (UserId) REFERENCES solidarity.Associates(UserId),
        CONSTRAINT FK_MembershipPeriods_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT CK_MembershipPeriods_Dates
            CHECK (WithdrawalDate IS NULL OR WithdrawalDate >= ISNULL(AdmissionDate, ApplicationDate))
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_MembershipPeriods_Org_User_Open' AND object_id = OBJECT_ID('solidarity.MembershipPeriods'))
    EXEC('CREATE UNIQUE INDEX UX_MembershipPeriods_Org_User_Open
          ON solidarity.MembershipPeriods(OrganizationId, UserId)
          WHERE WithdrawalDate IS NULL AND IsDeleted = 0');
GO

-- ---------- solidarity.DataPolicyVersions ----------
IF OBJECT_ID('solidarity.DataPolicyVersions', 'U') IS NULL
BEGIN
    CREATE TABLE solidarity.DataPolicyVersions (
        Id                uniqueidentifier NOT NULL CONSTRAINT PK_DataPolicyVersions PRIMARY KEY,
        OrganizationId    uniqueidentifier NOT NULL,
        Version           nvarchar(20)     NOT NULL,
        Purpose           nvarchar(300)    NOT NULL,
        [Text]            nvarchar(max)    NOT NULL,
        EffectiveFrom     date             NOT NULL,

        CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_DataPolicyVersions_CreatedAt DEFAULT (sysutcdatetime()),
        CreatedByUserId   uniqueidentifier NULL,
        UpdatedAt         datetime2(3)     NULL,
        UpdatedByUserId   uniqueidentifier NULL,
        IsDeleted         bit              NOT NULL CONSTRAINT DF_DataPolicyVersions_IsDeleted DEFAULT (0),
        DeletedAt         datetime2(3)     NULL,
        DeletedByUserId   uniqueidentifier NULL,

        CONSTRAINT FK_DataPolicyVersions_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id)
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_DataPolicyVersions_Org_Version' AND object_id = OBJECT_ID('solidarity.DataPolicyVersions'))
    EXEC('CREATE UNIQUE INDEX UX_DataPolicyVersions_Org_Version
          ON solidarity.DataPolicyVersions(OrganizationId, Version)
          WHERE IsDeleted = 0');
GO

-- ---------- solidarity.DataConsents (append-only: never deleted) ----------
IF OBJECT_ID('solidarity.DataConsents', 'U') IS NULL
BEGIN
    CREATE TABLE solidarity.DataConsents (
        Id                uniqueidentifier NOT NULL CONSTRAINT PK_DataConsents PRIMARY KEY,
        OrganizationId    uniqueidentifier NOT NULL,
        UserId            uniqueidentifier NOT NULL,
        PolicyVersionId   uniqueidentifier NOT NULL,
        GrantedAt         datetime2(3)     NOT NULL CONSTRAINT DF_DataConsents_GrantedAt DEFAULT (sysutcdatetime()),
        GrantedByUserId   uniqueidentifier NOT NULL,
        RevokedAt         datetime2(3)     NULL,
        RevokedByUserId   uniqueidentifier NULL,
        RevocationReason  nvarchar(400)    NULL,

        CreatedAt         datetime2(3)     NOT NULL CONSTRAINT DF_DataConsents_CreatedAt DEFAULT (sysutcdatetime()),
        CreatedByUserId   uniqueidentifier NULL,

        CONSTRAINT FK_DataConsents_Users FOREIGN KEY (UserId) REFERENCES iam.Users(Id),
        CONSTRAINT FK_DataConsents_Organizations FOREIGN KEY (OrganizationId) REFERENCES iam.Organizations(Id),
        CONSTRAINT FK_DataConsents_PolicyVersions FOREIGN KEY (PolicyVersionId) REFERENCES solidarity.DataPolicyVersions(Id),
        CONSTRAINT CK_DataConsents_Revocation
            CHECK ((RevokedAt IS NULL AND RevocationReason IS NULL) OR (RevokedAt IS NOT NULL AND RevocationReason IS NOT NULL))
    );
END
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_DataConsents_Org_User_Policy_Active' AND object_id = OBJECT_ID('solidarity.DataConsents'))
    EXEC('CREATE UNIQUE INDEX UX_DataConsents_Org_User_Policy_Active
          ON solidarity.DataConsents(OrganizationId, UserId, PolicyVersionId)
          WHERE RevokedAt IS NULL');
GO

PRINT 'Update 001_solidarity_core applied.';
GO

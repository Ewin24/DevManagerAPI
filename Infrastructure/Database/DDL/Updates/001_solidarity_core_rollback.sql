/* Rollback of 001_solidarity_core.sql (reverse order). Safe to re-run. */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF OBJECT_ID('solidarity.DataConsents', 'U') IS NOT NULL DROP TABLE solidarity.DataConsents;
IF OBJECT_ID('solidarity.DataPolicyVersions', 'U') IS NOT NULL DROP TABLE solidarity.DataPolicyVersions;
IF OBJECT_ID('solidarity.MembershipPeriods', 'U') IS NOT NULL DROP TABLE solidarity.MembershipPeriods;
IF OBJECT_ID('solidarity.Associates', 'U') IS NOT NULL DROP TABLE solidarity.Associates;
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_Users_Org_Document' AND object_id = OBJECT_ID('iam.Users'))
    DROP INDEX UX_Users_Org_Document ON iam.Users;
IF OBJECT_ID('iam.CK_Users_DocumentType', 'C') IS NOT NULL ALTER TABLE iam.Users DROP CONSTRAINT CK_Users_DocumentType;
IF OBJECT_ID('iam.DF_Users_IsEmployee', 'D') IS NOT NULL ALTER TABLE iam.Users DROP CONSTRAINT DF_Users_IsEmployee;
GO
IF COL_LENGTH('iam.Users', 'IsEmployee') IS NOT NULL ALTER TABLE iam.Users DROP COLUMN IsEmployee;
IF COL_LENGTH('iam.Users', 'DocumentNumber') IS NOT NULL ALTER TABLE iam.Users DROP COLUMN DocumentNumber;
IF COL_LENGTH('iam.Users', 'DocumentType') IS NOT NULL ALTER TABLE iam.Users DROP COLUMN DocumentType;
GO

IF OBJECT_ID('iam.CK_Organizations_EntityType', 'C') IS NOT NULL ALTER TABLE iam.Organizations DROP CONSTRAINT CK_Organizations_EntityType;
GO
IF COL_LENGTH('iam.Organizations', 'EntityType') IS NOT NULL ALTER TABLE iam.Organizations DROP COLUMN EntityType;
GO

-- The solidarity schema is shared with update 002; drop it only when empty.
IF EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'solidarity')
   AND NOT EXISTS (SELECT 1 FROM sys.objects WHERE schema_id = SCHEMA_ID('solidarity'))
    EXEC('DROP SCHEMA solidarity');
GO
PRINT 'Rollback 001_solidarity_core applied.';
GO

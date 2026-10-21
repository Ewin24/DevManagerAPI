/* =========================================================
   DevManager - Update 003: Organizations.Nit filtered unique
   Nit is optional; the UNIQUE constraint allowed only one NULL,
   so registering a second organization without NIT failed (500).
   Idempotent: re-run is a no-op.
   Rollback: 003_organizations_nit_filtered_unique_rollback.sql
   ========================================================= */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF EXISTS (SELECT 1 FROM sys.key_constraints
           WHERE name = 'UQ_Organizations_Nit' AND parent_object_id = OBJECT_ID('iam.Organizations'))
    ALTER TABLE iam.Organizations DROP CONSTRAINT UQ_Organizations_Nit;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UQ_Organizations_Nit' AND object_id = OBJECT_ID('iam.Organizations'))
    CREATE UNIQUE INDEX UQ_Organizations_Nit ON iam.Organizations(Nit) WHERE Nit IS NOT NULL;
GO

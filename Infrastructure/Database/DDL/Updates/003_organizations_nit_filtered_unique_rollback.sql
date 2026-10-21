/* =========================================================
   DevManager - Rollback 003: restore UNIQUE constraint on Organizations.Nit
   Fails if more than one organization has a NULL Nit.
   ========================================================= */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = 'UQ_Organizations_Nit' AND object_id = OBJECT_ID('iam.Organizations')
             AND has_filter = 1)
    DROP INDEX UQ_Organizations_Nit ON iam.Organizations;
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints
               WHERE name = 'UQ_Organizations_Nit' AND parent_object_id = OBJECT_ID('iam.Organizations'))
    ALTER TABLE iam.Organizations ADD CONSTRAINT UQ_Organizations_Nit UNIQUE (Nit);
GO

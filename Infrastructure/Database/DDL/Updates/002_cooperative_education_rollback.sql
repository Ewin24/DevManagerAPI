/* Rollback of 002_cooperative_education.sql (reverse order). Safe to re-run. */
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO

IF OBJECT_ID('solidarity.EducationEnrollments', 'U') IS NOT NULL DROP TABLE solidarity.EducationEnrollments;
IF OBJECT_ID('solidarity.EducationPrograms', 'U') IS NOT NULL DROP TABLE solidarity.EducationPrograms;
GO
PRINT 'Rollback 002_cooperative_education applied.';
GO

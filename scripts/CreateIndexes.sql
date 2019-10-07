USE [YOUR_DATABASE_NAME]

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [Name] = 'IX_GeoNames_Name' AND object_id = OBJECT_ID('GeoNames'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_GeoNames_Name
	ON [dbo].[GeoNames] ([Name])
	INCLUDE ([FeatureClass],[FeatureCode],[CountryCode],[Population])
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [Name] = 'IX_GeoNames_Name_CountryCode_Population' AND object_id = OBJECT_ID('GeoNames'))
BEGIN
	CREATE NONCLUSTERED INDEX [IX_GeoNames_Name_CountryCode_Population]
	ON [dbo].[GeoNames] ([Name] ASC, [CountryCode] ASC, [Population] ASC)
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [Name] = 'IX_AlternateNames_GeoNameId_IsColloquial_IsHistoric' AND object_id = OBJECT_ID('AlternateNames'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_AlternateNames_GeoNameId_IsColloquial_IsHistoric
	ON [dbo].[AlternateNames] ([GeoNameId],[IsColloquial],[IsHistoric])
	INCLUDE ([ISOLanguage], [AlternateName])
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE [Name] = 'IX_AlternateNames_ISOLanguage_IsColloquial_IsHistoric_AlternateName' AND object_id = OBJECT_ID('AlternateNames'))
BEGIN
	CREATE NONCLUSTERED INDEX IX_AlternateNames_ISOLanguage_IsColloquial_IsHistoric_AlternateName
	ON [dbo].[AlternateNames] ([ISOLanguage],[IsColloquial],[IsHistoric],[AlternateName])
	INCLUDE ([GeoNameId])
END
GO
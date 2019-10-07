USE [YOUR_DATABASE_NAME]

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE [TABLE_SCHEMA] = 'dbo' AND [TABLE_NAME] = 'GeoNames')
BEGIN
	CREATE TABLE [dbo].[GeoNames] (
		[Id] [int] NOT NULL,
		[Name] [nvarchar](200) NULL,
		[NameASCII] [varchar](200) NULL,
		[Latitude] [float] NULL,
		[Longitude] [float] NULL,
		[FeatureClass] [char](1) NULL,
		[FeatureCode] [varchar](10) NULL,
		[CountryCode] [varchar](2) NULL,
		[Population] [bigint] NULL,
		[Elevation] [int] NULL,
		[Dem] [int] NULL,
		[Timezone] [varchar](40) NULL,
		[ModificationDate] [datetime] NULL,
		CONSTRAINT [PK_GeoNames] PRIMARY KEY CLUSTERED ([Id] ASC)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE [TABLE_SCHEMA] = 'dbo' AND [TABLE_NAME] = 'CountryInfo')
BEGIN
	CREATE TABLE [dbo].[CountryInfo] (
		[ISO_Alpha2] [varchar](2) NOT NULL,
		[ISO_Alpha3] [varchar](3) NOT NULL,
		[ISO_Numeric] [varchar](3) NOT NULL,
		[FIPS] [varchar](20) NOT NULL,
		[Country] [nvarchar](200) NOT NULL,
		[Capital] [nvarchar](200) NOT NULL,
		[Area] [int] NOT NULL,
		[Population] [int] NOT NULL,
		[Continent] [varchar](2) NOT NULL,
		[Tld] [varchar](3) NOT NULL,
		[CurrencyCode] [varchar](3) NOT NULL,
		[CurrencyName] [nvarchar](50) NOT NULL,
		[Phone] [varchar](50) NOT NULL,
		[PostalCodeFormat] [varchar](100) NULL,
		[PostalCodeRegex] [varchar](255) NULL,
		[GeoNameId] [int] NOT NULL,
		[EquivalentFipsCode] [varchar](20) NOT NULL,
		CONSTRAINT [PK_CountryInfo] PRIMARY KEY CLUSTERED ([ISO_Alpha2] ASC, [ISO_Alpha3] ASC)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE [TABLE_SCHEMA] = 'dbo' AND [TABLE_NAME] = 'AlternateNames')
BEGIN
	CREATE TABLE [dbo].[AlternateNames] (
		[Id] [int] NOT NULL,
		[GeoNameId] [int] NOT NULL,
		[ISOLanguage] [varchar](7) NULL,
		[AlternateName] [nvarchar](400) NULL,
		[IsPreferredName] [bit] NOT NULL,
		[IsShortName] [bit] NOT NULL,
		[IsColloquial] [bit] NOT NULL,
		[IsHistoric] [bit] NOT NULL,
		[FromDate] [nvarchar](1000) NULL,
		[ToDate] [nvarchar](1000) NULL,
		CONSTRAINT [PK_AlternateNames] PRIMARY KEY CLUSTERED ([Id] ASC)
		WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
END
GO
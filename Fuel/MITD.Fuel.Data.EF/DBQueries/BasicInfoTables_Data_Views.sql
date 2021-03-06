USE [StorageSpace]
GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPaneCount' , @level0type=N'SCHEMA',@level0name=N'BasicInfo', @level1type=N'VIEW',@level1name=N'SharedGoodView'

GO
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'BasicInfo', @level1type=N'VIEW',@level1name=N'SharedGoodView'

GO
ALTER TABLE [BasicInfo].[Vessel] DROP CONSTRAINT [FK_BasicInfo.Vessel_BasicInfo.Company_CompanyId]
GO
ALTER TABLE [BasicInfo].[User] DROP CONSTRAINT [FK_BasicInfo.User_BasicInfo.Company_CompanyId]
GO
ALTER TABLE [BasicInfo].[Tank] DROP CONSTRAINT [FK_BasicInfo.Tank_BasicInfo.Vessel_VesselInCompanyId]
GO
ALTER TABLE [BasicInfo].[SharedGood] DROP CONSTRAINT [FK_dbo.SharedGoods_BasicInfo.Unit_MainUnitId]
GO
ALTER TABLE [BasicInfo].[Good] DROP CONSTRAINT [FK_BasicInfo.Good_dbo.SharedGoods_SharedGoodId]
GO
ALTER TABLE [BasicInfo].[Good] DROP CONSTRAINT [FK_BasicInfo.Good_BasicInfo.Unit_Unit_Id]
GO
ALTER TABLE [BasicInfo].[Good] DROP CONSTRAINT [FK_BasicInfo.Good_BasicInfo.Company_CompanyId]
GO
ALTER TABLE [BasicInfo].[CompanyGoodUnit] DROP CONSTRAINT [FK_BasicInfo.CompanyGoodUnit_BasicInfo.Good_GoodId]
GO
ALTER TABLE [BasicInfo].[CompanyGoodUnit] DROP CONSTRAINT [FK_BasicInfo.CompanyGoodUnit_BasicInfo.CompanyGoodUnit_ParentId]
GO
ALTER TABLE [BasicInfo].[Vessel] DROP CONSTRAINT [DF__Vessel__IsActive__2739D489]
GO
/****** Object:  Index [IX_CompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_CompanyId] ON [BasicInfo].[Vessel]
GO
/****** Object:  Index [IX_CompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_CompanyId] ON [BasicInfo].[User]
GO
/****** Object:  Index [IX_VesselInCompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_VesselInCompanyId] ON [BasicInfo].[Tank]
GO
/****** Object:  Index [IX_MainUnitId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_MainUnitId] ON [BasicInfo].[SharedGood]
GO
/****** Object:  Index [IX_Unit_Id]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_Unit_Id] ON [BasicInfo].[Good]
GO
/****** Object:  Index [IX_SharedGoodId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_SharedGoodId] ON [BasicInfo].[Good]
GO
/****** Object:  Index [IX_CompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_CompanyId] ON [BasicInfo].[Good]
GO
/****** Object:  Index [IX_ParentId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_ParentId] ON [BasicInfo].[CompanyGoodUnit]
GO
/****** Object:  Index [IX_GoodId]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP INDEX [IX_GoodId] ON [BasicInfo].[CompanyGoodUnit]
GO
/****** Object:  View [BasicInfo].[CurrencyView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[CurrencyView]
GO
/****** Object:  View [BasicInfo].[ActivityLocationView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[ActivityLocationView]
GO
/****** Object:  View [BasicInfo].[CompanyGoodUnitView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[CompanyGoodUnitView]
GO
/****** Object:  View [BasicInfo].[CompanyView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[CompanyView]
GO
/****** Object:  View [BasicInfo].[TankView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[TankView]
GO
/****** Object:  View [BasicInfo].[SharedGoodView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[SharedGoodView]
GO
/****** Object:  View [BasicInfo].[GoodPartyAssignmentView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[GoodPartyAssignmentView]
GO
/****** Object:  View [BasicInfo].[GoodView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[GoodView]
GO
/****** Object:  View [BasicInfo].[UserView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[UserView]
GO
/****** Object:  View [BasicInfo].[UnitView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[UnitView]
GO
/****** Object:  View [BasicInfo].[VesselView]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP VIEW [BasicInfo].[VesselView]
GO
/****** Object:  Table [BasicInfo].[Vessel]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[Vessel]
GO
/****** Object:  Table [BasicInfo].[User]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[User]
GO
/****** Object:  Table [BasicInfo].[Unit]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[Unit]
GO
/****** Object:  Table [BasicInfo].[Tank]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[Tank]
GO
/****** Object:  Table [BasicInfo].[SharedGood]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[SharedGood]
GO
/****** Object:  Table [BasicInfo].[GoodPartyAssignment]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[GoodPartyAssignment]
GO
/****** Object:  Table [BasicInfo].[Good]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[Good]
GO
/****** Object:  Table [BasicInfo].[Currency]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[Currency]
GO
/****** Object:  Table [BasicInfo].[CompanyGoodUnit]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[CompanyGoodUnit]
GO
/****** Object:  Table [BasicInfo].[Company]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[Company]
GO
/****** Object:  Table [BasicInfo].[ActivityLocation]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP TABLE [BasicInfo].[ActivityLocation]
GO
/****** Object:  Schema [BasicInfo]    Script Date: 7/4/2014 11:50:21 AM ******/
DROP SCHEMA [BasicInfo]
GO
/****** Object:  Schema [BasicInfo]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE SCHEMA [BasicInfo]
GO
/****** Object:  Table [BasicInfo].[ActivityLocation]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[ActivityLocation](
	[Id] [bigint] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](200) NULL,
	[Abbreviation] [nvarchar](50) NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_BasicInfo.ActivityLocation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[Company]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[Company](
	[Id] [bigint] NOT NULL,
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](150) NULL,
 CONSTRAINT [PK_BasicInfo.Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[CompanyGoodUnit]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[CompanyGoodUnit](
	[Id] [bigint] NOT NULL,
	[AliasName] [nvarchar](200) NULL,
	[Coefficient] [decimal](18, 2) NOT NULL,
	[GoodId] [bigint] NOT NULL,
	[ParentId] [bigint] NULL,
 CONSTRAINT [PK_BasicInfo.CompanyGoodUnit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[Currency]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[Currency](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Abbreviation] [nvarchar](max) NULL,
 CONSTRAINT [PK_BasicInfo.Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[Good]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[Good](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Code] [nvarchar](max) NULL,
	[CompanyId] [bigint] NOT NULL,
	[SharedGoodId] [bigint] NOT NULL,
	[Unit_Id] [bigint] NULL,
 CONSTRAINT [PK_BasicInfo.Good] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[GoodPartyAssignment]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[GoodPartyAssignment](
	[Id] [bigint] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[GoodId] [bigint] NOT NULL,
	[CompanyId] [bigint] NOT NULL,
	[CompanyName] [nvarchar](max) NULL,
 CONSTRAINT [PK_BasicInfo.GoodPartyAssignment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[SharedGood]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[SharedGood](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NULL,
	[Code] [nvarchar](max) NULL,
	[MainUnitId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.SharedGoods] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[Tank]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[Tank](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[VesselInCompanyId] [bigint] NOT NULL,
 CONSTRAINT [PK_BasicInfo.Tank] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[Unit]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[Unit](
	[Id] [bigint] NOT NULL,
	[Name] [nvarchar](200) NULL,
 CONSTRAINT [PK_BasicInfo.Unit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[User]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[User](
	[Id] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
	[Name] [nvarchar](200) NULL,
	[CompanyId] [bigint] NOT NULL,
 CONSTRAINT [PK_BasicInfo.User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [BasicInfo].[Vessel]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [BasicInfo].[Vessel](
	[Id] [bigint] NOT NULL,
	[Code] [nvarchar](max) NULL,
	[Name] [nvarchar](200) NULL,
	[Description] [nvarchar](2000) NULL,
	[CompanyId] [bigint] NOT NULL,
	[VesselState] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_BasicInfo.Vessel] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  View [BasicInfo].[VesselView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[VesselView]
	AS SELECT * FROM BasicInfo.Vessel
GO
/****** Object:  View [BasicInfo].[UnitView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[UnitView]
	AS SELECT * FROM BasicInfo.Unit
GO
/****** Object:  View [BasicInfo].[UserView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[UserView]
	AS SELECT * FROM BasicInfo.[User]
GO
/****** Object:  View [BasicInfo].[GoodView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[GoodView]
	AS SELECT * FROM BasicInfo.Good
GO
/****** Object:  View [BasicInfo].[GoodPartyAssignmentView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[GoodPartyAssignmentView]
	AS SELECT * FROM BasicInfo.GoodPartyAssignment
GO
/****** Object:  View [BasicInfo].[SharedGoodView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[SharedGoodView]
AS
SELECT   Id, Name, Code, MainUnitId
FROM      BasicInfo.SharedGood

GO
/****** Object:  View [BasicInfo].[TankView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[TankView]
	AS SELECT * FROM BasicInfo.Tank
GO
/****** Object:  View [BasicInfo].[CompanyView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[CompanyView]
	AS SELECT * FROM BasicInfo.Company
GO
/****** Object:  View [BasicInfo].[CompanyGoodUnitView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[CompanyGoodUnitView]
	AS SELECT * FROM BasicInfo.CompanyGoodUnit
GO
/****** Object:  View [BasicInfo].[ActivityLocationView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[ActivityLocationView]
	AS SELECT * FROM BasicInfo.ActivityLocation
GO
/****** Object:  View [BasicInfo].[CurrencyView]    Script Date: 7/4/2014 11:50:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [BasicInfo].[CurrencyView]
	AS SELECT * FROM BasicInfo.Currency
GO


INSERT [BasicInfo].[ActivityLocation] ([Id], [Code], [Name], [Abbreviation], [Latitude], [Longitude]) VALUES (1, N'IRBND', N'Bandar Abbas', N'B. Abbas', 122.348670959473, 47.619930267334)
GO
INSERT [BasicInfo].[ActivityLocation] ([Id], [Code], [Name], [Abbreviation], [Latitude], [Longitude]) VALUES (2, N'IRBKM', N'Bandar Khomeini', N'B.I.K', 122.396670959473, 47.999930267334)
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (1, N'SAPID', N'SAPID')
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (2, N'HAFIZ', N'HAFIZ')
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (3, N'IMSENGCO', N'IMSENGCO')
GO
INSERT [BasicInfo].[Company] ([Id], [Code], [Name]) VALUES (4, N'IRISL', N'IRISL')
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (1, N'MT', CAST(1.00 AS Decimal(18, 2)), 1, NULL)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (2, N'Liter', CAST(0.99 AS Decimal(18, 2)), 1, 1)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (3, N'Long Ton', CAST(1200.00 AS Decimal(18, 2)), 1, 2)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (4, N'MT', CAST(1.00 AS Decimal(18, 2)), 2, NULL)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (5, N'Liter', CAST(0.99 AS Decimal(18, 2)), 2, 4)
GO
INSERT [BasicInfo].[CompanyGoodUnit] ([Id], [AliasName], [Coefficient], [GoodId], [ParentId]) VALUES (6, N'Long Ton', CAST(1200.00 AS Decimal(18, 2)), 2, 5)
GO
INSERT [BasicInfo].[Currency] ([Id], [Name], [Abbreviation]) VALUES (1, N'USD', N'USD')
GO
INSERT [BasicInfo].[Currency] ([Id], [Name], [Abbreviation]) VALUES (2, N'EUR', N'EUR')
GO
INSERT [BasicInfo].[Currency] ([Id], [Name], [Abbreviation]) VALUES (3, N'IR Rials', N'IRR')
GO
INSERT [BasicInfo].[Good] ([Id], [Name], [Code], [CompanyId], [SharedGoodId], [Unit_Id]) VALUES (1, N'Heavy Fuel Oil', N'HFO', 1, 1, NULL)
GO
INSERT [BasicInfo].[Good] ([Id], [Name], [Code], [CompanyId], [SharedGoodId], [Unit_Id]) VALUES (2, N'Marine Diesel Oil', N'MDO', 1, 2, NULL)
GO
SET IDENTITY_INSERT [BasicInfo].[SharedGood] ON 

GO
INSERT [BasicInfo].[SharedGood] ([Id], [Name], [Code], [MainUnitId]) VALUES (1, N'Shared Good 1', N'SharedGood1', 1)
GO
INSERT [BasicInfo].[SharedGood] ([Id], [Name], [Code], [MainUnitId]) VALUES (2, N'Shared Good 2', N'SharedGood2', 2)
GO
SET IDENTITY_INSERT [BasicInfo].[SharedGood] OFF
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (1, N'Tank1', 1)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (2, N'Tank2', 1)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (3, N'Tank3', 2)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (4, N'Tank4', 2)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (5, N'Tank5', 3)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (6, N'Tank6', 11)
GO
INSERT [BasicInfo].[Tank] ([Id], [Name], [VesselInCompanyId]) VALUES (7, N'Tank7', 11)
GO
INSERT [BasicInfo].[Unit] ([Id], [Name]) VALUES (1, N'Metric Ton')
GO
INSERT [BasicInfo].[Unit] ([Id], [Name]) VALUES (2, N'Liter')
GO
INSERT [BasicInfo].[Unit] ([Id], [Name]) VALUES (3, N'Long Ton')
GO
INSERT [BasicInfo].[User] ([Id], [RoleId], [Name], [CompanyId]) VALUES (1, 1, N'Admin', 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (1, N'0123', N'ABBA', N'ABBA In SAPID', 1, 2, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (2, N'0093', N'AGEAN', N'AGEAN In SAPID', 1, 1, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (3, N'AREZOO', N'AREZOO', N'AREZOO In HAFIZ', 2, 2, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (4, N'NESHAT', N'NESHAT', N'NESHAT In HAFIZ', 2, 1, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (5, N'0123', N'ABBA', N'ABBA In IRISL', 4, 3, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (6, N'0093', N'AGEAN', N'AGEAN In IRISL', 4, 4, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (7, N'AREZOO', N'AREZOO', N'AREZOO In IRISL', 4, 3, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (8, N'NESHAT', N'NESHAT', N'NESHAT In IRISL', 4, 4, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (9, N'5002', N'TABUK', N'TABUK In IMSENGCO', 3, 3, 0)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (10, N'5001', N'PAGAS', N'PAGAS In IMSENGCO', 3, 4, 1)
GO
INSERT [BasicInfo].[Vessel] ([Id], [Code], [Name], [Description], [CompanyId], [VesselState], [IsActive]) VALUES (11, N'1093', N'TINA', N'TINA In SAPID', 1, 4, 1)
GO



/****** Object:  Index [IX_GoodId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_GoodId] ON [BasicInfo].[CompanyGoodUnit]
(
	[GoodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_ParentId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_ParentId] ON [BasicInfo].[CompanyGoodUnit]
(
	[ParentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_CompanyId] ON [BasicInfo].[Good]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SharedGoodId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_SharedGoodId] ON [BasicInfo].[Good]
(
	[SharedGoodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Unit_Id]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_Unit_Id] ON [BasicInfo].[Good]
(
	[Unit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_MainUnitId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_MainUnitId] ON [BasicInfo].[SharedGood]
(
	[MainUnitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_VesselInCompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_VesselInCompanyId] ON [BasicInfo].[Tank]
(
	[VesselInCompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_CompanyId] ON [BasicInfo].[User]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CompanyId]    Script Date: 7/4/2014 11:50:21 AM ******/
CREATE NONCLUSTERED INDEX [IX_CompanyId] ON [BasicInfo].[Vessel]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [BasicInfo].[Vessel] ADD  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [BasicInfo].[CompanyGoodUnit]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.CompanyGoodUnit_BasicInfo.CompanyGoodUnit_ParentId] FOREIGN KEY([ParentId])
REFERENCES [BasicInfo].[CompanyGoodUnit] ([Id])
GO
ALTER TABLE [BasicInfo].[CompanyGoodUnit] CHECK CONSTRAINT [FK_BasicInfo.CompanyGoodUnit_BasicInfo.CompanyGoodUnit_ParentId]
GO
ALTER TABLE [BasicInfo].[CompanyGoodUnit]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.CompanyGoodUnit_BasicInfo.Good_GoodId] FOREIGN KEY([GoodId])
REFERENCES [BasicInfo].[Good] ([Id])
GO
ALTER TABLE [BasicInfo].[CompanyGoodUnit] CHECK CONSTRAINT [FK_BasicInfo.CompanyGoodUnit_BasicInfo.Good_GoodId]
GO
ALTER TABLE [BasicInfo].[Good]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.Good_BasicInfo.Company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [BasicInfo].[Company] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [BasicInfo].[Good] CHECK CONSTRAINT [FK_BasicInfo.Good_BasicInfo.Company_CompanyId]
GO
ALTER TABLE [BasicInfo].[Good]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.Good_BasicInfo.Unit_Unit_Id] FOREIGN KEY([Unit_Id])
REFERENCES [BasicInfo].[Unit] ([Id])
GO
ALTER TABLE [BasicInfo].[Good] CHECK CONSTRAINT [FK_BasicInfo.Good_BasicInfo.Unit_Unit_Id]
GO
ALTER TABLE [BasicInfo].[Good]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.Good_dbo.SharedGoods_SharedGoodId] FOREIGN KEY([SharedGoodId])
REFERENCES [BasicInfo].[SharedGood] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [BasicInfo].[Good] CHECK CONSTRAINT [FK_BasicInfo.Good_dbo.SharedGoods_SharedGoodId]
GO
ALTER TABLE [BasicInfo].[SharedGood]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SharedGoods_BasicInfo.Unit_MainUnitId] FOREIGN KEY([MainUnitId])
REFERENCES [BasicInfo].[Unit] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [BasicInfo].[SharedGood] CHECK CONSTRAINT [FK_dbo.SharedGoods_BasicInfo.Unit_MainUnitId]
GO
ALTER TABLE [BasicInfo].[Tank]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.Tank_BasicInfo.Vessel_VesselInCompanyId] FOREIGN KEY([VesselInCompanyId])
REFERENCES [BasicInfo].[Vessel] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [BasicInfo].[Tank] CHECK CONSTRAINT [FK_BasicInfo.Tank_BasicInfo.Vessel_VesselInCompanyId]
GO
ALTER TABLE [BasicInfo].[User]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.User_BasicInfo.Company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [BasicInfo].[Company] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [BasicInfo].[User] CHECK CONSTRAINT [FK_BasicInfo.User_BasicInfo.Company_CompanyId]
GO
ALTER TABLE [BasicInfo].[Vessel]  WITH CHECK ADD  CONSTRAINT [FK_BasicInfo.Vessel_BasicInfo.Company_CompanyId] FOREIGN KEY([CompanyId])
REFERENCES [BasicInfo].[Company] ([Id])
GO
ALTER TABLE [BasicInfo].[Vessel] CHECK CONSTRAINT [FK_BasicInfo.Vessel_BasicInfo.Company_CompanyId]
GO

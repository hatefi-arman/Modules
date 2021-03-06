USE [StorageSpace]
GO

/****** Object:  Schema [BasicInfo]    Script Date: 7/4/2014 11:39:25 AM ******/
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'BasicInfo')
EXEC sys.sp_executesql N'CREATE SCHEMA [BasicInfo]'

GO
/****** Object:  View [BasicInfo].[ActivityLocationView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[ActivityLocationView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[ActivityLocationView]
	AS SELECT * FROM BasicInfo.ActivityLocation' 
GO
/****** Object:  View [BasicInfo].[CompanyGoodUnitView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[CompanyGoodUnitView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[CompanyGoodUnitView]
	AS SELECT * FROM BasicInfo.CompanyGoodUnit' 
GO
/****** Object:  View [BasicInfo].[CompanyView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[CompanyView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[CompanyView]
	AS SELECT * FROM BasicInfo.Company' 
GO
/****** Object:  View [BasicInfo].[CurrencyView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[CurrencyView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[CurrencyView]
	AS SELECT * FROM BasicInfo.Currency' 
GO
/****** Object:  View [BasicInfo].[GoodPartyAssignmentView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[GoodPartyAssignmentView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[GoodPartyAssignmentView]
	AS SELECT * FROM BasicInfo.GoodPartyAssignment' 
GO
/****** Object:  View [BasicInfo].[GoodView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[GoodView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[GoodView]
	AS SELECT * FROM BasicInfo.Good' 
GO
/****** Object:  View [BasicInfo].[SharedGoodView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[SharedGoodView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[SharedGoodView]
AS
SELECT   Id, Name, Code, MainUnitId
FROM      BasicInfo.SharedGood
' 
GO
/****** Object:  View [BasicInfo].[TankView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[TankView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[TankView]
	AS SELECT * FROM BasicInfo.Tank' 
GO
/****** Object:  View [BasicInfo].[UnitView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[UnitView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[UnitView]
	AS SELECT * FROM BasicInfo.Unit' 
GO
--/****** Object:  View [BasicInfo].[UserView]    Script Date: 7/4/2014 11:39:25 AM ******/
--SET ANSI_NULLS ON
--GO
--SET QUOTED_IDENTIFIER ON
--GO
--IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[UserView]'))
--EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[UserView]
--	AS SELECT * FROM BasicInfo.[User]' 
--GO
/****** Object:  View [BasicInfo].[VesselView]    Script Date: 7/4/2014 11:39:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[VesselView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[VesselView]
	AS SELECT * FROM BasicInfo.Vessel' 
GO


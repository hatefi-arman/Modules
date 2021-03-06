USE [StorageSpace]
GO
/****** Object:  View [BasicInfo].[InventoryUnitView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryUnitView]'))
DROP VIEW [BasicInfo].[InventoryUnitView]
GO
/****** Object:  View [BasicInfo].[InventorySharedGoodView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventorySharedGoodView]'))
DROP VIEW [BasicInfo].[InventorySharedGoodView]
GO
/****** Object:  View [BasicInfo].[InventoryCurrencyView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCurrencyView]'))
DROP VIEW [BasicInfo].[InventoryCurrencyView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyView]'))
DROP VIEW [BasicInfo].[InventoryCompanyView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselView]'))
DROP VIEW [BasicInfo].[InventoryCompanyVesselView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselTankView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselTankView]'))
DROP VIEW [BasicInfo].[InventoryCompanyVesselTankView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyGoodView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyGoodView]'))
DROP VIEW [BasicInfo].[InventoryCompanyGoodView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyGoodUnitView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyGoodUnitView]'))
DROP VIEW [BasicInfo].[InventoryCompanyGoodUnitView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselGoodUnitView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselGoodUnitView]'))
DROP VIEW [BasicInfo].[InventoryCompanyVesselGoodUnitView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselGoodView]    Script Date: 14/5/2014 10:59:17 AM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselGoodView]'))
DROP VIEW [BasicInfo].[InventoryCompanyVesselGoodView]
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselGoodView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselGoodView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyVesselGoodView]
AS
SELECT   InventoryDB.dbo.AbstractInventoryStocks.Id, InventoryDB.dbo.AbstractInventoryStocks.ProductTypeId AS SharedGoodId, 
                InventoryDB.dbo.AbstractStores.Id AS CompanyVesselId
FROM      InventoryDB.dbo.AbstractStores INNER JOIN
                InventoryDB.dbo.AbstractInventoryStocks ON InventoryDB.dbo.AbstractStores.Id = InventoryDB.dbo.AbstractInventoryStocks.StoreId


' 
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselGoodUnitView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselGoodUnitView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyVesselGoodUnitView]
AS
SELECT   InventoryDB.dbo.StockingUnitOfMeasures.Id, InventoryDB.dbo.ProductTypeUnitOfMeasures.UnitOfMeasure, 
                [BasicInfo].[InventoryCompanyVesselGoodView].Id AS CompanyVesselGoodId
FROM      InventoryDB.dbo.StockingUnitOfMeasures INNER JOIN
                InventoryDB.dbo.ProductTypeUnitOfMeasures ON 
                InventoryDB.dbo.StockingUnitOfMeasures.ProductTypeUnitOfMeasureId = InventoryDB.dbo.ProductTypeUnitOfMeasures.Id INNER JOIN
                [BasicInfo].[InventoryCompanyVesselGoodView] ON InventoryDB.dbo.StockingUnitOfMeasures.MeasurableInventoryStockId = [BasicInfo].[InventoryCompanyVesselGoodView].Id


' 
GO
/****** Object:  View [BasicInfo].[InventoryCompanyGoodUnitView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyGoodUnitView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyGoodUnitView]
AS
SELECT   InventoryDB.dbo.StoreGroups.Id * 10000 + InventoryDB.dbo.AbstractProductTypes.Id * 10 + InventoryDB.dbo.ProductTypeUnitOfMeasures.Id AS Id, 
                InventoryDB.dbo.StoreGroups.Id * 1000 + InventoryDB.dbo.AbstractProductTypes.Id AS CompanyGoodId, 
                InventoryDB.dbo.AbstractProductTypes.Id AS SharedGoodId, InventoryDB.dbo.StoreGroups.Id AS CompanyId, 
                InventoryDB.dbo.ProductTypeUnitOfMeasures.UnitOfMeasure AS Name, InventoryDB.dbo.ProductTypeUnitOfMeasures.UnitOfMeasure AS Abbreviation, 
                InventoryDB.dbo.ProductTypeUnitOfMeasures.[To], ISNULL(InventoryDB.dbo.ProductTypeUnitOfMeasures.Factor, 0) AS Coefficient, 
                InventoryDB.dbo.ProductTypeUnitOfMeasures.Offset, CAST(NULL AS bigint) AS ParentId
FROM      InventoryDB.dbo.ProductTypeUnitOfMeasures INNER JOIN
                InventoryDB.dbo.AbstractProductTypes ON 
                InventoryDB.dbo.ProductTypeUnitOfMeasures.MeasurableProductTypeId = InventoryDB.dbo.AbstractProductTypes.Id CROSS JOIN
                InventoryDB.dbo.StoreGroups

' 
GO
/****** Object:  View [BasicInfo].[InventoryCompanyGoodView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyGoodView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyGoodView]
AS
SELECT   InventoryDB.dbo.StoreGroups.Id * 1000 + InventoryDB.dbo.AbstractProductTypes.Id AS Id, InventoryDB.dbo.AbstractProductTypes.Id AS SharedGoodId, 
                InventoryDB.dbo.StoreGroups.Id AS CompanyId, InventoryDB.dbo.AbstractProductTypes.Name, InventoryDB.dbo.AbstractProductTypes.Code
FROM      InventoryDB.dbo.StoreGroups CROSS JOIN
                InventoryDB.dbo.AbstractProductTypes

' 
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselTankView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselTankView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyVesselTankView]
AS
SELECT   ISNULL(InventoryDB.dbo.Stores.Id, 1) * 1000 + ISNULL(InventoryDB.dbo.StorageAreas.Id, 1) AS Id, InventoryDB.dbo.Stores.Id AS VesselInInventoryId, 
                ISNULL(InventoryDB.dbo.StorageAreas.Name, ''---'') AS Name, InventoryDB.dbo.StorageAreas.Description
FROM      InventoryDB.dbo.StorageAreas FULL OUTER JOIN
                InventoryDB.dbo.Stores ON InventoryDB.dbo.StorageAreas.Id = InventoryDB.dbo.Stores.Id

' 
GO
/****** Object:  View [BasicInfo].[InventoryCompanyVesselView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyVesselView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyVesselView]
AS
SELECT   InventoryDB.dbo.Stores.Id, InventoryDB.dbo.AbstractStores.Name, InventoryDB.dbo.AbstractStores.Description AS Code, 
                InventoryDB.dbo.Stores.GroupId AS CompanyId, CAST(1 AS bit) AS IsActive
FROM      InventoryDB.dbo.AbstractStores INNER JOIN
                InventoryDB.dbo.Stores ON InventoryDB.dbo.AbstractStores.Id = InventoryDB.dbo.Stores.Id INNER JOIN
                InventoryDB.dbo.StoreGroups ON InventoryDB.dbo.Stores.GroupId = InventoryDB.dbo.StoreGroups.Id

' 
GO
/****** Object:  View [BasicInfo].[InventoryCompanyView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCompanyView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCompanyView]
AS
SELECT   InventoryDB.dbo.AbstractStores.Id, InventoryDB.dbo.AbstractStores.Description AS Code, InventoryDB.dbo.AbstractStores.Name
FROM      InventoryDB.dbo.AbstractStores INNER JOIN
                InventoryDB.dbo.StoreGroups ON InventoryDB.dbo.AbstractStores.Id = InventoryDB.dbo.StoreGroups.Id

' 
GO
/****** Object:  View [BasicInfo].[InventoryCurrencyView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryCurrencyView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryCurrencyView]
AS
SELECT        Id, Description1 AS Abbreviation, Description2 AS Name
FROM            InventoryDB.dbo.Currencies


' 
GO
/****** Object:  View [BasicInfo].[InventorySharedGoodView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventorySharedGoodView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventorySharedGoodView]
AS
SELECT   InventoryDB.dbo.AbstractProductTypes.Id, InventoryDB.dbo.AbstractProductTypes.Name, InventoryDB.dbo.AbstractProductTypes.Code, 
                InventoryDB.dbo.AbstractProductTypes.Description, InventoryDB.dbo.ProductTypeUnitOfMeasures.UnitOfMeasure AS MainUnitCode, 
                InventoryDB.dbo.ProductTypeUnitOfMeasures.Id AS MainUnitId
FROM      InventoryDB.dbo.AbstractProductTypes INNER JOIN
                InventoryDB.dbo.ProductTypeUnitOfMeasures ON 
                InventoryDB.dbo.AbstractProductTypes.Id = InventoryDB.dbo.ProductTypeUnitOfMeasures.MeasurableProductTypeId
WHERE   (InventoryDB.dbo.ProductTypeUnitOfMeasures.IsPreferred = 1)

' 
GO
/****** Object:  View [BasicInfo].[InventoryUnitView]    Script Date: 14/5/2014 10:59:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[BasicInfo].[InventoryUnitView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [BasicInfo].[InventoryUnitView]
AS
SELECT   InventoryDB.dbo.ProductTypeUnitOfMeasures.Id, InventoryDB.dbo.ProductTypeUnitOfMeasures.UnitOfMeasure AS Abbreviation, 
                InventoryDB.dbo.AbstractUnitOfMeasures.Description1 AS Name
FROM      InventoryDB.dbo.ProductTypeUnitOfMeasures INNER JOIN
                InventoryDB.dbo.AbstractUnitOfMeasures ON 
                InventoryDB.dbo.ProductTypeUnitOfMeasures.UnitOfMeasure = InventoryDB.dbo.AbstractUnitOfMeasures.Abbreviation

' 
GO

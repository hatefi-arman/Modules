SELECT 
VoyagesQuery.Id , 
VoyagesQuery.VoyageNumber, 
VoyagesQuery.VoyageNumber AS Description,
VoyagesQuery.CompanyId,
Fuel.VesselInCompany.Id AS VesselInCompanyId,
VoyagesQuery.StartDateTime AS StartDate,
VoyagesQuery.EndDateTime AS EndDate,
VoyagesQuery.IsActive FROM (
SELECT [Id]
      ,[VesselCode]
      ,4 AS [ShipOwnerId]
      ,[VoyageNumber]
      ,[StartDateTime]
      ,[EndDateTime]
      ,[IsActive]
	  , 1 AS CompanyId
  FROM [StorageSpace].[dbo].[SAPIDVoyagesView]

UNION 

SELECT 10000000 + [Id]
      ,[VesselCode] 
      ,4 AS [ShipOwnerId]
      ,[VoyageNumber]
      ,[StartDateTime]
      ,[EndDateTime]
      ,[IsActive]
	  , 2 AS CompanyId
  FROM [StorageSpace].[dbo].[HAFIZVoyagesView]) AS VoyagesQuery 
  INNER JOIN Fuel.Vessel ON VoyagesQuery.VesselCode COLLATE SQL_Latin1_General_CP1256_CI_AS = Fuel.Vessel.Code COLLATE SQL_Latin1_General_CP1256_CI_AS
  INNER JOIN Fuel.VesselInCompany ON Fuel.VesselInCompany.VesselId = Fuel.Vessel.Id AND Fuel.VesselInCompany.CompanyId = VoyagesQuery.CompanyId

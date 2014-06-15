INSERT INTO Fuel.Voyage 
SELECT 
[VoyageNumber]
,[VoyageNumber] AS [Description]
,1 AS [VesselInCompanyId] --For ABBA In SAPID
,11 AS CompanyId  -- SAPID
,[StartDateTime] AS [StartDate]
,[EndDateTime] AS [EndDate]
,[IsActive]
FROM dbo.SAPIDVoyagesView 
INNER JOIN Fuel.Vessel ON Vessel.Code COLLATE Arabic_CI_AS =  SAPIDVoyagesView.VesselCode
WHERE IsActive = 1

 
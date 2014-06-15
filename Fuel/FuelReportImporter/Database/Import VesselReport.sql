SELECT 
	VRSh.ShipID, 
	VRFR.ID AS Code, 
	VRFR.VoyageNo + ' / ' + ISNULL(VRFR.PortName, '') AS Description,
	CASE ISNULL(VRFR.FuelRepotType , 1)
		WHEN 1 THEN 1
		WHEN 2 THEN 2
		WHEN 3 THEN 8
		WHEN 4 THEN 7 
		WHEN 5 THEN 5 
		WHEN 6 THEN 6 
		WHEN 7 THEN 3 
		WHEN 8 THEN 4 
		END AS FuelReportType,
	CAST(CAST(VRFR.Year AS NVARCHAR(4)) + '/' + CAST(VRFR.Month AS NVARCHAR(4)) + '/' + CAST(VRFR.Day AS NVARCHAR(4)) AS DATETIME) AS ReportDate,
	CAST(CAST(VRFR.Year AS NVARCHAR(4)) + '/' + CAST(VRFR.Month AS NVARCHAR(4)) + '/' + CAST(VRFR.Day AS NVARCHAR(4)) AS DATETIME) AS EventDate,
	VRFR.VoyageNo AS VoyageNumber,
	FRVoy.Id AS VoyageId,
	FRVCo.Id AS [VesselInCompanyId]
FROM 
	dbo.RPMInfo VRFR 
	INNER JOIN dbo.Ships VRSh  ON VRFR.ShipID = VRSh.ID
	INNER JOIN StorageSpace.Fuel.Vessel FRV ON FRV.Code = VRSh.ShipID
	INNER JOIN StorageSpace.Fuel.VesselInCompany FRVCo ON FRVCo.VesselId =  FRV.Id 
	INNER JOIN StorageSpace.Fuel.Voyage FRVoy ON FRVoy.VoyageNumber = VRFR.VoyageNo
--WHERE FRVCo.Id = @VesselInCompanyId
ORDER BY VRFR.Year, VRFR.Month, VRFR.Day

---------------------------------------------------------------------


---------------------------------------------------------------------
SELECT 
VRFR.ID AS FuelReportCode, 
'HFO' AS GoodCode,
InvCoGood.Id AS GoodId,
'Ton' AS UnitCode,
InvCoGoodUnit.Id AS GoodUnitId,
ConsInPortHO + ConsAtSeaHO AS Consumption,
TransferHo AS Transfer,
ROBHO AS ROB,
ReceivedHO AS Recieve,
CorrectionHo AS Correction,
CAST(CASE 
	WHEN CorrectionTypeHo IS NULL THEN NULL 
	WHEN CorrectionTypeHo = 1 THEN 1 --Plus
	ELSE 0 -- Minus 
	END AS BIT) AS CorrectionType
FROM dbo.RPMInfo VRFR
	INNER JOIN dbo.Ships VRSh  ON VRFR.ShipID = VRSh.ID
	INNER JOIN StorageSpace.Fuel.Vessel FRV ON FRV.Code = VRSh.ShipID
	INNER JOIN StorageSpace.Fuel.VesselInCompany FRVCo ON FRVCo.VesselId =  FRV.Id 
	INNER JOIN [StorageSpace].[BasicInfo].[InventoryCompanyGoodView] InvCoGood ON InvCoGood.CompanyId = FRVCo.CompanyId AND InvCoGood.Code = 'HFO'
	INNER JOIN [StorageSpace].[BasicInfo].[InventoryCompanyGoodUnitView] InvCoGoodUnit ON InvCoGoodUnit.CompanyGoodId = InvCoGood.Id  AND UPPER(InvCoGoodUnit.Abbreviation) = 'Ton'
--WHERE VRFR.ID = @FuelReportCode

UNION 

SELECT 
VRFR.ID AS FuelReportCode, 
'MDO' AS GoodCode,
InvCoGood.Id AS GoodId,
'Ton' AS UnitCode,
InvCoGoodUnit.Id AS GoodUnitId,
ConsInPortDO + ConsAtSeaDO AS Consumption,
TransferDO AS Transfer,
ROBDO AS ROB,
ReceivedDO AS Recieve,
CorrectionDO AS Correction,
CAST(CASE 
	WHEN CorrectionTypeDO IS NULL THEN NULL 
	WHEN CorrectionTypeDO = 1 THEN 1 --Plus
	ELSE 0 -- Minus 
	END AS BIT) AS CorrectionType
FROM dbo.RPMInfo VRFR
	INNER JOIN dbo.Ships VRSh  ON VRFR.ShipID = VRSh.ID
	INNER JOIN StorageSpace.Fuel.Vessel FRV ON FRV.Code = VRSh.ShipID
	INNER JOIN StorageSpace.Fuel.VesselInCompany FRVCo ON FRVCo.VesselId =  FRV.Id 
	INNER JOIN [StorageSpace].[BasicInfo].[InventoryCompanyGoodView] InvCoGood ON InvCoGood.CompanyId = FRVCo.CompanyId AND InvCoGood.Code = 'MDO'
	INNER JOIN [StorageSpace].[BasicInfo].[InventoryCompanyGoodUnitView] InvCoGoodUnit ON InvCoGoodUnit.CompanyGoodId = InvCoGood.Id  AND UPPER(InvCoGoodUnit.Abbreviation) = 'Ton'
--WHERE VRFR.ID = @FuelReportCode

UNION

SELECT 
VRFR.ID AS FuelReportCode, 
'MGO' AS GoodCode,
InvCoGood.Id AS GoodId,
'Ton' AS UnitCode,
InvCoGoodUnit.Id AS GoodUnitId,
ConsInPortMGO + ConsAtSeaMGO AS Consumption,
TransferMGOLS AS Transfer,
ROBMGO AS ROB,
ReceivedMGO AS Recieve,
CorrectionMGOLS AS Correction,
CAST(CASE 
	WHEN CorrectionTypeMGOLS IS NULL THEN NULL 
	WHEN CorrectionTypeMGOLS = 1 THEN 1 --Plus
	ELSE 0 -- Minus 
	END AS BIT) AS CorrectionType
FROM dbo.RPMInfo VRFR
	INNER JOIN dbo.Ships VRSh  ON VRFR.ShipID = VRSh.ID
	INNER JOIN StorageSpace.Fuel.Vessel FRV ON FRV.Code = VRSh.ShipID
	INNER JOIN StorageSpace.Fuel.VesselInCompany FRVCo ON FRVCo.VesselId =  FRV.Id 
	INNER JOIN [StorageSpace].[BasicInfo].[InventoryCompanyGoodView] InvCoGood ON InvCoGood.CompanyId = FRVCo.CompanyId AND InvCoGood.Code = 'MGO'
	INNER JOIN [StorageSpace].[BasicInfo].[InventoryCompanyGoodUnitView] InvCoGoodUnit ON InvCoGoodUnit.CompanyGoodId = InvCoGood.Id  AND UPPER(InvCoGoodUnit.Abbreviation) = 'Ton'
--WHERE VRFR.ID = @FuelReportCode
---------------------------------------------------------------------






---------------------------------------------------------------------
SELECT 
VRFR.ID AS FuelReportCode, 
'HFO' AS GoodCode,
'Ton' AS UnitCode,
ConsInPortHO + ConsAtSeaHO AS Consumption,
TransferHo AS Transfer,
ROBHO AS ROB,
ReceivedHO AS Recieve,
CorrectionHo AS Correction,
CASE 
	WHEN CorrectionTypeHo IS NULL THEN NULL 
	WHEN CorrectionTypeHo = 1 THEN 1 --Plus
	ELSE 0 -- Minus 
	END AS CorrectionType
FROM dbo.RPMInfo VRFR

UNION 

SELECT 
VRFR.ID AS FuelReportCode, 
'MDO' AS GoodCode,
'Ton' AS UnitCode,
ConsInPortDO + ConsAtSeaDO AS Consumption,
TransferDO AS Transfer,
ROBDO AS ROB,
ReceivedDO AS Recieve,
CorrectionDO AS Correction,
CASE 
	WHEN CorrectionTypeDO IS NULL THEN NULL 
	WHEN CorrectionTypeDO = 1 THEN 1 --Plus
	ELSE 0 -- Minus 
	END AS CorrectionType
FROM dbo.RPMInfo VRFR

UNION

SELECT 
VRFR.ID AS FuelReportCode, 
'MGO' AS GoodCode,
'Ton' AS UnitCode,
ConsInPortMGO + ConsAtSeaMGO AS Consumption,
TransferMGOLS AS Transfer,
ROBMGO AS ROB,
ReceivedMGO AS Recieve,
CorrectionMGOLS AS Correction,
CASE 
	WHEN CorrectionTypeMGOLS IS NULL THEN NULL 
	WHEN CorrectionTypeMGOLS = 1 THEN 1 --Plus
	ELSE 0 -- Minus 
	END AS CorrectionType
FROM dbo.RPMInfo VRFR
---------------------------------------------------------------------








--select * from StorageSpace.Fuel.Voyage FRVoy

--select * from   
--dbo.RPMInfo   where ShipID = 91


--update  
--dbo.RPMInfo  set VoyageNo = 'PJL2098'  where VoyageNo = 'BID1092'

--update  
--dbo.RPMInfo  set VoyageNo = REPLACE( VoyageNo, 'S4-', 'BID')  where ShipID = 91
--update StorageSpace.Fuel.Voyage set VoyageNumber = REPLACE('BID', VoyageNumber, 

/*
Fuel.FuelReport Types
--------------------------------------------
NoonReport = 1,
EndOfVoyage = 2,
ArrivalReport = 3,
DepartureReport = 4,
EndOfYear = 5,
EndOfMonth = 6,
CharterInEnd = 7,
CharterOutStart = 8,
DryDock = 9,
OffHire = 10,
LayUp = 11,

============================================
VesselReport FR Types
--------------------------------------------
case 1:
    return FuelReportTypeEnum.NoonReport;
case 2:
    return FuelReportTypeEnum.EndofVoyage;
case 3:
    return FuelReportTypeEnum.CharterOutStart;
case 4:
    return FuelReportTypeEnum.CharterInEnd;
case 5:
    return FuelReportTypeEnum.EndOfYear;
case 6:
    return FuelReportTypeEnum.EndOfMonth;
case 7:
    return FuelReportTypeEnum.ArrivalReport;
case 8:
    return FuelReportTypeEnum.DepartureReport;

*/

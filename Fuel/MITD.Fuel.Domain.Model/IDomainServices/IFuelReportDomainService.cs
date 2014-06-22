using System;
using System.Collections.Generic;
using MITD.Domain.Model;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface IFuelReportDomainService : IDomainService
    {
        FuelReport Get(long id);

        List<FuelReport> GetAll();

        PageResult<FuelReport> GetByFilter(long companyId, long vesselInCompanyId, int pageSize, int pageIndex);

        List<FuelReport> GetPreviousNotFinalApprovedReports(FuelReport fuelReport);

        List<FuelReport> GetFuelReportsFromYesterday(FuelReport fuelReport);

        List<FuelReport> GetVoyagesValidEndOfVoyageFuelReports(List<Voyage> voyages);

        FuelReport GetVoyageValidEndOfVoyageFuelReport(long voyageId);
        FuelReport GetVoyageValidEndOfVoyageFuelReport(Voyage voyage);

        List<FuelReport> GetVesselFuelReports(VesselInCompany vesselInCompany, DateTime? startDate, DateTime? endDate, bool ignoreTime = false);

        List<Reference> GetFuelReportDetailTransferReferences(FuelReportDetail fuelReportDetail);
        List<Reference> GetFuelReportDetailReceiveReferences(FuelReportDetail fuelReportDetail);
        List<Reference> GetFuelReportDetailCorrectionReferences(FuelReportDetail fuelReportDetail);

        FuelReport GetLastIssuedEOVFuelReportOfPreviousVoyages(FuelReport fuelReport);

        List<FuelReport> GetNotIssuedEOVFuelReportsOfPreviousVoyages(FuelReport fuelReport);

        FuelReport GetLastIssuedFuelReportBefore(FuelReport fuelReport);

        void SetFuelReportInventoryResults(InventoryResultCommand resultCommand, FuelReport fuelReport);

        //Commented due to be unusable
        //void InvalidateAllPreviousEndOfVoyageFuelReports(FuelReport fuelReport);

        List<FuelReport> GetVoyageAllEndOfVoyageFuelReport(Voyage voyage);

        InventoryOperation GetVoyageConsumptionIssueOperation(long voyageId);

        ChangingFuelReportDateData GetChangingFuelReportDateData(long fuelReportId, DateTime newDateTime);
        List<Reference> GetFuelReportDetailRejectedTransferReferences(FuelReportDetail fuelReportDetail);

        decimal CalculateReportingConsumption(FuelReportDetail fuelReportDetail);

        FuelReportDetail GetLastReceiveFuelReportDetailBefore(FuelReportDetail fuelReportDetail);
    }

    public class InventoryResultCommand
    {
        public long FuelReportId;
        public List<InventoryResultCommandItem> Items;
    }

    public class InventoryResultCommandItem
    {
        public long FuelReportDetailId;
        public long OperationId;
        public string ActionNumber;
        public InventoryActionType ActionType;
        public DateTime ActionDate;
    }

    public class ChangingFuelReportDateData
    {
        public FuelReport ChangingFuelReport;

        public FuelReport NextFuelReportBeforeChangeDate;

        public FuelReport NextFuelReportAfterChangeDate;
    }
}

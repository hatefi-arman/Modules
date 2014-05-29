using System;
using System.Data;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Presentation.Contracts.FacadeServices;

namespace MITD.Fuel.Application.Service
{
    public class FuelReportApplicationService : IFuelReportApplicationService
    {
        private readonly IUnitOfWorkScope unitOfWorkScope;
        private readonly ICurrencyDomainService currencyDomainService;
        private readonly IVoyageDomainService voyageDomainService;
        private readonly IFuelReportDomainService fuelReportDomainService;
        private readonly IApproveFlowApplicationService approveFlowApplicationService;

        public FuelReportApplicationService(
            IUnitOfWorkScope unitOfWorkScope,
            ICurrencyDomainService currencyDomainService,
            IVoyageDomainService voyageDomainService,
            IFuelReportDomainService fuelReportDomainService,
            IApproveFlowApplicationService approveFlowApplicationService)
        {
            this.unitOfWorkScope = unitOfWorkScope;
            this.currencyDomainService = currencyDomainService;
            this.voyageDomainService = voyageDomainService;
            this.fuelReportDomainService = fuelReportDomainService;
            this.approveFlowApplicationService = approveFlowApplicationService;
        }

        //================================================================================

        public FuelReport GetById(long id)
        {
            var fuelReport = fuelReportDomainService.Get(id);

            if (fuelReport == null)
                throw new ObjectNotFound("FuelReport", id);

            return fuelReport;
        }

        //================================================================================

        public FuelReportDetail UpdateFuelReportDetail(
            long fuelReportId,
            long fuelReportDetailId,
            double rob,
            double consumption,
            double? receive,
            ReceiveTypes? receiveType,
            double? transfer,
            TransferTypes? transferType,
            double? correction,
            CorrectionTypes? correctionType,
            decimal? correctionPrice,
            long? currencyId,
            Reference transferReference,
            Reference receiveReference,
            Reference correctionReference)
        {
            var currentFuelReport = GetById(fuelReportId);

            var result = currentFuelReport.UpdateFuelReportDetail(
                fuelReportDetailId,
                rob,
                consumption,
                receive,
                receiveType,
                transfer,
                transferType,
                correction,
                correctionType,
                correctionPrice,
                currencyId,
                transferReference,
                receiveReference,
                correctionReference,
                fuelReportDomainService,
                currencyDomainService);

            try
            {
                this.unitOfWorkScope.Commit();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw new ConcurencyException("UpdateFuelReportDetail");
            }
            //            catch (Exception ex)
            //            {
            //                throw new UnHandleException(ex);
            //            }

            return result;
        }

        //================================================================================

        public FuelReport UpdateVoyageId(long fuelReportId, long voyageId)
        {
            var fuelReport = GetById(fuelReportId);

            fuelReport.UpdateVoyageId(voyageId, voyageDomainService);

            try
            {
                unitOfWorkScope.Commit();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw new ConcurencyException("UpdateVoyage");
            }

            return fuelReport;
        }

        //================================================================================

        public void IsSetFuelReportInventoryResultPossible(long fuelReportId)
        {
            var fuelReport = GetById(fuelReportId);

            fuelReport.IsWaitingToBeClosed();
        }

        //================================================================================

        public void SetFuelReportInventoryResults(InventoryResultCommand resultBag)
        {
            var fuelReport = GetById(resultBag.FuelReportId);

            this.fuelReportDomainService.SetFuelReportInventoryResults(resultBag, fuelReport);

            try
            {
                this.unitOfWorkScope.Commit();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw new ConcurencyException("SetFuelReportInventoryResults");
            }
        }

        //================================================================================

        public FuelReport UpdateVoyageEndOfVoyageFuelReport(long voyageId, DateTime newDateTime)
        {
            var eovFuelReport = this.fuelReportDomainService.GetVoyageValidEndOfVoyageFuelReport(voyageId);

            var changingFuelReportData = this.fuelReportDomainService.GetChangingFuelReportDateData(eovFuelReport.Id, newDateTime);


            approveFlowApplicationService.ActApprovalFlow(changingFuelReportData.ChangingFuelReport.Id, WorkflowEntities.FuelReport, 1, "", WorkflowActions.Reject);

            if (changingFuelReportData.NextFuelReportAfterChangeDate != null && changingFuelReportData.NextFuelReportAfterChangeDate.State == States.Submitted)
                approveFlowApplicationService.ActApprovalFlow(changingFuelReportData.NextFuelReportAfterChangeDate.Id, WorkflowEntities.FuelReport, 1, "", WorkflowActions.Reject);

            if (changingFuelReportData.NextFuelReportBeforeChangeDate != null && changingFuelReportData.NextFuelReportBeforeChangeDate.State == States.Submitted)
                approveFlowApplicationService.ActApprovalFlow(changingFuelReportData.NextFuelReportBeforeChangeDate.Id, WorkflowEntities.FuelReport, 1, "", WorkflowActions.Reject);

            changingFuelReportData.ChangingFuelReport.UpdateEventDate(newDateTime);


            if (changingFuelReportData.NextFuelReportBeforeChangeDate != null)
                changingFuelReportData.NextFuelReportBeforeChangeDate.UpdateConsumption(fuelReportDomainService);

            changingFuelReportData.ChangingFuelReport.UpdateConsumption(fuelReportDomainService);

            if (changingFuelReportData.NextFuelReportAfterChangeDate != null)
                changingFuelReportData.NextFuelReportAfterChangeDate.UpdateConsumption(fuelReportDomainService);

            try
            {
                unitOfWorkScope.Commit();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw new ConcurencyException("UpdateVoyageEndOfVoyageFuelReport");
            }

            return changingFuelReportData.ChangingFuelReport;
        }

        //================================================================================
    }
}

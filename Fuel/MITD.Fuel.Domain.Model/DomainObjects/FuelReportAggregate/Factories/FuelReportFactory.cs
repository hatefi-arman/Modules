using System;
using System.Collections.Generic;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Domain.Model.DomainObjects.Factories
{
    public class FuelReportFactory : IFuelReportFactory
    {
        private readonly IEntityConfigurator<FuelReport> fuelReportConfigurator;
        private readonly IWorkflowRepository workflowRepository;
        private readonly IVesselInCompanyRepository vesselInCompanyRepository;
        //private readonly IOffhireDomainService offhireDomainService;
        //private readonly IVesselInCompanyDomainService vesselDomainService;
        //private readonly ICompanyDomainService companyDomainService;
        //private readonly ITankDomainService tankDomainService;
        //private readonly ICurrencyDomainService currencyDomainService;
        //private readonly IGoodDomainService goodDomainService;
        //private readonly IGoodUnitDomainService goodUnitDomainService;
        //private readonly IOffhireManagementSystemDomainService offhireManagementSystemDomainService;
        //private readonly IVoyageDomainService voyageDomainService;
        //private readonly IActivityLocationDomainService activityLocationDomainService;

        public FuelReportFactory(
            IEntityConfigurator<FuelReport> fuelReportConfigurator,
            IWorkflowRepository workflowRepository, 
            IVesselInCompanyRepository vesselInCompanyRepository /*,
            IOffhireDomainService offhireDomainService,
            IVesselInCompanyDomainService vesselDomainService,
            ICompanyDomainService companyDomainService,
            ITankDomainService tankDomainService,
            ICurrencyDomainService currencyDomainService,
            IGoodDomainService goodDomainService,
            IGoodUnitDomainService goodUnitDomainService,
            IOffhireManagementSystemDomainService offhireManagementSystemDomainService,
            IVoyageDomainService voyageDomainService,
            IActivityLocationDomainService activityLocationDomainService*/)
        {
            this.fuelReportConfigurator = fuelReportConfigurator;
            this.workflowRepository = workflowRepository;
            //this.vesselInInventoryRepository = vesselInInventoryRepository;
            this.vesselInCompanyRepository = vesselInCompanyRepository;
            
        }

        public FuelReport CreateFuelReport(
            string code,
            string description,
            DateTime eventDate,
            DateTime reportDate,
            long vesselInCompanyId,
            long? voyageId,
            FuelReportTypes fuelReportType,
            States state)
        {
            var vesselInCompany = vesselInCompanyRepository.First(vic => vic.Id == vesselInCompanyId);

            var fuelReport = new FuelReport(
                0,
                code,
                description,
                eventDate,
                reportDate,
                vesselInCompany,
                voyageId,
                fuelReportType,
                state);

            var init = this.workflowRepository.Single(c => 
                    c.WorkflowEntity == WorkflowEntities.FuelReport && 
                    c.CurrentWorkflowStage == WorkflowStages.Initial);

            if (init == null)
                throw new ObjectNotFound("FuelReportInitialStep");

            var fuelReportWorkflow = new FuelReportWorkflowLog(-1, WorkflowEntities.FuelReport, DateTime.Now, WorkflowActions.Init,
                //TODO: Fake ActorId
                    1101, "", init.Id, true);

            fuelReport.ApproveWorkFlows.Add(fuelReportWorkflow);

            fuelReportConfigurator.Configure(fuelReport);

            return fuelReport;
        }

        public FuelReportDetail CreateFuelReportDetail(long fuelReportId,
            double rob,
            string robUOM,
            double consumption,
            double? receive,
            ReceiveTypes? receiveType,
            double? transfer,
            TransferTypes? transferType,
            double? correction,
            CorrectionTypes? correctionType,
            decimal? correctionPrice,
            string correctionPriceCurrencyISOCode,
            long? correctionPriceCurrencyId,
            long fuelTypeId,
            long measuringUnitId,
            long tankId)
        {
            var fuelReportDetail = new FuelReportDetail(
                0, 
                fuelReportId,
                rob,
                robUOM,
                consumption,
                receive,
                receiveType,
                transfer,
                transferType,
                correction,
                correctionType,
                correctionPrice,
                correctionPriceCurrencyISOCode,
                correctionPriceCurrencyId,
                fuelTypeId,
                measuringUnitId,
                tankId);

            return fuelReportDetail;
        }

}
}
using System.Linq;
using FuelReportImporter.Database.VesselReportsDataSetTableAdapters;
using MITD.Fuel.Data.EF.Context;
using MITD.Fuel.Data.EF.Repositories;
using MITD.Fuel.Domain.Model.DomainObjects.Factories;
using MITD.Fuel.Domain.Model.DomainObjects.FuelReportAggregate.Factories;
using MITD.Fuel.Domain.Model.DomainObjects.VesselInCompanyAggregate.Factories;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Domain.Repository;
using MITD.DataAccess.EF;
using MITD.Fuel.Integration.Inventory;

namespace FuelReportImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var fuelContext = new DataContainer();
            UnitOfMeasuresAndCurrenciesRegsitrar.ReloadData();

            var reportTableAdapter = new VesselReportTableAdapter();
            var reportDetailsTableAdapter = new VesselReportDetailsTableAdapter();


            var reportDataTable = reportTableAdapter.GetData(1);

            var userId = 1101;

            var initialToApprovedFuelReportStep = fuelContext.ApproveFlows.Where(
                fl =>
                    fl.WorkflowEntity == WorkflowEntities.FuelReport &&
                    fl.ActorUserId == userId &&
                    fl.CurrentWorkflowStage == WorkflowStages.Initial &&
                    fl.WithWorkflowAction == WorkflowActions.Approve);

            var efUnitOfWork = new UnitOfWorkScope( new EFUnitOfWorkFactory(() => fuelContext));

            var vesselInInventoryRepository = new VesselInInventoryRepository(efUnitOfWork);
            var vesselInCompanyStateFactory  = new VesselInCompanyStateFactory();
            
            var fuelReportFactory = new FuelReportFactory(
                new FuelReportConfigurator(
                    new FuelReportStateFactory(), 
                    vesselInInventoryRepository,
                    vesselInCompanyStateFactory), 
                new WorkflowRepository(efUnitOfWork),
                new VesselInCompanyRepository(efUnitOfWork, new VesselInCompanyConfigurator(vesselInCompanyStateFactory, vesselInInventoryRepository)));

            foreach (var reportRow in reportDataTable)
            {
                var reportDetailsDataTable = reportDetailsTableAdapter.GetData(reportRow.Code);

                var fuelReport = fuelReportFactory.CreateFuelReport(reportRow.Code.ToString(), reportRow.Description, reportRow.EventDate, reportRow.ReportDate, reportRow.VesselInCompanyId, reportRow.VoyageId, (FuelReportTypes) reportRow.FuelReportType, States.Open);

                foreach (var reportDetailRow in reportDetailsDataTable)
                {

                    var fuelReportDetail = fuelReportFactory.CreateFuelReportDetail(
                        0, 
                        reportDetailRow.ROB, 
                        "TON", 
                        reportDetailRow.Consumption,
                        reportDetailRow.IsRecieveNull() ? null : (double?) reportDetailRow.Recieve, 
                        null, 
                        reportDetailRow.IsTransferNull() ? null : (double?)(double) reportDetailRow.Transfer,
                        null,
                        reportDetailRow.IsCorrectionNull() ? null : (double?) (double) reportDetailRow.Correction, 
                        null,
                        null, "USD", null, reportDetailRow.GoodId, reportDetailRow.GoodUnitId, 12001);

                    fuelReport.FuelReportDetails.Add(fuelReportDetail);
                }


                fuelContext.FuelReports.Add(fuelReport);

                fuelContext.SaveChanges();

                efUnitOfWork.Commit();
            }

            //foreach (FuelReport fuelReport in fuelContext.FuelReports)
            //{
            //    fuelReport.ApproveWorkFlows.Add(new FuelReportWorkflowLog(fuelReport.Id, WorkflowEntities.FuelReport, fuelReport.ReportDate.AddHours(5), WorkflowActions.Approve, 1, "", workflowStepId, true));
            //}

            //context.SaveChanges();

        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using MITD.Fuel.Data.EF.Context;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using System.Data.Entity.Migrations;
using System.Data.Entity;
using MITD.Fuel.Domain.Model.FakeDomainServices;

namespace MITD.Fuel.Data.EF.Migrations
{
    [Profile("debug")]
    public class Migration_Seed : Migration
    {
        public override void Up()
        {
            DataContainer context = new DataContainer();

            DbContextTransaction transaction = context.Database.BeginTransaction();

            try
            {
                System.Windows.Forms.MessageBox.Show("Continue Seeding the Database without debugging? \n\n At the moment, you can attach visual studio to the process with the name of 'Migrate.exe', put break point at any desired point and then hit the 'OK' in order to continue with debugging the process of Seed method, if no just hit the 'OK'.");

                //insertBasicInfo(context);


                context.Vessels.AddOrUpdate(FakeDomainService.GetVessels().ToArray());
                context.SaveChanges();

                context.VesselsInCompanies.AddOrUpdate(FakeDomainService.GetVesselsInCompanies().ToArray());
                context.SaveChanges();

                //context.Tanks.AddOrUpdate(FakeDomainService.GetTanks().ToArray());
                //context.SaveChanges();


                #region Add Voyages

                context.Voyages.AddOrUpdate(FakeDomainService.GetVoyages().ToArray());
                context.SaveChanges();

                context.VoyagesLog.AddOrUpdate(FakeDomainService.CreateVoyagesLog(context.Voyages.FirstOrDefault()).ToArray());
                context.SaveChanges();

                #endregion

                context.EffectiveFactors.AddOrUpdate(FakeDomainService.GetEffectiveFactors().ToArray());
                context.SaveChanges();

                var userId = context.Users.FirstOrDefault().Id;


                var initialToApprovedFuelReportStep = insertFuelReportWorkflowConfig(context, userId);
                insertFuelReportsFakeData(context, userId, initialToApprovedFuelReportStep.Id);

                insertOrderWorkflowConfig(userId, context);

                //insertOrderFakeData(userId, context);


                insertInvoiceWorkflowConfig(userId, context);
                //insertInvoiceFakeData(userId, context);

                var initialScrapStep = insertScrapWorkflowConfig(context, userId);
                //insertScrapsFakeData(context, userId, initialScrapStep.Id);


                var initialStep = insertCharterInWorkflowConfig(context, userId);
                //insertCharterInFakeData(context, userId, initialStep.Id);

                insertCharterOutWorkflowConfig(context, userId);

                insertOffhireWorkflowConfig(context, userId);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                context.Database.Connection.Close();

                //Database.Delete("DataContainer");
                throw;

            }
        }

        private void insertOrderFakeData(long userId, DataContainer context)
        {
            //context.Database.ExecuteSqlCommand(
            //    "Delete From  [Fuel].[Order]");

            //context.Database.ExecuteSqlCommand(
            //    "Delete From  [Fuel].[OrderItems]");

            //context.Database.ExecuteSqlCommand(
            //    " SET IDENTITY_INSERT[Fuel].[Order] ON "
            //    +
            //    " INSERT [Fuel].[Order]([Id],[Code], [Description], [SupplierId], [ReceiverId], [TransporterId], [OwnerId], [OrderType], [OrderDate],  [FromVesselInCompanyId], [ToVesselInCompanyId],State) " +
            //    " VALUES (1,1,'code1', 3, NULL, NULL, 1, 1, '2013-11-25',   NULL, NULL,1);" +
            //    " INSERT [Fuel].[Order]([Id],[Code], [Description], [SupplierId], [ReceiverId], [TransporterId], [OwnerId], [OrderType], [OrderDate],  [FromVesselInCompanyId], [ToVesselInCompanyId],State) " +
            //    " VALUES (2,2,'code1', 3, NULL, NULL, 1, 1, '2013-11-25',  NULL, NULL,0)" +
            //    " SET IDENTITY_INSERT[Fuel].[Order] Off " +
            //    " SET IDENTITY_INSERT [Fuel].[OrderItems] ON " +
            //    " INSERT [Fuel].[OrderItems]([Id],[Description], [Quantity], [OrderId], [GoodId], [MeasuringUnitId], [GoodPartyAssignmentId],Received,Invoiced)  VALUES (1,'desc', 100,1,1,1,1,50,10)" +
            //    " INSERT [Fuel].[OrderItems]([Id],[Description], [Quantity], [OrderId], [GoodId], [MeasuringUnitId], [GoodPartyAssignmentId],Received,Invoiced)  VALUES (2,'desc', 100,1,2,4,1,50,10)" +
            //    " INSERT [Fuel].[OrderItems]([Id],[Description], [Quantity], [OrderId], [GoodId], [MeasuringUnitId], [GoodPartyAssignmentId],Received,Invoiced)  VALUES (3,'desc', 100,2,1,1,1,50,10)" +
            //    " SET IDENTITY_INSERT [Fuel].[OrderItems] Off ");

            //context.SaveChanges();

            //var orderWorkflowLog1 = new OrderWorkflowLog(1, WorkflowEntities.Order, DateTime.Now.AddDays(-2), WorkflowActions.Approve, 1, "", initialOrderStep.Id, true);
            //var orderWorkflowLog2 = new OrderWorkflowLog(2, WorkflowEntities.Order, DateTime.Now.AddDays(-2), WorkflowActions.Approve, 1, "", initialOrderStep.Id, true);
            //context.ApproveFlowWorks.AddOrUpdate(orderWorkflowLog1, orderWorkflowLog2);
            //context.SaveChanges(); 
        }

        private void insertInvoiceFakeData(long userId, DataContainer context)
        {

            //context.Database.ExecuteSqlCommand(
            //    "Delete From  [Fuel].[Invoice]");

            //context.Database.ExecuteSqlCommand(
            //    "Delete From  [Fuel].[InvoiceItems]");

            //context.Database.ExecuteSqlCommand(
            //    " SET IDENTITY_INSERT[Fuel].[Invoice] ON " +

            //    " INSERT INTO [Fuel].[Invoice](Id,[InvoiceDate],[CurrencyId],[State],[Description],[DivisionMethod],[InvoiceNumber],[InvoiceType],[TransporterId],[SupplierId],[AccountingType],[OwnerId],[InvoiceRefrenceId],IsCreditor,TotalOfDivisionPrice)"
            //    + "                  VALUES(1,'2013-12-15',1,0,'decription1',1,'invoice01',0,NULL,2,0,1,NULL,0,0)" +
            //    " INSERT INTO [Fuel].[Invoice](Id,[InvoiceDate],[CurrencyId],[State],[Description],[DivisionMethod],[InvoiceNumber],[InvoiceType],[TransporterId],[SupplierId],[AccountingType],[OwnerId],[InvoiceRefrenceId],IsCreditor,TotalOfDivisionPrice) "
            //     + "                  VALUES(2,'2013-12-16',1,0,'decription2',1,'invoice02',0,NULL,2,0,1,NULL,0,0)" +
            //   " SET IDENTITY_INSERT [Fuel].[Invoice] Off " +
            //    " SET IDENTITY_INSERT[Fuel].[InvoiceItems] ON" +

            //    " INSERT INTO [Fuel].[InvoiceItems](id,[Quantity],[Fee],DivisionPrice,[InvoiceId],[GoodId],[MeasuringUnitId],[Description]) VALUES(1,10,100,50,1,1,1,'desctiption1')" +
            //    " INSERT INTO [Fuel].[InvoiceItems](id,[Quantity],[Fee],DivisionPrice,[InvoiceId],[GoodId],[MeasuringUnitId],[Description]) VALUES(2,10,100,50,1,2,4,'desctiption2')" +
            //    " INSERT INTO [Fuel].[InvoiceItems](id,[Quantity],[Fee],DivisionPrice,[InvoiceId],[GoodId],[MeasuringUnitId],[Description]) VALUES(3,10,100,50,2,1,1,'desctiption3')" +


            //   " SET IDENTITY_INSERT [Fuel].[InvoiceItems] Off ");

            //var invoiceWorkflowLog1 = new InvoiceWorkflowLog(1, WorkflowEntities.Invoice, DateTime.Now.AddDays(-2), WorkflowActions.Approve, 1, "", initialInvoiceStep.Id, true);
            //var invoiceWorkflowLog2 = new InvoiceWorkflowLog(2, WorkflowEntities.Invoice, DateTime.Now.AddDays(-2), WorkflowActions.Approve, 1, "", initialInvoiceStep.Id, true);
            //context.ApproveFlowWorks.AddOrUpdate(invoiceWorkflowLog1, invoiceWorkflowLog2);
            //context.SaveChanges();

            //context.Invoices.Single(c => c.Id == 1).OrderRefrences.Add(context.Orders.Single(c => c.Id == 1));
            //context.Invoices.Single(c => c.Id == 2).OrderRefrences.Add(context.Orders.Single(c => c.Id == 2));

            //context.OrderItemBalance.Add(new OrderItemBalance(context.Orders.Single(c => c.Id == 1).OrderItems.Single(c => c.Id == 1), context.Invoices.SingleOrDefault(c => c.Id == 1).InvoiceItems.Single(c => c.Id == 1), 10));
            //context.OrderItemBalance.Add(new OrderItemBalance(context.Orders.Single(c => c.Id == 1).OrderItems.Single(c => c.Id == 2), context.Invoices.SingleOrDefault(c => c.Id == 1).InvoiceItems.Single(c => c.Id == 2), 10));
            //context.OrderItemBalance.Add(new OrderItemBalance(context.Orders.Single(c => c.Id == 2).OrderItems.Single(c => c.Id == 3), context.Invoices.SingleOrDefault(c => c.Id == 2).InvoiceItems.Single(c => c.Id == 3), 10));

        }

        private static void insertInvoiceWorkflowConfig(long userId, DataContainer context)
        {
            //Workflow Config
            var initialInvoiceStep = new WorkflowStep(WorkflowEntities.Invoice, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);
            var approveWithApproveInvoice = new WorkflowStep(WorkflowEntities.Invoice, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var approveWithRejectInvoice = new WorkflowStep(WorkflowEntities.Invoice, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);
            var submitWithApproveInvoice = new WorkflowStep(WorkflowEntities.Invoice, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Approve, null);
            var submitWithCancelInvoice = new WorkflowStep(WorkflowEntities.Invoice, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Cancel, null);
            var cancelInvoiceStage = new WorkflowStep(WorkflowEntities.Invoice, userId, States.Cancelled, WorkflowStages.Canceled, WorkflowActions.Cancel, null);

            context.ApproveFlows.AddOrUpdate(initialInvoiceStep, approveWithApproveInvoice, approveWithRejectInvoice, submitWithApproveInvoice, submitWithCancelInvoice, cancelInvoiceStage);
            context.SaveChanges();

            initialInvoiceStep.NextWorkflowStep = approveWithApproveInvoice;
            approveWithApproveInvoice.NextWorkflowStep = submitWithApproveInvoice;
            approveWithRejectInvoice.NextWorkflowStep = initialInvoiceStep;

            submitWithCancelInvoice.NextWorkflowStep = cancelInvoiceStage;

            context.ApproveFlows.AddOrUpdate(initialInvoiceStep, approveWithApproveInvoice, approveWithRejectInvoice, submitWithApproveInvoice, submitWithCancelInvoice);
            context.SaveChanges();
        }

        private static void insertOrderWorkflowConfig(long userId, DataContainer context)
        {
            //Workflow Config
            var initialOrderStep = new WorkflowStep(WorkflowEntities.Order, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);
            var approveWithApproveOrder = new WorkflowStep(WorkflowEntities.Order, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var approveWithRejectOrder = new WorkflowStep(WorkflowEntities.Order, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);
            var submitWithApproveOrder = new WorkflowStep(WorkflowEntities.Order, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Approve, null);
            var submitWithCancelOrder = new WorkflowStep(WorkflowEntities.Order, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Cancel, null);
            var canceOrderlStage = new WorkflowStep(WorkflowEntities.Order, userId, States.Cancelled, WorkflowStages.Canceled, WorkflowActions.Cancel, null);

            context.ApproveFlows.AddOrUpdate(initialOrderStep, approveWithApproveOrder, approveWithRejectOrder, submitWithApproveOrder, submitWithCancelOrder, canceOrderlStage);
            context.SaveChanges();

            initialOrderStep.NextWorkflowStep = approveWithApproveOrder;
            approveWithApproveOrder.NextWorkflowStep = submitWithApproveOrder;
            approveWithRejectOrder.NextWorkflowStep = initialOrderStep;

            submitWithCancelOrder.NextWorkflowStep = canceOrderlStage;

            context.ApproveFlows.AddOrUpdate(initialOrderStep, approveWithApproveOrder, approveWithRejectOrder, submitWithApproveOrder, submitWithCancelOrder);
            context.SaveChanges();
        }

        //private void insertBasicInfo(DataContainer context)
        //{
        //    context.Users.AddOrUpdate(FakeDomainService.GetUsers().ToArray());

        //    context.Currencies.AddOrUpdate(FakeDomainService.GetCurrencies().ToArray());

        //    context.Companies.AddOrUpdate(FakeDomainService.GetCompanies().ToArray());

        //    context.Units.AddOrUpdate(FakeDomainService.GetUnits().ToArray());

        //    context.SharedGoods.AddOrUpdate(FakeDomainService.GetShareGoods().ToArray());

        //    context.Goods.AddOrUpdate(FakeDomainService.GetGoods().ToArray());

        //    context.GoodUnits.AddOrUpdate(FakeDomainService.GetCompanyGoodUnits().ToArray());

        //    //context.GoodPartyAssignments.AddOrUpdate(FakeDomainService.GetGoodPartyAssignments().ToArray());

        //    //insertActivityLocations(context);

        //}

        private WorkflowStep insertOffhireWorkflowConfig(DataContainer context, long userId)
        {
            //Workflow Config
            var initialOffhireStep = new WorkflowStep(WorkflowEntities.Offhire, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);

            var middleApproveOffhireStep = new WorkflowStep(WorkflowEntities.Offhire, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var middleRejectOffhireStep = new WorkflowStep(WorkflowEntities.Offhire, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);

            var rejectingSubmittedOffhireStep = new WorkflowStep(WorkflowEntities.Offhire, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Reject, null);
            var submittingRejectedOffhireStep = new WorkflowStep(WorkflowEntities.Offhire, userId, States.SubmitRejected, WorkflowStages.SubmitRejected, WorkflowActions.Approve, null);

            context.ApproveFlows.AddOrUpdate(initialOffhireStep,
                middleApproveOffhireStep, middleRejectOffhireStep,
                rejectingSubmittedOffhireStep, submittingRejectedOffhireStep);
            context.SaveChanges();

            initialOffhireStep.NextWorkflowStep = middleApproveOffhireStep;

            middleApproveOffhireStep.NextWorkflowStep = rejectingSubmittedOffhireStep;
            middleRejectOffhireStep.NextWorkflowStep = initialOffhireStep;

            rejectingSubmittedOffhireStep.NextWorkflowStep = submittingRejectedOffhireStep;
            submittingRejectedOffhireStep.NextWorkflowStep = rejectingSubmittedOffhireStep;

            context.ApproveFlows.AddOrUpdate(initialOffhireStep,
                 middleApproveOffhireStep, middleRejectOffhireStep,
                 rejectingSubmittedOffhireStep, submittingRejectedOffhireStep);
            context.SaveChanges();

            return initialOffhireStep;
        }

        private void insertFuelReportsFakeData(DataContainer context, long userId, long workflowStepId)
        {
            //Fake Data
            context.FuelReports.AddOrUpdate(FakeDomainService.GetFuelReports().ToArray());

            context.SaveChanges();

            context.FuelReportDetails.AddOrUpdate(FakeDomainService.GetFuelReportDetails(context.FuelReports.ToList()).ToArray());

            context.SaveChanges();

            foreach (FuelReport fuelReport in context.FuelReports)
            {
                fuelReport.ApproveWorkFlows.Add(new FuelReportWorkflowLog(fuelReport.Id, WorkflowEntities.FuelReport, fuelReport.ReportDate.AddHours(5), WorkflowActions.Approve, 1, "", workflowStepId, true));
            }

            context.SaveChanges();
        }

        private static WorkflowStep insertFuelReportWorkflowConfig(DataContainer context, long userId)
        {
            //Workflow Config
            var initialToApprovedFuelReportStep = new WorkflowStep(WorkflowEntities.FuelReport, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);
            var approvedToSubmittedFuelReportStep = new WorkflowStep(WorkflowEntities.FuelReport, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var approvedToInitialFuelReport = new WorkflowStep(WorkflowEntities.FuelReport, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);

            var submittedToSubmitRejectedFuelReport = new WorkflowStep(WorkflowEntities.FuelReport, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Reject, null);
            var submitRejectedToSubmittedFuelReport = new WorkflowStep(WorkflowEntities.FuelReport, userId, States.SubmitRejected, WorkflowStages.SubmitRejected, WorkflowActions.Approve, null);

            context.ApproveFlows.AddOrUpdate(initialToApprovedFuelReportStep, approvedToSubmittedFuelReportStep, approvedToInitialFuelReport, submittedToSubmitRejectedFuelReport, submitRejectedToSubmittedFuelReport);
            context.SaveChanges();

            initialToApprovedFuelReportStep.NextWorkflowStep = approvedToSubmittedFuelReportStep;
            approvedToSubmittedFuelReportStep.NextWorkflowStep = submittedToSubmitRejectedFuelReport;
            approvedToInitialFuelReport.NextWorkflowStep = initialToApprovedFuelReportStep;

            submitRejectedToSubmittedFuelReport.NextWorkflowStep = submittedToSubmitRejectedFuelReport;
            submittedToSubmitRejectedFuelReport.NextWorkflowStep = submitRejectedToSubmittedFuelReport;

            context.ApproveFlows.AddOrUpdate(initialToApprovedFuelReportStep, approvedToSubmittedFuelReportStep, approvedToInitialFuelReport, submittedToSubmitRejectedFuelReport, submitRejectedToSubmittedFuelReport);
            context.SaveChanges();
            return initialToApprovedFuelReportStep;
        }

        private void insertScrapsFakeData(DataContainer context, long userId, long workflowStepId)
        {

            return;


            //Fake Data
            context.Scraps.AddOrUpdate(
                new Scrap[]
                {
                    Construct<Scrap>(context.VesselsInCompanies.ToList()[0], context.Companies.ToList()[0], DateTime.Now),
                    //Construct<Scrap>(context.VesselsInCompanies.ToList()[1], context.Companies.ToList()[0], DateTime.Now)
                }
                );

            context.SaveChanges();

            context.ScrapDetails.AddOrUpdate(new ScrapDetail[]
                {
                    Construct<ScrapDetail>((double)234, (double)560, context.Currencies.ToList()[0], context.Goods.ToList()[1], context.GoodUnits.ToList()[0] ,context.Tanks.ToList()[0], context.Scraps.ToList()[0]),
                   Construct<ScrapDetail> ((double)125, (double)895, context.Currencies.ToList()[0], context.Goods.ToList()[0], context.GoodUnits.ToList()[0] ,context.Tanks.ToList()[0], context.Scraps.ToList()[0])
                                              
                });

            context.SaveChanges();


            var scraps = context.Scraps;

            foreach (Scrap scrap in scraps)
            {
                scrap.ApproveWorkflows.Add(new ScrapWorkflowLog(scrap, WorkflowEntities.Scrap, DateTime.Now.AddHours(5), WorkflowActions.Approve, 1, "", workflowStepId, true));
            }

            context.SaveChanges();

            int i = 1;
            foreach (var scrap in scraps)
            {
                scrap.InventoryOperations.AddRange(new InventoryOperation[]
                    {
                     new InventoryOperation(
                     "INV#" + i + " - " +scrap.ScrapDate,
                        DateTime.Now,
                        InventoryActionType.Issue,
                        (long? )null,
                        (long? )null),
                     new InventoryOperation(
                        "INV#" + i++ + " - " +scrap.ScrapDate,
                        DateTime.Now,
                        InventoryActionType.Receipt,
                        (long? )null,
                        (long? )null)
                    });
            }

            context.SaveChanges();
        }

        private WorkflowStep insertScrapWorkflowConfig(DataContainer context, long userId)
        {
            //Workflow Config
            var initialScrapStep = new WorkflowStep(WorkflowEntities.Scrap, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);

            var middleApproveScrapStep = new WorkflowStep(WorkflowEntities.Scrap, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var middleRejectScrapStep = new WorkflowStep(WorkflowEntities.Scrap, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);

            var rejectingSubmittedScrapStep = new WorkflowStep(WorkflowEntities.Scrap, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Reject, null);
            var submittingRejectedScrapStep = new WorkflowStep(WorkflowEntities.Scrap, userId, States.SubmitRejected, WorkflowStages.SubmitRejected, WorkflowActions.Approve, null);

            context.ApproveFlows.AddOrUpdate(initialScrapStep,
                                             middleApproveScrapStep, middleRejectScrapStep,
                                             rejectingSubmittedScrapStep, submittingRejectedScrapStep);
            context.SaveChanges();

            initialScrapStep.NextWorkflowStep = middleApproveScrapStep;

            middleApproveScrapStep.NextWorkflowStep = rejectingSubmittedScrapStep;
            middleRejectScrapStep.NextWorkflowStep = initialScrapStep;

            rejectingSubmittedScrapStep.NextWorkflowStep = submittingRejectedScrapStep;
            submittingRejectedScrapStep.NextWorkflowStep = rejectingSubmittedScrapStep;

            context.ApproveFlows.AddOrUpdate(initialScrapStep,
                                             middleApproveScrapStep, middleRejectScrapStep,
                                             rejectingSubmittedScrapStep, submittingRejectedScrapStep);
            context.SaveChanges();
            return initialScrapStep;
        }

        //private void insertActivityLocations(DataContainer context)
        //{
        //    context.ActivityLocations.Add(new ActivityLocation()
        //    {
        //        Id = 1,
        //        Code = "IRBND",
        //        Abbreviation = "B. Abbas",
        //        Name = "Bandar Abbas",
        //        Latitude = 122.348670959473,
        //        Longitude = 47.619930267334
        //    });
        //    context.ActivityLocations.Add(new ActivityLocation()
        //    {
        //        Id = 2,
        //        Code = "IRBKM",
        //        Abbreviation = "B.I.K",
        //        Name = "Bandar Khomeini",
        //        Latitude = 122.396670959473,
        //        Longitude = 47.999930267334
        //    });

        //    context.SaveChanges();
        //}

        private void insertCharterInFakeData(DataContainer context, long userId, long workflowStepId)
        {
            return;
            //Fake Data
            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [Fuel].[Charter] ON \n" +
                                                   "INSERT INTO [Fuel].[Charter] ([Id], [CurrentState], [ActionDate], [CharterType], [CharterEndType], [ChartererId], [OwnerId], [VesselInCompanyId], [CurrencyId]) VALUES (1, 4, N'2014-01-01 08:00:00', 1, 3, 1, 4, 1, 1) \n" +
                                                   " SET IDENTITY_INSERT [Fuel].[Charter] OFF ");

            context.Database.ExecuteSqlCommand("INSERT INTO [Fuel].[CharterIn] ([Id], [OffHirePricingType]) VALUES (1, 1) ");

            //context.CharterIns.Add(
            //    Construct<CharterIn>(
            //        new Type[] { typeof(long), typeof(long), typeof(long), typeof(long), typeof(long), typeof(DateTime), typeof(List<CharterItem>), typeof(List<InventoryOperation>), typeof(CharterType), typeof(CharterEndType), typeof(OffHirePricingType) },
            //        new object[]{ 0, context.Companies.Single(c=>c.Code=="SAPID").Id, context.Companies.Single(c=>c.Code=="IRISL").Id, context.Vessels.First(v=>v.Company.Code == "SAPID" && v.VesselState == VesselStates.CharterIn).Id, 
            //        context.Currencies.First().Id, new DateTime(2014, 1, 10), null, null, CharterType.Start, CharterEndType.CharteroutEnd, OffHirePricingType.CharterPartyBase}));


            context.SaveChanges();

            context.InventoryOperations.Add(new InventoryOperation("INV" + DateTime.Now.ToString("yyyyMMddHHmmss"), DateTime.Now, InventoryActionType.Receipt, null, 1));

            context.SaveChanges();

            var vesselInCompanyId = context.CharterIns.First().VesselInCompanyId;
            var vesselInCompany = context.VesselsInCompanies.First(v => v.Id == vesselInCompanyId);

            var vesselInInventory = context.VesselsInInventory.First(v => v.Code == vesselInCompany.Code && v.CompanyId == vesselInCompany.CompanyId);


            var charterId = context.CharterIns.First().Id;
            var tankId = context.Tanks.First(t => t.VesselInInventoryId == vesselInInventory.Id).Id;
            var good = context.Goods.First();
            var good2 = context.Goods.First(g => g.Id != good.Id);

            context.CharterItems.Add(
                    new CharterItem(
                            0, charterId, 300, (decimal)800, (decimal)700, good.Id,
                            tankId, good.GoodUnits[0].Id));

            context.CharterItems.Add(
                    new CharterItem(
                            0, charterId, 150, (decimal)1200, (decimal)1100, good2.Id,
                            tankId, good2.GoodUnits[0].Id));

            context.SaveChanges();

            var charterIns = context.CharterIns.ToList();
            foreach (CharterIn charterIn in charterIns)
            {
                charterIn.SetStateType(States.Submitted);
                context.SaveChanges();
                charterIn.ApproveWorkflows.Add(new CharterWorkflowLog(charterIn, WorkflowEntities.CharterIn, charterIn.ActionDate.AddHours(5), WorkflowActions.Approve, 1, "", workflowStepId, true));
            }

            context.SaveChanges();

        }

        private WorkflowStep insertCharterInWorkflowConfig(DataContainer context, long userId)
        {
            //Workflow Config
            var initialCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);

            var middleApproveCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var middleRejectCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);

            var rejectingSubmittedCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Reject, null);
            var submittingRejectedCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.SubmitRejected, WorkflowStages.SubmitRejected, WorkflowActions.Approve, null);
            //var closingSubmittedCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Approve, null);
            //var closedCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Closed, WorkflowStages.Closed, WorkflowActions.None, null);

            context.ApproveFlows.AddOrUpdate(initialCharterInStep,
                                             middleApproveCharterInStep, middleRejectCharterInStep,
                                             rejectingSubmittedCharterInStep, submittingRejectedCharterInStep);
            context.SaveChanges();

            initialCharterInStep.NextWorkflowStep = middleApproveCharterInStep;

            middleApproveCharterInStep.NextWorkflowStep = rejectingSubmittedCharterInStep;
            middleRejectCharterInStep.NextWorkflowStep = initialCharterInStep;

            rejectingSubmittedCharterInStep.NextWorkflowStep = submittingRejectedCharterInStep;
            submittingRejectedCharterInStep.NextWorkflowStep = rejectingSubmittedCharterInStep;

            context.ApproveFlows.AddOrUpdate(initialCharterInStep,
                                             middleApproveCharterInStep, middleRejectCharterInStep,
                                             rejectingSubmittedCharterInStep, submittingRejectedCharterInStep);
            context.SaveChanges();
            return initialCharterInStep;
        }

        private WorkflowStep insertCharterOutWorkflowConfig(DataContainer context, long userId)
        {
            //Workflow Config
            var initialCharterInStep = new WorkflowStep(WorkflowEntities.CharterOut, userId, States.Open, WorkflowStages.Initial, WorkflowActions.Approve, null);

            var middleApproveCharterInStep = new WorkflowStep(WorkflowEntities.CharterOut, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Approve, null);
            var middleRejectCharterInStep = new WorkflowStep(WorkflowEntities.CharterOut, userId, States.Open, WorkflowStages.Approved, WorkflowActions.Reject, null);

            var rejectingSubmittedCharterInStep = new WorkflowStep(WorkflowEntities.CharterOut, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Reject, null);
            var submittingRejectedCharterInStep = new WorkflowStep(WorkflowEntities.CharterOut, userId, States.SubmitRejected, WorkflowStages.SubmitRejected, WorkflowActions.Approve, null);
            //var closingSubmittedCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Submitted, WorkflowStages.Submited, WorkflowActions.Approve, null);
            //var closedCharterInStep = new WorkflowStep(WorkflowEntities.CharterIn, userId, States.Closed, WorkflowStages.Closed, WorkflowActions.None, null);

            context.ApproveFlows.AddOrUpdate(initialCharterInStep,
                middleApproveCharterInStep, middleRejectCharterInStep,
                rejectingSubmittedCharterInStep, submittingRejectedCharterInStep);
            context.SaveChanges();

            initialCharterInStep.NextWorkflowStep = middleApproveCharterInStep;

            middleApproveCharterInStep.NextWorkflowStep = rejectingSubmittedCharterInStep;
            middleRejectCharterInStep.NextWorkflowStep = initialCharterInStep;

            rejectingSubmittedCharterInStep.NextWorkflowStep = submittingRejectedCharterInStep;
            submittingRejectedCharterInStep.NextWorkflowStep = rejectingSubmittedCharterInStep;

            context.ApproveFlows.AddOrUpdate(initialCharterInStep,
                 middleApproveCharterInStep, middleRejectCharterInStep,
                 rejectingSubmittedCharterInStep, submittingRejectedCharterInStep);
            context.SaveChanges();

            return initialCharterInStep;
        }



        private static T Construct<T>(Type[] paramTypes, object[] paramValues)
        {
            //A method to Construct objects via private constructors to bypass business rules checkings.
            Type t = typeof(T);

            ConstructorInfo ci = t.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null, paramTypes, null);

            return (T)ci.Invoke(paramValues);
        }

        private static T Construct<T>(params object[] paramValues)
        {
            //A method to Construct objects via private constructors to bypass business rules checkings.
            var paramTypes = paramValues.Select(v => v.GetType()).ToArray();
            return Construct<T>(paramTypes, paramValues);
        }

        public override void Down()
        {
            //Nothing to do.
        }
    }
}

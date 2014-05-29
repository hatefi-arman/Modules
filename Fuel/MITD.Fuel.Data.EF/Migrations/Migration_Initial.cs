using System.Data;
using FluentMigrator;

namespace MITD.Fuel.Data.EF.Migrations
{
    [Migration(1)]
    public class Migration_Initial : Migration
    {

        public override void Up()
        {
            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\BasicInfoViews_Create.sql");

            //Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\Create SAPID-HAFIZ Rotation Linked Servers.sql");
            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\Create SAPID-HAFIZ Voyages Views.sql");

            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\Create Inventory BasicInfo Views.sql");

            Create.Schema("Fuel");

            Create.Table("Vessel").InSchema("Fuel")
                    .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                    .WithColumn("Code").AsString(50).NotNullable().Indexed()
                    .WithColumn("OwnerId").AsInt64().NotNullable().Indexed()
                    .WithColumn("RowVersion").AsCustom("RowVersion");

            Create.Table("VesselInCompany").InSchema("Fuel")
                    .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                    .WithColumn("Name").AsString(200).Nullable().Indexed()
                    .WithColumn("Description").AsString(2000).Nullable().Indexed()
                    .WithColumn("CompanyId").AsInt64().NotNullable().Indexed()
                    .WithColumn("VesselId").AsInt64().NotNullable()
                        .ForeignKey("FK_VesselInCompany_VesselId_Vessel_Id", "Fuel", "Vessel", "Id")
                    .WithColumn("VesselStateCode").AsInt32().NotNullable()
                    .WithColumn("RowVersion").AsCustom("RowVersion");

            Create.Table("Voyage").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().PrimaryKey()
                  .WithColumn("VoyageNumber").AsString(200)
                  .WithColumn("Description").AsString(200)
                  .WithColumn("VesselInCompanyId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_Voyage_VesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id")
                  .WithColumn("CompanyId").AsInt64().NotNullable().Indexed()
                  .WithColumn("StartDate").AsDateTime().NotNullable()
                  .WithColumn("EndDate").AsDateTime().NotNullable()
                  .WithColumn("IsActive").AsBoolean().NotNullable();


            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\CreateVoyagesView.sql");


            Create.Table("ApproveFlowConfig").InSchema("Fuel")
                    .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                    .WithColumn("ActorUserId").AsInt64().NotNullable().Indexed()
                    .WithColumn("NextWorkflowStepId").AsInt64().Nullable().Indexed()
                        .ForeignKey("FK_ApproveFlowConfig_NextWorkflowStepId_ApproveFlowConfig_Id", "Fuel", "ApproveFlowConfig", "Id")
                    .WithColumn("WithWorkflowAction").AsInt32().NotNullable()
                    .WithColumn("State").AsInt32().NotNullable()
                    .WithColumn("WorkflowEntity").AsInt32().NotNullable()
                    .WithColumn("CurrentWorkflowStage").AsInt32().NotNullable()
                    .WithColumn("RowVersion").AsCustom("RowVersion");



            Create.Table("Invoice").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("InvoiceDate").AsDateTime().NotNullable()
                  .WithColumn("CurrencyId").AsInt64().NotNullable().Indexed()
                  .WithColumn("State").AsInt32().NotNullable()
                  .WithColumn("Description").AsString().Nullable()
                  .WithColumn("DivisionMethod").AsInt32().NotNullable()
                  .WithColumn("InvoiceNumber").AsString().NotNullable()
                  .WithColumn("AccountingType").AsInt32().NotNullable()
                  .WithColumn("InvoiceRefrenceId").AsInt64().Nullable().Indexed()
                        .ForeignKey("FK_Invoice_InvoiceRefrenceId_Invoice_Id", "Fuel", "Invoice", "Id")
                  .WithColumn("InvoiceType").AsInt32().NotNullable()
                  .WithColumn("TransporterId").AsInt64().Nullable().Indexed()
                  .WithColumn("SupplierId").AsInt64().Nullable().Indexed()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("OwnerId").AsInt64().NotNullable().Indexed()
                  .WithColumn("IsCreditor").AsBoolean().NotNullable()
                  .WithColumn("TotalOfDivisionPrice").AsDecimal(18, 2).NotNullable();



            Create.Table("EffectiveFactor").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("Name").AsString(200)
                  .WithColumn("EffectiveFactorType").AsInt32().NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion");


            Create.Table("InvoiceAdditionalPrices").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("EffectiveFactorId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_InvoiceAdditionalPrices_EffectiveFactorId_EffectiveFactor_Id", "Fuel", "EffectiveFactor", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("Price").AsDecimal(18, 2).NotNullable()
                  .WithColumn("Description").AsString().Nullable()
                  .WithColumn("Divisionable").AsBoolean().NotNullable()
                  .WithColumn("InvoiceId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_InvoiceAdditionalPrices_InvoiceId_Invoice_Id", "Fuel", "Invoice", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("TimeStamp").AsCustom("RowVersion");


            Create.Table("InvoiceItems").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("Quantity").AsDecimal(18, 2).NotNullable()
                  .WithColumn("Fee").AsDecimal(18, 2).NotNullable()
                  .WithColumn("InvoiceId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_InvoiceItems_InvoiceId_Invoice_Id", "Fuel", "Invoice", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("GoodId").AsInt64().NotNullable().Indexed()
                  .WithColumn("MeasuringUnitId").AsInt64().NotNullable().Indexed()
                  .WithColumn("Description").AsString().Nullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("DivisionPrice").AsDecimal(18, 2).NotNullable();



            Create.Table("FuelReport").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("Code").AsString(200)
                  .WithColumn("Description").AsString(2000)
                  .WithColumn("EventDate").AsDateTime().NotNullable()
                  .WithColumn("ReportDate").AsDateTime().NotNullable()
                  .WithColumn("VesselInCompanyId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_FuelReport_VesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id")
                  .WithColumn("VoyageId").AsInt64().Nullable().Indexed()
                        //.ForeignKey("FK_FuelReport_VoyageId_Voyage_Id", "Fuel", "Voyage", "Id")
                  .WithColumn("FuelReportType").AsInt32().NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("State").AsInt32().NotNullable();



            Create.Table("FuelReportDetail").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("FuelReportId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_FuelReportDetail_FuelReportId_FuelReport_Id", "Fuel", "FuelReport", "Id")
                  .WithColumn("GoodId").AsInt64().NotNullable().Indexed()
                  .WithColumn("TankId").AsInt64().Nullable().Indexed()
                  .WithColumn("Consumption").AsDouble().NotNullable()
                  .WithColumn("ROB").AsDouble().NotNullable()
                  .WithColumn("ROBUOM").AsString(50).NotNullable()
                  .WithColumn("Receive").AsDouble().Nullable()
                  .WithColumn("ReceiveType").AsInt32().Nullable()
                  .WithColumn("ReceiveReference_ReferenceType").AsInt32().Nullable()
                  .WithColumn("ReceiveReference_ReferenceId").AsInt64().Nullable()
                  .WithColumn("ReceiveReference_Code").AsString().Nullable()
                  .WithColumn("Transfer").AsDouble().Nullable()
                  .WithColumn("TransferType").AsInt32().Nullable()
                  .WithColumn("TransferReference_ReferenceType").AsInt32().Nullable()
                  .WithColumn("TransferReference_ReferenceId").AsInt64().Nullable()
                  .WithColumn("TransferReference_Code").AsString().Nullable()
                  .WithColumn("Correction").AsDouble().Nullable()
                  .WithColumn("CorrectionPrice").AsDecimal(18, 2).Nullable()
                  .WithColumn("CorrectionType").AsInt32().Nullable()
                  .WithColumn("CorrectionReference_ReferenceType").AsInt32().Nullable()
                  .WithColumn("CorrectionReference_ReferenceId").AsInt64().Nullable()
                  .WithColumn("CorrectionReference_Code").AsString().Nullable()
                  .WithColumn("CorrectionPriceCurrencyId").AsInt64().Nullable().Indexed()
                  .WithColumn("CorrectionPriceCurrencyISOCode").AsString(20).Nullable()
                  .WithColumn("MeasuringUnitId").AsInt64().NotNullable().Indexed()
                  .WithColumn("TimeStamp").AsCustom("RowVersion");



            Create.Table("Order").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("Code").AsString()
                  .WithColumn("Description").AsString().Nullable()
                  .WithColumn("SupplierId").AsInt64().Nullable().Indexed()
                  .WithColumn("ReceiverId").AsInt64().Nullable().Indexed()
                  .WithColumn("TransporterId").AsInt64().Nullable().Indexed()
                  .WithColumn("OwnerId").AsInt64().NotNullable().Indexed()
                  .WithColumn("OrderType").AsInt32().NotNullable()
                  .WithColumn("OrderDate").AsDateTime().NotNullable()
                  .WithColumn("FromVesselInCompanyId").AsInt64().Nullable().Indexed()
                        .ForeignKey("FK_Order_FromVesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id")
                  .WithColumn("ToVesselInCompanyId").AsInt64().Nullable().Indexed()
                        .ForeignKey("FK_Order_ToVesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id")
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("State").AsInt32().NotNullable();


            Create.Table("OrderItems").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("Description").AsString().Nullable()
                  .WithColumn("Quantity").AsDecimal(18, 2).NotNullable()
                  .WithColumn("OrderId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_OrderItems_OrderId_Order_Id", "Fuel", "Order", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("GoodId").AsInt64().NotNullable().Indexed()
                  .WithColumn("MeasuringUnitId").AsInt64().NotNullable().Indexed()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("InvoicedInMainUnit").AsDecimal(18, 2).NotNullable()
                  .WithColumn("ReceivedInMainUnit").AsDecimal(18, 2).NotNullable();



            Create.Table("OrderItemBalances").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("OrderId").AsInt64().NotNullable()
                  .WithColumn("OrderItemId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_OrderItemBalances_OrderItemId_OrderItems_Id", "Fuel", "OrderItems", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("QuantityAmountInMainUnit").AsDecimal(18, 2).NotNullable()
                  .WithColumn("UnitCode").AsString(50).NotNullable()
                  .WithColumn("FuelReportDetailId").AsInt64().Indexed().NotNullable()
                        .ForeignKey("FK_OrderItemBalances_FuelReportDetailId_FuelReportDetail_Id", "Fuel", "FuelReportDetail", "Id")
                  .WithColumn("InvoiceItemId").AsInt64().Indexed().NotNullable()
                        .ForeignKey("FK_OrderItemBalances_InvoiceItemId_InvoiceItems_Id", "Fuel", "InvoiceItems", "Id")
                  .WithColumn("TimeStamp").AsCustom("RowVersion");


            Create.Table("Charter").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("CurrentState").AsInt32().NotNullable()
                  .WithColumn("ActionDate").AsDateTime().NotNullable()
                  .WithColumn("CharterType").AsInt32().NotNullable()
                  .WithColumn("CharterEndType").AsInt32().NotNullable()
                  .WithColumn("ChartererId").AsInt64().Indexed()
                  .WithColumn("OwnerId").AsInt64().Indexed()
                  .WithColumn("VesselInCompanyId").AsInt64().Indexed()
                        .ForeignKey("FK_Charter_VesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id")
                  .WithColumn("CurrencyId").AsInt64().Indexed()
                  .WithColumn("TimeStamp").AsCustom("RowVersion");


            Create.Table("CharterItem").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("CharterId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_CharterItem_CharterId_Charter_Id", "Fuel", "Charter", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("GoodUnitId").AsInt64().NotNullable().Indexed()
                  .WithColumn("GoodId").AsInt64().NotNullable().Indexed()
                  .WithColumn("TankId").AsInt64().Nullable().Indexed()
                  .WithColumn("Rob").AsDecimal(18, 2).NotNullable()
                  .WithColumn("Fee").AsDecimal(18, 2).NotNullable()
                  .WithColumn("OffhireFee").AsDecimal(18, 2).NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion");



            Create.Table("Offhire").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("ReferenceNumber").AsInt64().NotNullable()
                  .WithColumn("StartDateTime").AsDateTime().NotNullable()
                  .WithColumn("EndDateTime").AsDateTime().NotNullable()
                  .WithColumn("IntroducerType").AsInt32().NotNullable()
                  .WithColumn("VoucherDate").AsDateTime().NotNullable()
                  .WithColumn("VoucherCurrencyId").AsInt64().NotNullable()
                  .WithColumn("PricingReference_Number").AsString()
                  .WithColumn("PricingReference_Type").AsInt32().NotNullable()
                  .WithColumn("State").AsInt32().NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("IntroducerId").AsInt64().NotNullable().Indexed()
                  .WithColumn("OffhireLocationId").AsInt64().NotNullable().Indexed()
                  .WithColumn("VesselInCompanyId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_Offhire_VesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id")
                  .WithColumn("VoyageId").AsInt64().Nullable().Indexed();
                        //.ForeignKey("FK_Offhire_Voyage_Id_Voyage_Id", "Fuel", "Voyage", "Id");



            Create.Table("OffhireDetail").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("Quantity").AsDecimal(18, 2).NotNullable()
                  .WithColumn("FeeInVoucherCurrency").AsDecimal(18, 2).NotNullable()
                  .WithColumn("FeeInMainCurrency").AsDecimal(18, 2).NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("GoodId").AsInt64().NotNullable().Indexed()
                  .WithColumn("OffhireId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_OffhireDetail_Offhire_Id_Offhire_Id", "Fuel", "Offhire", "Id")
                  .WithColumn("TankId").AsInt64().Nullable().Indexed()
                  .WithColumn("UnitId").AsInt64().NotNullable().Indexed();



            Create.Table("Scrap").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("ScrapDate").AsDateTime().NotNullable()
                  .WithColumn("State").AsInt32().NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("SecondPartyId").AsInt64().NotNullable().Indexed()
                  .WithColumn("VesselInCompanyId").AsInt64().NotNullable().Indexed()
                      .ForeignKey("FK_Scrap_VesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id");


            Create.Table("ScrapDetail").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("ROB").AsDouble().NotNullable()
                  .WithColumn("Price").AsDouble().NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("CurrencyId").AsInt64().NotNullable().Indexed()
                  .WithColumn("GoodId").AsInt64().NotNullable().Indexed()
                  .WithColumn("TankId").AsInt64().Nullable().Indexed()
                  .WithColumn("UnitId").AsInt64().NotNullable().Indexed()
                  .WithColumn("ScrapId").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_ScrapDetail_Scrap_Id_Scrap_Id", "Fuel", "Scrap", "Id").OnDeleteOrUpdate(Rule.Cascade);


            Create.Table("VoyageLog").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("ReferencedVoyageId").AsInt64().NotNullable().Indexed()
                        //.ForeignKey("FK_VoyageLog_ReferencedVoyageId_Voyage_Id", "Fuel", "Voyage", "Id")
                  .WithColumn("ChangeDate").AsDateTime().NotNullable()
                  .WithColumn("VoyageNumber").AsString(200)
                  .WithColumn("Description").AsString(200)
                  .WithColumn("StartDate").AsDateTime().NotNullable()
                  .WithColumn("EndDate").AsDateTime().NotNullable()
                  .WithColumn("IsActive").AsBoolean().NotNullable()
                  .WithColumn("CompanyId").AsInt64().NotNullable().Indexed()
                  .WithColumn("VesselInCompanyId").AsInt64().NotNullable().Indexed()
                      .ForeignKey("FK_VoyageLog_VesselInCompanyId_VesselInCompany_Id", "Fuel", "VesselInCompany", "Id");



            Create.Table("InvoiceOrders").InSchema("Fuel")
                  .WithColumn("Invoice_Id").AsInt64().NotNullable().PrimaryKey().Indexed()
                        .ForeignKey("FK_InvoiceOrders_Invoice_Id_Invoice_Id", "Fuel", "Invoice", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("Order_Id").AsInt64().NotNullable().PrimaryKey().Indexed()
                        .ForeignKey("FK_InvoiceOrders_Order_Id_Order_Id", "Fuel", "Order", "Id").OnDeleteOrUpdate(Rule.Cascade);


            Create.Table("CharterIn").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_CharterIn_Id_Charter_Id", "Fuel", "Charter", "Id")
                  .WithColumn("OffHirePricingType").AsInt32().NotNullable();



            Create.Table("CharterOut").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Indexed()
                        .ForeignKey("FK_CharterOut_Id_Charter_Id", "Fuel", "Charter", "Id")
                  .WithColumn("OffHirePricingType").AsInt32().NotNullable();

            Create.Table("InventoryOperation").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("ActionNumber").AsString(200)
                  .WithColumn("ActionDate").AsDateTime().NotNullable()
                  .WithColumn("ActionType").AsInt32().NotNullable()
                  .WithColumn("TimeStamp").AsCustom("RowVersion")
                  .WithColumn("FuelReportDetailId").AsInt64().Indexed().Nullable()
                        .ForeignKey("FK_InventoryOperation_FuelReportDetailId_FuelReportDetail_Id", "Fuel", "FuelReportDetail", "Id")
                  .WithColumn("CharterId").AsInt64().Indexed().Nullable()
                        .ForeignKey("FK_InventoryOperation_CharterId_Charter_Id", "Fuel", "Charter", "Id")
                  .WithColumn("Scrap_Id").AsInt64().Indexed().Nullable()
                        .ForeignKey("FK_InventoryOperation_Scrap_Id_Scrap_Id", "Fuel", "Scrap", "Id");


            Create.Table("WorkflowLog").InSchema("Fuel")
                  .WithColumn("Id").AsInt64().NotNullable().Identity().PrimaryKey()
                  .WithColumn("WorkflowEntity").AsInt32().NotNullable()
                  .WithColumn("ActionDate").AsDateTime().NotNullable()
                  .WithColumn("WorkflowAction").AsInt32().Nullable()
                  .WithColumn("ActorUserId").AsInt64().NotNullable().Indexed()
                  .WithColumn("Remark").AsString().Nullable()
                  .WithColumn("Active").AsBoolean().NotNullable()
                  .WithColumn("CurrentWorkflowStepId").AsInt64().NotNullable().Indexed()
                    .ForeignKey("FK_WorkflowLog_CurrentWorkflowStepId_ApproveFlowConfig_Id", "Fuel", "ApproveFlowConfig", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("InvoiceId").AsInt64().Indexed().Nullable()
                    .ForeignKey("FK_WorkflowLog_InvoiceId_Invoice_Id", "Fuel", "Invoice", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("OrderId").AsInt64().Indexed().Nullable()
                    .ForeignKey("FK_WorkflowLog_OrderId_Order_Id", "Fuel", "Order", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("CharterId").AsInt64().Indexed().Nullable()
                    .ForeignKey("FK_WorkflowLog_CharterId_Charter_Id", "Fuel", "Charter", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("FuelReportId").AsInt64().Indexed().Nullable()
                    .ForeignKey("FK_WorkflowLog_FuelReportId_FuelReport_Id", "Fuel", "FuelReport", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("OffhireId").AsInt64().Indexed().Nullable()
                    .ForeignKey("FK_WorkflowLog_OffhireId_Offhire_Id", "Fuel", "Offhire", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("ScrapId").AsInt64().Indexed().Nullable()
                    .ForeignKey("FK_WorkflowLog_ScrapId_Scrap_Id", "Fuel", "Scrap", "Id").OnDeleteOrUpdate(Rule.Cascade)
                  .WithColumn("Discriminator").AsString(128).NotNullable()
                  .WithColumn("RowVersion").AsCustom("RowVersion");

        }

        public override void Down()
        {
            Delete.Table("WorkflowLog").InSchema("Fuel");
            Delete.Table("InventoryOperation").InSchema("Fuel");
            Delete.Table("CharterOut").InSchema("Fuel");
            Delete.Table("CharterIn").InSchema("Fuel");
            Delete.Table("InvoiceOrders").InSchema("Fuel");
            Delete.Table("VoyageLog").InSchema("Fuel");
            Delete.Table("ScrapDetail").InSchema("Fuel");
            Delete.Table("Scrap").InSchema("Fuel");
            Delete.Table("OffhireDetail").InSchema("Fuel");
            Delete.Table("Offhire").InSchema("Fuel");
            Delete.Table("CharterItem").InSchema("Fuel");
            Delete.Table("Charter").InSchema("Fuel");
            Delete.Table("OrderItemBalances").InSchema("Fuel");
            Delete.Table("OrderItems").InSchema("Fuel");
            Delete.Table("Order").InSchema("Fuel");
            Delete.Table("FuelReportDetail").InSchema("Fuel");
            Delete.Table("FuelReport").InSchema("Fuel");
            Delete.Table("InvoiceItems").InSchema("Fuel");
            Delete.Table("InvoiceAdditionalPrices").InSchema("Fuel");
            Delete.Table("EffectiveFactor").InSchema("Fuel");
            Delete.Table("Invoice").InSchema("Fuel");
            Delete.Table("ApproveFlowConfig").InSchema("Fuel");

            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\DropVoyagesView.sql");
            Delete.Table("Voyage").InSchema("Fuel");
            Delete.Table("VesselInCompany").InSchema("Fuel");
            Delete.Table("Vessel").InSchema("Fuel");

            Delete.Schema("Fuel");

            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\Drop Inventory BasicInfo Views.sql");

            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\Drop SAPID-HAFIZ Voyages Views.sql");
            //Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\Drop SAPID-HAFIZ Rotation Linked Servers.sql");

            Execute.Script(@"Fuel\MITD.Fuel.Data.EF\DBQueries\BasicInfoViews_Drop.sql");
        }
    }

    /*
     * #set projectPath = Fuel\MITD.Fuel.Data.EF\MITD.Fuel.Data.EF.csproj
#set msbuildPath = "%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
#%msbuildPath% %projectPath% /t:Rebuild
#Clean`;Build`;

     */
}

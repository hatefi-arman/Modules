using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MITD.Core;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Integration.Inventory.Data.ReversePOCO;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.SqlTypes;
using IsolationLevel = System.Data.IsolationLevel;

namespace MITD.Fuel.Integration.Inventory
{
    public class InventoryOperationManagement : IInventoryOperationManagement
    {
        private enum TransactionActionType
        {
            Receipt = 1,
            Issue = 2,
        }

        #region Constants

        private const string INVENTORY_ADD_ACTION_VALUE = "insert";
        private const string INVENTORY_REMOVE_ACTION_VALUE = "delete";
        private const string INVENTORY_UPDATE_ACTION_VALUE = "update";

        private const long INVALID_ID = -1;

        private const string EOV_EOM_EOY_FUEL_REPORT_DETAIL_CONSUMPTION = "EOV_EOM_EOY_FUEL_REPORT_DETAIL_CONSUMPTION";
        private const string FUEL_REPORT_DETAIL_RECEIVE = "FUEL_REPORT_DETAIL_RECEIVE";
        private const string FUEL_REPORT_DETAIL_TRANSFER = "FUEL_REPORT_DETAIL_TRANSFER";
        private const string FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION = "FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION";
        private const string FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION = "FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION";

        private const string CHARTER_IN_START_RECEIPT = "CHARTER_IN_START_RECEIPT";
        private const string CHARTER_IN_START_RECEIPT_PRICING = "CHARTER_IN_START_RECEIPT_PRICING";
        private const string CHARTER_IN_END_ISSUE = "CHARTER_IN_END_ISSUE";
        private const string CHARTER_IN_END_ISSUE_PRICING = "CHARTER_IN_END_ISSUE_PRICING";
        private const string CHARTER_OUT_START_ISSUE = "CHARTER_OUT_START_ISSUE";
        private const string CHARTER_OUT_START_ISSUE_PRICING = "CHARTER_OUT_START_ISSUE_PRICING";
        private const string CHARTER_OUT_END_RECEIPT = "CHARTER_OUT_END_RECEIPT";
        private const string CHARTER_OUT_END_RECEIPT_PRICING = "CHARTER_OUT_END_RECEIPT_PRICING";

        private const string SCRAP_ISSUE = "SCRAP_ISSUE";
        private const string SCRAP_ISSUE_PRICING = "SCRAP_ISSUE_PRICING";

        private const string FUEL_REPORT_DETAIL_RECEIPT_INVOICE = "FUEL_REPORT_DETAIL_RECEIPT_INVOICE";

        #endregion

        //================================================================================

        private decimal calculateConsumption(FuelReportDetail source)
        {
            if (!(source.FuelReport.FuelReportType == FuelReportTypes.EndOfMonth ||
                source.FuelReport.FuelReportType == FuelReportTypes.EndOfVoyage ||
                source.FuelReport.FuelReportType == FuelReportTypes.EndOfYear))
            {
                return 0;
            }
            else
            {
                var fuelReportDomainService = ServiceLocator.Current.GetInstance<IFuelReportDomainService>();

                var reportingConsumption = fuelReportDomainService.CalculateReportingConsumption(source);

                return reportingConsumption;
            }
        }

        //================================================================================

        #region Inventory Operations

        //================================================================================

        private OperationReference findInvenotryOperationReference(InventoryDbContext dbContext, InventoryOperationType transactionType, string referenceType, string referenceNumber)
        {
            var foundOperationReference = dbContext.OperationReferences.FirstOrDefault(tr => tr.OperationType == (int)transactionType &&
                tr.ReferenceType == referenceType &&
                tr.ReferenceNumber == referenceNumber);

            return foundOperationReference;

            if (foundOperationReference != null)
            {
                return foundOperationReference;
            }

            return new OperationReference()
            {
                ReferenceNumber = referenceNumber,
                ReferenceType = referenceType,
                OperationId = INVALID_ID,
                OperationType = (int)transactionType
            };
        }

        //================================================================================

        private OperationReference issue(InventoryDbContext dbContext, int companyId, int warehouseId, int timeBucketId,
            int storeTypesId, string referenceType, string referenceNumber, int userId, out string code, out string message)
        {
            var transactionIdParameter = new SqlParameter("@TransactionId", SqlDbType.Int, sizeof(int), ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, 0);
            var codeParameter = new SqlParameter("@Code", SqlDbType.Decimal, sizeof(decimal), ParameterDirection.Output, false, 20, 2, "", DataRowVersion.Default, 0);
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

            dbContext.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction, "dbo.[TransactionOperation]",
                               new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                               new SqlParameter("@TransactionAction", (int)TransactionActionType.Issue),
                               new SqlParameter("@CompanyId", companyId),
                               new SqlParameter("@WarehouseId", warehouseId),
                               new SqlParameter("@TimeBucketId", timeBucketId),
                               new SqlParameter("@StoreTypesId", storeTypesId),
                               new SqlParameter("@RegistrationDate", DateTime.Now),
                               new SqlParameter("@ReferenceType", referenceType),
                               new SqlParameter("@ReferenceNo", referenceNumber),
                               new SqlParameter("@UserCreatorId", userId),
                               transactionIdParameter,
                               codeParameter,
                               messageParameter);


            var transactionId = (int)transactionIdParameter.Value;

            code = codeParameter.Value.ToString();

            message = messageParameter.Value as string;

            return addIssueOperationReference(dbContext, referenceType, referenceNumber, transactionId);
        }

        //================================================================================

        private OperationReference receipt(InventoryDbContext dbContext, int companyId, int warehouseId, int timeBucketId,
            int storeTypesId, string referenceType, string referenceNumber, int userId, out string code, out string message)
        {
            var transactionIdParameter = new SqlParameter("@TransactionId", SqlDbType.Int, sizeof(int), ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, 0);
            var codeParameter = new SqlParameter("@Code", SqlDbType.Decimal, sizeof(decimal), ParameterDirection.Output, false, 20, 2, "", DataRowVersion.Default, 0);
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

            dbContext.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                "dbo.[TransactionOperation] @Action=@Action, @TransactionAction=@TransactionAction, @CompanyId=@CompanyId, @WarehouseId=@WarehouseId, @TimeBucketId=@TimeBucketId, @StoreTypesId=@StoreTypesId, @RegistrationDate=@RegistrationDate, @ReferenceType=@ReferenceType, @ReferenceNo=@ReferenceNo, @UserCreatorId=@UserCreatorId, @TransactionId=@TransactionId OUT, @Code=@Code OUT, @Message=@Message OUT",
                               new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                               new SqlParameter("@TransactionAction", (int)TransactionActionType.Receipt),
                               new SqlParameter("@CompanyId", companyId),
                               new SqlParameter("@WarehouseId", warehouseId),
                               new SqlParameter("@TimeBucketId", timeBucketId),
                               new SqlParameter("@StoreTypesId", storeTypesId),
                               new SqlParameter("@RegistrationDate", DateTime.Now),
                               new SqlParameter("@ReferenceType", referenceType),
                               new SqlParameter("@ReferenceNo", referenceNumber),
                               new SqlParameter("@UserCreatorId", userId),
                               transactionIdParameter,
                               codeParameter,
                               messageParameter);


            var transactionId = (int)transactionIdParameter.Value;

            code = codeParameter.Value.ToString();

            message = messageParameter.Value as string;

            return addReceiptOperationReference(dbContext, referenceType, referenceNumber, transactionId);
        }

        //================================================================================

        private void addTransactionItems(InventoryDbContext dbContext, int transactionId, IEnumerable<TransactionItem> transactionItems, int userId, out string message)
        {
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");
            var transactionItemIdsParameter = new SqlParameter("@TransactionItemsId", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

            var transactionItemsTable = new DataTable();

            transactionItemsTable.Columns.AddRange(new DataColumn[]{
                                                            new DataColumn("Id"),
                                                            new DataColumn("GoodId"),
                                                            new DataColumn("QuantityUnitId"),
                                                            new DataColumn("QuantityAmount"),
                                                            new DataColumn("Description")
                                                       });

            foreach (var transactionItem in transactionItems)
            {
                var itemRow = transactionItemsTable.NewRow();

                itemRow["Id"] = null;
                itemRow["GoodId"] = transactionItem.GoodId;
                itemRow["QuantityUnitId"] = transactionItem.QuantityUnitId;
                itemRow["QuantityAmount"] = transactionItem.QuantityAmount;
                itemRow["Description"] = transactionItem.Description;

                transactionItemsTable.Rows.Add(itemRow);
            }

            var transactionItemsParameter = new SqlParameter("@TransactionItems", SqlDbType.Structured, 4096, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, transactionItemsTable);
            transactionItemsParameter.TypeName = "TypeTransactionItems";

            dbContext.Database.ExecuteSqlCommand(
                TransactionalBehavior.EnsureTransaction,
                "dbo.[TransactionItemsOperation] @Action=@Action, @TransactionId=@TransactionId, @UserCreatorId=@UserCreatorId, @TransactionItems=@TransactionItems, @TransactionItemsId=@TransactionItemsId OUT, @Message=@Message OUT",
                               new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                               new SqlParameter("@TransactionId", transactionId),
                               new SqlParameter("@UserCreatorId", userId),
                               transactionItemsParameter,
                               transactionItemIdsParameter,
                               messageParameter);

            message = messageParameter.Value as string;
        }

        //================================================================================

        private void addTransactionItemsPrices(InventoryDbContext dbContext, IEnumerable<TransactionItemPrice> transactionItemsPrices,
            int userId, out string message, string pricingReferenceType, string pricingReferenceNumber)
        {
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");
            var transactionItemPriceIdsParameter = new SqlParameter("@TransactionItemPriceIds", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

            var transactionItemsPricesTable = new DataTable();
            transactionItemsPricesTable.Columns.AddRange(new DataColumn[]
                                                         {
                                                            new DataColumn("Id"),
                                                            new DataColumn("TransactionItemId"),
                                                            new DataColumn("QuantityUnitId"),
                                                            new DataColumn("QuantityAmount"),
                                                            new DataColumn("PriceUnitId"),
                                                            new DataColumn("Fee"),
                                                            new DataColumn("RegistrationDate"),
                                                            new DataColumn("Description")
                                                         });

            foreach (var transactionItemPrice in transactionItemsPrices)
            {
                var itemRow = transactionItemsPricesTable.NewRow();
                itemRow["TransactionItemId"] = transactionItemPrice.TransactionItemId;
                itemRow["QuantityUnitId"] = transactionItemPrice.QuantityUnitId;
                itemRow["QuantityAmount"] = transactionItemPrice.QuantityAmount;
                itemRow["PriceUnitId"] = transactionItemPrice.PriceUnitId;
                itemRow["Fee"] = transactionItemPrice.Fee;
                itemRow["RegistrationDate"] = transactionItemPrice.RegistrationDate;
                itemRow["Description"] = transactionItemPrice.Description;

                transactionItemsPricesTable.Rows.Add(itemRow);
            }

            var transactionItemsPricesParameter = new SqlParameter("@TransactionItemPrices", SqlDbType.Structured, 4096, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, transactionItemsPricesTable);
            transactionItemsPricesParameter.TypeName = "TypeTransactionItemPrices";

            dbContext.Database.ExecuteSqlCommand(
                TransactionalBehavior.EnsureTransaction,
                "dbo.[TransactionItemPricesOperation] @Action=@Action, @UserCreatorId=@UserCreatorId, @TransactionItemPrices=@TransactionItemPrices,@TransactionItemPriceIds=@TransactionItemPriceIds OUT, @Message=@Message OUT",
                               new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                               new SqlParameter("@UserCreatorId", userId),
                               transactionItemsPricesParameter,
                               transactionItemPriceIdsParameter,
                               messageParameter);

            var addedIds = transactionItemPriceIdsParameter.Value.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse);

            foreach (var addedId in addedIds)
            {
                addPricingOperationReference(dbContext, pricingReferenceType, pricingReferenceNumber, addedId);
            }

            message = messageParameter.Value as string;
        }

        //================================================================================

        private void addTransactionItemPrice(InventoryDbContext dbContext, TransactionItemPrice transactionItemPrice,
            int userId, out string message, string pricingReferenceType, string pricingReferenceNumber)
        {
            var transactionItemsPrices = new List<TransactionItemPrice>
                                         {
                                             transactionItemPrice
                                         };

            addTransactionItemsPrices(dbContext, transactionItemsPrices, userId, out message, pricingReferenceType, pricingReferenceNumber);
        }

        //================================================================================

        private OperationReference addPricingOperationReference(InventoryDbContext dbContext, string pricingReferenceType, string pricingReferenceNumber, long operationId)
        {
            var result = dbContext.OperationReferences.Add(new OperationReference()
            {
                OperationId = operationId,
                OperationType = (int)InventoryOperationType.Pricing,
                ReferenceNumber = pricingReferenceNumber,
                ReferenceType = pricingReferenceType
            });

            dbContext.SaveChanges();

            return result;
        }

        //================================================================================

        private OperationReference addIssueOperationReference(InventoryDbContext dbContext, string pricingReferenceType, string pricingReferenceNumber, long operationId)
        {
            var result = dbContext.OperationReferences.Add(new OperationReference()
             {
                 OperationId = operationId,
                 OperationType = (int)InventoryOperationType.Issue,
                 ReferenceNumber = pricingReferenceNumber,
                 ReferenceType = pricingReferenceType
             });

            dbContext.SaveChanges();

            return result;
        }

        //================================================================================

        private OperationReference addReceiptOperationReference(InventoryDbContext dbContext, string pricingReferenceType, string pricingReferenceNumber, long operationId)
        {
            var result = dbContext.OperationReferences.Add(new OperationReference()
            {
                OperationId = operationId,
                OperationType = (int)InventoryOperationType.Receipt,
                ReferenceNumber = pricingReferenceNumber,
                ReferenceType = pricingReferenceType
            });

            dbContext.SaveChanges();

            return result;
        }

        //================================================================================

        private void activateWarehouse(InventoryDbContext dbContext, int warehouseId, int userId)
        {
            dbContext.Database.ExecuteSqlCommand(
                TransactionalBehavior.EnsureTransaction,
                "dbo.[ChangeWarehouseStatus] @Active, @WarehouseId, @UserCreatorId",
                               new SqlParameter("@Active", true),
                               new SqlParameter("@WarehouseId", warehouseId),
                               new SqlParameter("@UserCreatorId", userId));
        }

        //================================================================================

        private void deactivateWarehouse(InventoryDbContext dbContext, int warehouseId, int userId)
        {
            dbContext.Database.ExecuteSqlCommand(
                TransactionalBehavior.EnsureTransaction,
                "dbo.[ChangeWarehouseStatus]",
                               new SqlParameter("@Active", false),
                               new SqlParameter("@WarehouseId", warehouseId),
                               new SqlParameter("@UserCreatorId", userId));
        }

        //================================================================================

        private static int getCurrencyId(InventoryDbContext dbContext, string abbreviation)
        {
            return dbContext.Units.Single(u =>
                 u.IsCurrency.Value == true &&
                 u.Code.ToUpper() == abbreviation.ToUpper()).Id;
        }

        //================================================================================

        private static int getMeasurementUnitId(InventoryDbContext dbContext, string abbreviation)
        {
            return dbContext.Units.Single(u =>
                (u.IsCurrency == null || u.IsCurrency.Value == false)
                && u.Code.ToUpper() == abbreviation.ToUpper()).Id;
        }

        //================================================================================

        #endregion

        //================================================================================

        public List<InventoryOperation> ManageFuelReportDetail(FuelReportDetail fuelReportDetail, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    if (fuelReportDetail.FuelReport.FuelReportType == FuelReportTypes.EndOfMonth ||
                        fuelReportDetail.FuelReport.FuelReportType == FuelReportTypes.EndOfYear ||
                        fuelReportDetail.FuelReport.FuelReportType == FuelReportTypes.EndOfMonth)
                    {
                        var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Issue, EOV_EOM_EOY_FUEL_REPORT_DETAIL_CONSUMPTION, fuelReportDetail.Id.ToString());

                        //if (reference.OperationId == INVALID_ID)
                        if (reference == null)
                        {

                            string transactionCode, transactionMessage;

                            var operationReference =
                                issue(
                                    dbContext,
                                    (int)fuelReportDetail.FuelReport.VesselInCompany.CompanyId,
                                    (int)fuelReportDetail.FuelReport.VesselInCompany.VesselInInventory.Id,
                                    1,
                                    1,
                                    EOV_EOM_EOY_FUEL_REPORT_DETAIL_CONSUMPTION,
                                    fuelReportDetail.Id.ToString(),
                                    userId,
                                    out transactionCode,
                                    out transactionMessage);

                            //TODO: Items
                            string transactionItemMessage;

                            var consumption = calculateConsumption(fuelReportDetail);

                            var transactionItems = new List<TransactionItem>();
                            transactionItems.Add(new TransactionItem()
                                                 {
                                                     GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                                                     CreateDate = DateTime.Now,
                                                     Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                                                     QuantityAmount = consumption,
                                                     QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                                                     TransactionId = (int)operationReference.OperationId,
                                                     UserCreatorId = userId
                                                 });

                            addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                            //TODO: Items Pricing



                            var result = new List<Domain.Model.DomainObjects.InventoryOperation>
                                         {
                                             new InventoryOperation(
                                                 actionNumber : string.Format("{0}/{1}", operationReference.OperationType, operationReference.OperationId),
                                                 actionDate: DateTime.Now,
                                                 actionType: InventoryActionType.Issue,
                                                 fuelReportDetailId:fuelReportDetail.Id,
                                                 charterId: null)
                                         };

                            return result;
                        }
                        else
                        {
                            var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                        }
                    }
                    else
                    {
                        if (fuelReportDetail.Receive.HasValue)
                        {
                            var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIVE, fuelReportDetail.Id.ToString());

                            //if (reference.OperationId == INVALID_ID)
                            if (reference == null)
                            {

                            }
                            else
                            {
                                var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                            }
                        }

                        if (fuelReportDetail.Transfer.HasValue)
                        {
                            var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Issue, FUEL_REPORT_DETAIL_TRANSFER, fuelReportDetail.Id.ToString());

                            //if (reference.OperationId == INVALID_ID)
                            if (reference == null)
                            {

                            }
                            else
                            {
                                var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                            }
                        }

                        if (fuelReportDetail.Correction.HasValue && fuelReportDetail.CorrectionType.HasValue &&
                            fuelReportDetail.CorrectionPrice.HasValue &&
                            !string.IsNullOrWhiteSpace(fuelReportDetail.CorrectionPriceCurrencyISOCode))
                        {
                            if (fuelReportDetail.CorrectionType.Value == CorrectionTypes.Minus)
                            {
                                var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Issue, FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION, fuelReportDetail.Id.ToString());

                                //if (reference.OperationId == INVALID_ID)
                                if (reference == null)
                                {

                                }
                                else
                                {
                                    var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                                }
                            }
                            else
                            {
                                var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION, fuelReportDetail.Id.ToString());

                                //if (reference.OperationId == INVALID_ID)
                                if (reference == null)
                                {

                                }
                                else
                                {
                                    var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        //================================================================================

        public List<InventoryOperation> ManageScrap(Scrap scrap, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Issue, CHARTER_IN_START_RECEIPT, scrap.Id.ToString());

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {

                    }
                    else
                    {
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }
                }
            }
            return null;
        }

        //================================================================================

        public List<InventoryOperation> ManageInvoice(Invoice invoice, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    foreach (var orderItemBalance in invoice.OrderRefrences.SelectMany(o => o.OrderItems.SelectMany(oi => oi.OrderItemBalances)))
                    {
                        //Finding relevant Receipt Item

                        var receiptReference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIVE, orderItemBalance.FuelReportDetailId.ToString());

                        var receiptRegisteredPrices = dbContext.TransactionItemPrices.Where(tip => tip.TransactionItem.TransactionId == (int)receiptReference.OperationId &&
                            tip.TransactionItem.GoodId == (int)orderItemBalance.FuelReportDetail.GoodId);


                        var receiptPriceReferenceNumber = generateOrderItemBalancePricingReferenceNumber(orderItemBalance);

                        var receiptRegisteredPriceReference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIPT_INVOICE, receiptPriceReferenceNumber);

                        receiptRegisteredPrices.FirstOrDefault(p => p.Id == receiptRegisteredPriceReference.OperationId);


                        var referenceNumber = orderItemBalance.FuelReportDetailId;
                    }

                    var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Issue, CHARTER_IN_START_RECEIPT, invoice.Id.ToString());

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {

                    }
                    else
                    {
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }
                }
            }

            return null;
        }

        //================================================================================

        private string generateOrderItemBalancePricingReferenceNumber(Domain.Model.DomainObjects.OrderAggreate.OrderItemBalance orderItemBalance)
        {
            return string.Format("{0},{1}", orderItemBalance.FuelReportDetailId, orderItemBalance.InvoiceItemId);
        }

        //================================================================================

        public List<InventoryOperation> ManageCharterInStart(CharterIn charterInStart, int userId)
        {
            if (charterInStart.CharterType != CharterType.Start)
                throw new InvalidArgument("The given entity is not Charter In Start", "charterInStart");

            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var result = new List<Domain.Model.DomainObjects.InventoryOperation>();

                        var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterInStart.Id.ToString());


                        //if (reference.OperationId == INVALID_ID)
                        if (reference == null)
                        {
                            activateWarehouse(dbContext, (int)charterInStart.VesselInCompany.VesselInInventory.Id, userId);

                            var warehouse = dbContext.Warehouses.Single(w => w.Id == 12);

                            string transactionCode, transactionMessage;

                            var operationReference = receipt(
                                      dbContext,
                                      (int)charterInStart.VesselInCompany.CompanyId,
                                      (int)charterInStart.VesselInCompany.VesselInInventory.Id,
                                      1,
                                      1,
                                      CHARTER_IN_START_RECEIPT,
                                      charterInStart.Id.ToString(),
                                      userId,
                                      out transactionCode,
                                      out transactionMessage);

                            //TODO: Items
                            string transactionItemMessage;

                            var transactionItems = new List<TransactionItem>();

                            foreach (var charterItem in charterInStart.CharterItems)
                            {

                                transactionItems.Add(new TransactionItem()
                                                     {
                                                         GoodId = (int)charterItem.Good.SharedGoodId,
                                                         CreateDate = DateTime.Now,
                                                         Description = "Charter In Start > " + charterItem.Good.Name,
                                                         QuantityAmount = charterItem.Rob,
                                                         QuantityUnitId = getMeasurementUnitId(dbContext, charterItem.GoodUnit.Abbreviation),
                                                         TransactionId = (int)operationReference.OperationId,
                                                         UserCreatorId = userId
                                                     });
                            }

                            addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);


                            //TODO: Items Pricing

                            var registeredTransaction = dbContext.Transactions.Single(t => t.Id == (int)operationReference.OperationId);

                            foreach (var charterItem in charterInStart.CharterItems)
                            {
                                var registeredTransactionItem = registeredTransaction.TransactionItems.Single(ti => ti.GoodId == charterItem.Good.SharedGoodId);

                                var transactionItemPrice = new TransactionItemPrice()
                                    {
                                        TransactionItemId = registeredTransactionItem.Id,
                                        QuantityUnitId = getMeasurementUnitId(dbContext, charterItem.GoodUnit.Abbreviation),
                                        QuantityAmount = charterItem.Rob,
                                        PriceUnitId = getCurrencyId(dbContext, charterInStart.Currency.Abbreviation),
                                        Fee = charterItem.Fee,
                                        RegistrationDate = DateTime.Now,
                                        Description = "Charter In Start Pricing > " + charterItem.Good.Name,
                                        UserCreatorId = userId
                                    };

                                string pricingMessage;

                                addTransactionItemPrice(dbContext, transactionItemPrice, userId, out pricingMessage, CHARTER_IN_START_RECEIPT_PRICING, charterItem.Id.ToString());
                            }
                            
                            result.Add(new InventoryOperation(
                                actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                                actionDate: DateTime.Now,
                                actionType: InventoryActionType.Receipt,
                                fuelReportDetailId:null,
                                charterId: charterInStart.Id));
                        }
                        else
                        {
                            var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                        }

                        //throw new Exception();

                        transaction.Commit();

                        return result;
                    }
                    catch (Exception)
                    {
                        //transaction.Rollback();
                        throw;
                    }
                }
            }

            return null;
        }

        //================================================================================

        public List<InventoryOperation> ManageCharterInEnd(CharterIn charterInEnd, int userId)
        {
            if (charterInEnd.CharterType != CharterType.End)
                throw new InvalidArgument("The given entity is not Charter In End", "charterInEnd");

            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterInEnd.Id.ToString());

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {

                    }
                    else
                    {
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }
                }
            }
            return null;
        }

        //================================================================================

        public List<InventoryOperation> ManageCharterOutStart(CharterOut charterOutStart, int userId)
        {
            if (charterOutStart.CharterType != CharterType.Start)
                throw new InvalidArgument("The given entity is not Charter Out Start", "charterOutStart");

            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterOutStart.Id.ToString());

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {

                    }
                    else
                    {
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }
                }
            }
            return null;
        }

        //================================================================================

        public List<InventoryOperation> ManageCharterOutEnd(CharterOut charterOutEnd, int userId)
        {
            if (charterOutEnd.CharterType != CharterType.End)
                throw new InvalidArgument("The given entity is not Charter Out End", "charterOutEnd");

            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    var reference = findInvenotryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterOutEnd.Id.ToString());

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {

                    }
                    else
                    {
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }
                }
            }
            return null;
        }

        //================================================================================
    }
}

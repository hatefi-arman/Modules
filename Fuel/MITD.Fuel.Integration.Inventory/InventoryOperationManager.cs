using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Transactions;
using System.Linq;
using MITD.Core;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Integration.Inventory.Data.ReversePOCO;
using Transaction = MITD.Fuel.Integration.Inventory.Data.ReversePOCO.Transaction;

namespace MITD.Fuel.Integration.Inventory
{
    public class InventoryOperationManager : IInventoryOperationManager
    {
        public enum TransactionActionType : byte
        {
            Receipt = 1,
            Issue = 2,
            SaleFactor = 3,
        }

        public enum TransactionStatus : byte
        {
            JustRegistered = 1,
            PartialPriced = 2,
            FullPriced = 3,
            Vouchered = 4
        }

        #region Constants

        private const string INVENTORY_ADD_ACTION_VALUE = "insert";
        private const string INVENTORY_REMOVE_ACTION_VALUE = "delete";
        private const string INVENTORY_UPDATE_ACTION_VALUE = "update";

        private const string OPERATION_SUCCESSFUL_MESSAGE = "OperationSuccessful";

        private const long INVALID_ID = -1;

        private const string EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION = "EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION";
        private const string EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION_PRICING = "EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION_PRICING";
        private const string FUEL_REPORT_DETAIL_RECEIVE = "FUEL_REPORT_DETAIL_RECEIVE";
        private const string FUEL_REPORT_DETAIL_TRANSFER = "FUEL_REPORT_DETAIL_TRANSFER";
        private const string FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION = "FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION";
        private const string FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION_PRICING = "FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION_PRICING";
        private const string FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION = "FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION";
        private const string FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION_PRICING = "FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION_PRICING";

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

        private decimal calculateConsumption(FuelReportDetail fuelReportDetail)
        {
            if (!(fuelReportDetail.FuelReport.FuelReportType == FuelReportTypes.EndOfMonth ||
                fuelReportDetail.FuelReport.FuelReportType == FuelReportTypes.EndOfVoyage ||
                fuelReportDetail.FuelReport.FuelReportType == FuelReportTypes.EndOfYear))
            {
                return 0;
            }

            var fuelReportDomainService = ServiceLocator.Current.GetInstance<IFuelReportDomainService>();

            var reportingConsumption = fuelReportDomainService.CalculateReportingConsumption(fuelReportDetail);

            return reportingConsumption;
        }

        //================================================================================

        #region Inventory Operations

        //================================================================================

        private OperationReference findInventoryOperationReference(InventoryDbContext dbContext, InventoryOperationType transactionType, string referenceType, string referenceNumber)
        {
            var foundOperationReference = dbContext.OperationReferences.Where(
                tr =>
                    tr.OperationType == (int)transactionType &&
                    tr.ReferenceType == referenceType &&
                    tr.ReferenceNumber == referenceNumber).ToList().OrderBy(tr => tr.RegistrationDate).LastOrDefault();

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
            int storeTypesId, int? pricingReferenceId, string referenceType, string referenceNumber, int userId, out string code, out string message)
        {
            var transactionIdParameter = new SqlParameter("@TransactionId", SqlDbType.Int, sizeof(int), ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, 0);
            var codeParameter = new SqlParameter("@Code", SqlDbType.Decimal, sizeof(decimal), ParameterDirection.Output, false, 20, 2, "", DataRowVersion.Default, 0);
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");
            try
            {
                dbContext.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                                    "dbo.[TransactionOperation] @Action=@Action, @TransactionAction=@TransactionAction, @CompanyId=@CompanyId, @WarehouseId=@WarehouseId, @TimeBucketId=@TimeBucketId, @StoreTypesId=@StoreTypesId, @PricingReferenceId=@PricingReferenceId, @RegistrationDate=@RegistrationDate, @ReferenceType=@ReferenceType, @ReferenceNo=@ReferenceNo, @UserCreatorId=@UserCreatorId, @TransactionId=@TransactionId OUT, @Code=@Code OUT, @Message=@Message OUT",
                                    new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                                    new SqlParameter("@TransactionAction", (byte)TransactionActionType.Issue),
                                    new SqlParameter("@CompanyId", companyId),
                                    new SqlParameter("@WarehouseId", warehouseId),
                                    new SqlParameter("@TimeBucketId", timeBucketId),
                                    new SqlParameter("@StoreTypesId", storeTypesId),
                                    new SqlParameter("@PricingReferenceId", pricingReferenceId.HasValue ? (object)pricingReferenceId : DBNull.Value),
                                    new SqlParameter("@RegistrationDate", DateTime.Now),
                                    new SqlParameter("@ReferenceType", referenceType),
                                    new SqlParameter("@ReferenceNo", referenceNumber),
                                    new SqlParameter("@UserCreatorId", userId),
                                    transactionIdParameter,
                                    codeParameter,
                                    messageParameter);

            }
            catch (Exception ex)
            {
                throw new InvalidOperation("AddIssue", ex.Message);
            }

            var transactionId = (int)transactionIdParameter.Value;

            code = codeParameter.Value.ToString();

            message = messageParameter.Value as string;

            if (message != OPERATION_SUCCESSFUL_MESSAGE)
                throw new InvalidOperation("AddIssue", message);

            return addIssueOperationReference(dbContext, referenceType, referenceNumber, transactionId);
        }

        //================================================================================

        private OperationReference receipt(InventoryDbContext dbContext, int companyId, int warehouseId, int timeBucketId,
            int storeTypesId, int? pricingReferenceId, string referenceType, string referenceNumber, int userId, out string code, out string message)
        {
            var transactionIdParameter = new SqlParameter("@TransactionId", SqlDbType.Int, sizeof(int), ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, 0);
            var codeParameter = new SqlParameter("@Code", SqlDbType.Decimal, sizeof(decimal), ParameterDirection.Output, false, 20, 2, "", DataRowVersion.Default, 0);
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

            try
            {
                dbContext.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,
                    "dbo.[TransactionOperation] @Action=@Action, @TransactionAction=@TransactionAction, @CompanyId=@CompanyId, @WarehouseId=@WarehouseId, @TimeBucketId=@TimeBucketId, @StoreTypesId=@StoreTypesId, @PricingReferenceId=@PricingReferenceId, @RegistrationDate=@RegistrationDate, @ReferenceType=@ReferenceType, @ReferenceNo=@ReferenceNo, @UserCreatorId=@UserCreatorId, @TransactionId=@TransactionId OUT, @Code=@Code OUT, @Message=@Message OUT",
                                   new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                                   new SqlParameter("@TransactionAction", (byte)TransactionActionType.Receipt),
                                   new SqlParameter("@CompanyId", companyId),
                                   new SqlParameter("@WarehouseId", warehouseId),
                                   new SqlParameter("@TimeBucketId", timeBucketId),
                                   new SqlParameter("@StoreTypesId", storeTypesId),
                                   new SqlParameter("@PricingReferenceId", pricingReferenceId.HasValue ? (object)pricingReferenceId : DBNull.Value),
                                   new SqlParameter("@RegistrationDate", DateTime.Now),
                                   new SqlParameter("@ReferenceType", referenceType),
                                   new SqlParameter("@ReferenceNo", referenceNumber),
                                   new SqlParameter("@UserCreatorId", userId),
                                   transactionIdParameter,
                                   codeParameter,
                                   messageParameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperation("AddReceipt", ex.Message);
            }

            var transactionId = (int)transactionIdParameter.Value;

            code = codeParameter.Value.ToString();

            message = messageParameter.Value as string;

            if (message != OPERATION_SUCCESSFUL_MESSAGE)
                throw new InvalidOperation("AddReceipt", message);

            return addReceiptOperationReference(dbContext, referenceType, referenceNumber, transactionId);
        }

        //================================================================================

        private List<int> addTransactionItems(InventoryDbContext dbContext, int transactionId, IEnumerable<TransactionItem> transactionItems, int userId, out string message)
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

            try
            {
                dbContext.Database.ExecuteSqlCommand(
                    TransactionalBehavior.EnsureTransaction,
                    "dbo.[TransactionItemsOperation] @Action=@Action, @TransactionId=@TransactionId, @UserCreatorId=@UserCreatorId, @TransactionItems=@TransactionItems, @TransactionItemsId=@TransactionItemsId OUT, @Message=@Message OUT",
                                   new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                                   new SqlParameter("@TransactionId", transactionId),
                                   new SqlParameter("@UserCreatorId", userId),
                                   transactionItemsParameter,
                                   transactionItemIdsParameter,
                                   messageParameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperation("AddTransactionItem", ex.Message);
            }

            message = messageParameter.Value as string;

            if (message != OPERATION_SUCCESSFUL_MESSAGE)
                throw new InvalidOperation("AddTransactionItem", message);

            var result = extractIds(transactionItemIdsParameter.Value.ToString());

            return result;
        }

        private List<int> extractIds(string listString)
        {
            return listString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        }

        //================================================================================

        private void priceTransactionItemsManually(InventoryDbContext dbContext, IEnumerable<TransactionItemPrice> transactionItemsPrices,
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
                itemRow["Id"] = null;
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

            try
            {
                dbContext.Database.ExecuteSqlCommand(
                                                     TransactionalBehavior.EnsureTransaction,
                                                     "dbo.[TransactionItemPricesOperation] @Action=@Action, @UserCreatorId=@UserCreatorId, @TransactionItemPrices=@TransactionItemPrices,@TransactionItemPriceIds=@TransactionItemPriceIds OUT, @Message=@Message OUT",
                                                     new SqlParameter("@Action", INVENTORY_ADD_ACTION_VALUE),
                                                     new SqlParameter("@UserCreatorId", userId),
                                                     transactionItemsPricesParameter,
                                                     transactionItemPriceIdsParameter,
                                                     messageParameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperation("AddTransactionItemPrices", ex.Message);
            }

            message = messageParameter.Value as string;

            if (message != OPERATION_SUCCESSFUL_MESSAGE)
                throw new InvalidOperation("AddTransactionItemPrices", message);

            addPricingOperationReferences(dbContext, pricingReferenceType, pricingReferenceNumber, transactionItemPriceIdsParameter.Value.ToString());
        }

        //================================================================================

        private void priceTransactionItemManually(InventoryDbContext dbContext, TransactionItemPrice transactionItemPrice,
            int userId, out string message, string pricingReferenceType, string pricingReferenceNumber)
        {
            var transactionItemsPrices = new List<TransactionItemPrice>
                                         {
                                             transactionItemPrice
                                         };

            priceTransactionItemsManually(dbContext, transactionItemsPrices, userId, out message, pricingReferenceType, pricingReferenceNumber);
        }

        //================================================================================

        private void priceIssuedItemsInFIFO(InventoryDbContext dbContext, IEnumerable<TransactionItemPricingId> transactionItemsIds,
            int userId, out string message, string pricingReferenceType, string pricingReferenceNumber)
        {
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");
            var transactionItemPriceIdsParameter = new SqlParameter("@TransactionItemPriceIds", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");
            var notPricedTransactionIdParameter = new SqlParameter("@NotPricedTransactionId", SqlDbType.Int, sizeof(int), ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, null);

            var transactionItemsPricesTable = new DataTable();
            transactionItemsPricesTable.Columns.AddRange(new DataColumn[]
                                                         {
                                                            new DataColumn("Id"),
                                                            new DataColumn("Description")
                                                         });

            foreach (var transactionItemsId in transactionItemsIds)
            {
                var itemRow = transactionItemsPricesTable.NewRow();
                itemRow["Id"] = transactionItemsId.Id;
                itemRow["Description"] = transactionItemsId.Description;

                transactionItemsPricesTable.Rows.Add(itemRow);
            }

            var issueItemIds = new SqlParameter("@IssueItemIds", SqlDbType.Structured, 4096, ParameterDirection.Input, false, 0, 0, "", DataRowVersion.Default, transactionItemsPricesTable);
            issueItemIds.TypeName = "Ids";

            try
            {
                dbContext.Database.ExecuteSqlCommand(
                    TransactionalBehavior.EnsureTransaction,
                    "dbo.[PriceGivenIssuedItems] @UserCreatorId=@UserCreatorId, @IssueItemIds=@IssueItemIds,@TransactionItemPriceIds=@TransactionItemPriceIds OUT, @Message=@Message OUT, @NotPricedTransactionId=@NotPricedTransactionId OUT",
                                   new SqlParameter("@UserCreatorId", userId),
                                   issueItemIds,
                                   transactionItemPriceIdsParameter,
                                   messageParameter,
                                   notPricedTransactionIdParameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperation("PriceIssuedItemsAutomatically", ex.Message);
            }

            var notPricedTransactionId = notPricedTransactionIdParameter.Value == null ? null : (int?)(int)notPricedTransactionIdParameter.Value;

            if (notPricedTransactionId.HasValue && notPricedTransactionId.Value != 0)
            {
                var transactionCode = dbContext.Transactions.Single(t => t.Id == notPricedTransactionId.Value).Code;

                throw new InvalidOperation("PriceIssuedItemsInFIFO", "The issue pricing procedure reached to a not priced Receipt. Receipt Code : " + transactionCode);
            }

            message = messageParameter.Value as string;

            if (message != OPERATION_SUCCESSFUL_MESSAGE)
                throw new InvalidOperation("PriceIssuedItemsInFIFO", message);

            addPricingOperationReferences(dbContext, pricingReferenceType, pricingReferenceNumber, transactionItemPriceIdsParameter.Value.ToString());
        }

        //================================================================================

        private void priceIssuedItemInFIFO(InventoryDbContext dbContext, int transactionItemId, string description,
            int userId, out string message, string pricingReferenceType, string pricingReferenceNumber)
        {
            var transactionItemsIds = new List<TransactionItemPricingId> { new TransactionItemPricingId() { Id = transactionItemId, Description = description } };

            priceIssuedItemsInFIFO(dbContext, transactionItemsIds, userId,
                out message, pricingReferenceType, pricingReferenceNumber);
        }

        //================================================================================

        private void priceSuspendedTransactionUsingReference(InventoryDbContext dbContext, int transactionId, string description,
            int userId, out string message, string pricingReferenceType, string pricingReferenceNumber)
        {
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");
            var transactionItemPriceIdsParameter = new SqlParameter("@TransactionItemPriceIds", SqlDbType.NVarChar, 4096, ParameterDirection.Output, false, 0, 0, "", DataRowVersion.Default, "");

            try
            {
                dbContext.Database.ExecuteSqlCommand(
                            TransactionalBehavior.EnsureTransaction,
                            "dbo.[PriceSuspendedTransactionUsingReference] @TransactionId=@TransactionId, @Description=@Description, @UserCreatorId=@UserCreatorId, @TransactionItemPriceIds=@TransactionItemPriceIds OUT, @Message=@Message OUT",
                            new SqlParameter("@TransactionId", transactionId),
                            new SqlParameter("@Description", description),
                            new SqlParameter("@UserCreatorId", userId),
                            transactionItemPriceIdsParameter,
                            messageParameter);
            }
            catch (Exception ex)
            {
                throw new InvalidOperation("PriceSuspendedTransactionUsingReference", ex.Message);
            }

            message = messageParameter.Value as string;

            if (message != OPERATION_SUCCESSFUL_MESSAGE)
                throw new InvalidOperation("PriceSuspendedTransactionUsingReference", message);

            addPricingOperationReferences(dbContext, pricingReferenceType, pricingReferenceNumber, transactionItemPriceIdsParameter.Value.ToString());
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

        private IList<OperationReference> addPricingOperationReferences(InventoryDbContext dbContext, string pricingReferenceType, string pricingReferenceNumber, string createdIds)
        {
            var result = new List<OperationReference>();

            var addedIds = createdIds.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse);

            foreach (var addedId in addedIds)
            {
                result.Add(
                    addPricingOperationReference(dbContext, pricingReferenceType, pricingReferenceNumber, addedId));
            }

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
                "dbo.[ChangeWarehouseStatus] @Active, @WarehouseId, @UserCreatorId",
                               new SqlParameter("@Active", false),
                               new SqlParameter("@WarehouseId", warehouseId),
                               new SqlParameter("@UserCreatorId", userId));
        }

        //================================================================================

        private static int getCurrencyId(InventoryDbContext dbContext, string abbreviation)
        {
            return dbContext.Units.Single(u =>
                 u.IsCurrency != null && u.IsCurrency.Value == true &&
                 u.Code.ToUpper() == abbreviation.ToUpper()).Id;
        }

        //================================================================================

        private static int getMeasurementUnitId(InventoryDbContext dbContext, string abbreviation)
        {
            return dbContext.Units.Single(u =>
                (u.IsCurrency == null || u.IsCurrency.Value == false) &&
                u.Code.ToUpper() == abbreviation.ToUpper()).Id;
        }

        //================================================================================

        #endregion

        //================================================================================

        public InventoryOperation ManageFuelReportConsumption(FuelReport fuelReport, int userId)
        {
            if (!(fuelReport.FuelReportType == FuelReportTypes.EndOfVoyage ||
                fuelReport.FuelReportType == FuelReportTypes.EndOfYear ||
                fuelReport.FuelReportType == FuelReportTypes.EndOfMonth))
                throw new InvalidArgument("The given entity is not EOV, EOM, EOY.", "charterOutStart");


            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    InventoryOperation result = null;

                    //TODO: EOV-EOM-EOY
                    #region EOV-EOM-EOY

                    var goodsConsumption = new Dictionary<long, decimal>();

                    foreach (var detail in fuelReport.FuelReportDetails)
                    {
                        var consumption = calculateConsumption(detail);

                        goodsConsumption.Add(detail.GoodId, consumption);
                    }


                    var transactionReferenceNumber = fuelReport.Id.ToString();

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Issue, EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION, transactionReferenceNumber);

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {
                        string transactionCode, transactionMessage;

                        var operationReference =
                            issue(
                                  dbContext,
                                  (int)fuelReport.VesselInCompany.CompanyId,
                                  (int)fuelReport.VesselInCompany.VesselInInventory.Id,
                                  1,
                                  convertFuelReportConsumptionTypeToStoreType(fuelReport),
                                    null,
                                  EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION,
                                  transactionReferenceNumber,
                                  userId,
                                  out transactionCode,
                                  out transactionMessage);

                        string transactionItemMessage;

                        var transactionItems = new List<TransactionItem>();

                        foreach (var fuelReportDetail in fuelReport.FuelReportDetails)
                        {
                            transactionItems.Add(new TransactionItem()
                                                 {
                                                     GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                                                     CreateDate = DateTime.Now,
                                                     Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                                                     QuantityAmount = goodsConsumption[fuelReportDetail.GoodId],
                                                     QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                                                     TransactionId = (int)operationReference.OperationId,
                                                     UserCreatorId = userId
                                                 });
                        }

                        var registeredTransactionIds = addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                        string issuedItemsPricingMessage;

                        var pricingTransactionIds = registeredTransactionIds.Select(id => new TransactionItemPricingId() { Id = id, Description = "Voyage Consumption FIFO Pricing" });

                        try
                        {
                            priceIssuedItemsInFIFO(dbContext, pricingTransactionIds, userId, out issuedItemsPricingMessage, EOV_EOM_EOY_FUEL_REPORT_CONSUMPTION_PRICING, transactionReferenceNumber);
                        }
                        catch
                        {
                        }

                        result = new InventoryOperation(
                                       inventoryOperationId: operationReference.OperationId,
                                       actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, operationReference.OperationId),
                                       actionDate: DateTime.Now,
                                       actionType: InventoryActionType.Issue,
                                       fuelReportDetailId: null,
                                       charterId: null);
                    }
                    else
                    {
                        throw new InvalidOperation("EndOfVoyage/Month/Year inventory edit", "EndOfVoyage/Month/Year inventory edit is invalid");
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);
                    }

                    #endregion

                    transaction.Commit();

                    return result;
                }
            }
        }

        public List<InventoryOperation> ManageFuelReportDetail(FuelReportDetail fuelReportDetail, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var result = new List<Domain.Model.DomainObjects.InventoryOperation>();

                    if (fuelReportDetail.Receive.HasValue)
                    {
                        //TODO: Receive OK Except for Trust
                        #region Receive
                        var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIVE, fuelReportDetail.Id.ToString());

                        //if (reference.OperationId == INVALID_ID)
                        if (reference == null)
                        {
                            string transactionCode, transactionMessage;

                            var operationReference =
                                receipt(
                                    dbContext,
                                    (int)fuelReportDetail.FuelReport.VesselInCompany.CompanyId,
                                    (int)fuelReportDetail.FuelReport.VesselInCompany.VesselInInventory.Id,
                                    1,
                                    convertFuelReportReceiveTypeToStoreType(fuelReportDetail),
                                    null,
                                    FUEL_REPORT_DETAIL_RECEIVE,
                                    fuelReportDetail.Id.ToString(),
                                    userId,
                                    out transactionCode,
                                    out transactionMessage);

                            string transactionItemMessage;

                            var transactionItems = new List<TransactionItem>();
                            transactionItems.Add(new TransactionItem()
                            {
                                GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                                CreateDate = DateTime.Now,
                                Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                                QuantityAmount = (decimal?)fuelReportDetail.Receive.Value,
                                QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                                TransactionId = (int)operationReference.OperationId,
                                UserCreatorId = userId
                            });

                            addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                            result.Add(new InventoryOperation(
                                        inventoryOperationId: operationReference.OperationId,
                                        actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                                        actionDate: DateTime.Now,
                                        actionType: InventoryActionType.Receipt,
                                        fuelReportDetailId: fuelReportDetail.Id,
                                        charterId: null));
                        }
                        else
                        {
                            throw new InvalidOperation("FR Receive Receipt Edit", "FueReport Receive edit is invalid");

                            var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                        }
                        #endregion
                    }

                    if (fuelReportDetail.Transfer.HasValue)
                    {
                        //TODO: Transfer
                        #region Transfer

                        var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Issue, FUEL_REPORT_DETAIL_TRANSFER, fuelReportDetail.Id.ToString());

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
                                    convertFuelReportTransferTypeToStoreType(fuelReportDetail),
                                    null,
                                    FUEL_REPORT_DETAIL_TRANSFER,
                                    fuelReportDetail.Id.ToString(),
                                    userId,
                                    out transactionCode,
                                    out transactionMessage);

                            string transactionItemMessage;

                            var transactionItems = new List<TransactionItem>();
                            transactionItems.Add(new TransactionItem()
                            {
                                GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                                CreateDate = DateTime.Now,
                                Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                                QuantityAmount = (decimal?)fuelReportDetail.Transfer.Value,
                                QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                                TransactionId = (int)operationReference.OperationId,
                                UserCreatorId = userId
                            });

                            addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                            result.Add(new InventoryOperation(
                                        inventoryOperationId: operationReference.OperationId,
                                        actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                                        actionDate: DateTime.Now,
                                        actionType: InventoryActionType.Issue,
                                        fuelReportDetailId: fuelReportDetail.Id,
                                        charterId: null));
                        }
                        else
                        {
                            throw new InvalidOperation("FR Transfer Issue Edit", "FueReport Transfer edit is invalid");

                            var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                        }
                        #endregion
                    }

                    transaction.Commit();

                    return result;
                }
            }
            return null;
        }

        //private decimal? calculateCorrectionAmount(FuelReportDetail detail)
        //{
        //    if (detail.Correction.HasValue && detail.CorrectionType.HasValue)
        //    {
        //        return (decimal?)((detail.CorrectionType.Value == CorrectionTypes.Minus ? -1 : 1) * detail.Correction.Value);
        //    }

        //    return null;
        //}

        /*
            1,  1, N'Charter In Start'
            2,  1, N'FuelReport Receive Trust'
            3,  1, N'FuelReport Receive InternalTransfer'
            4,  1, N'FuelReport Receive TransferPurchase'
            5,  1, N'FuelReport Receive Purchase'
            6 , 1, N'FuelReport Receive From Tank'
            7 , 1, N'FuelReport Incremental Correction', 
            8 , 1, N'Charter Out End'
                   
            9 , 2, N'Charter Out Start'
            10, 2, N'FuelReport Transfer InternalTransfer'
            11, 2, N'FuelReport Transfer TransferSale'
            12, 2, N'FuelReport Transfer Rejected'
            13, 2, N'FuelReport Decremental Correction'
            14, 2, N'EOV Consumption'
            15, 2, N'EOM Consumption'
            16, 2, N'EOY Consumption'
            17, 2, N'Charter In End'
         
        */

        private int convertFuelReportReceiveTypeToStoreType(FuelReportDetail detail)
        {
            switch (detail.ReceiveType)
            {
                case ReceiveTypes.Trust:
                    return 2;
                case ReceiveTypes.InternalTransfer:
                    return 3;
                case ReceiveTypes.TransferPurchase:
                    return 4;
                case ReceiveTypes.Purchase:
                    return 5;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private int convertFuelReportTransferTypeToStoreType(FuelReportDetail detail)
        {
            switch (detail.TransferType)
            {
                case TransferTypes.InternalTransfer:
                    return 10;
                case TransferTypes.TransferSale:
                    return 11;
                case TransferTypes.Rejected:
                    return 12;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private int convertFuelReportCorrectionTypeToStoreType(FuelReportDetail detail)
        {
            switch (detail.CorrectionType)
            {
                case CorrectionTypes.Plus:
                    return 7;
                case CorrectionTypes.Minus:
                    return 13;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private int convertFuelReportConsumptionTypeToStoreType(FuelReport fuelReport)
        {
            switch (fuelReport.FuelReportType)
            {
                case FuelReportTypes.EndOfVoyage:
                    return 14;
                case FuelReportTypes.EndOfYear:
                    return 15;
                case FuelReportTypes.EndOfMonth:
                    return 16;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //================================================================================

        public List<InventoryOperation> ManageFuelReportDetailDecrementalCorrection(FuelReportDetail fuelReportDetail, int userId)
        {
            //if (fuelReportDetail.Correction.HasValue && 
            //    fuelReportDetail.CorrectionType.HasValue &&
            //    fuelReportDetail.CorrectionPrice.HasValue &&
            //    !string.IsNullOrWhiteSpace(fuelReportDetail.CorrectionPriceCurrencyISOCode))

            //if (fuelReportDetail.CorrectionType.Value == CorrectionTypes.Minus)
            //{
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var result = new List<Domain.Model.DomainObjects.InventoryOperation>();

                    //TODO: Decremental Correction
                    #region Decremental Correction

                    var transactionReferenceNumber = fuelReportDetail.Id.ToString();

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Issue, FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION, transactionReferenceNumber);

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
                                  convertFuelReportCorrectionTypeToStoreType(fuelReportDetail),
                                  null,
                                  FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION,
                                  transactionReferenceNumber,
                                  userId,
                                  out transactionCode,
                                  out transactionMessage);

                        string transactionItemMessage;

                        var transactionItems = new List<TransactionItem>();
                        transactionItems.Add(new TransactionItem()
                                             {
                                                 GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                                                 CreateDate = DateTime.Now,
                                                 Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                                                 QuantityAmount = (decimal?)fuelReportDetail.Correction,
                                                 QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                                                 TransactionId = (int)operationReference.OperationId,
                                                 UserCreatorId = userId
                                             });

                        var transactionItemIds = addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                        string pricingMessage;
                        //int? notPricedTransactionId;

                        priceIssuedItemInFIFO(dbContext, transactionItemIds[0], "Decremental Correction FIFO Pricing", userId,
                                              out pricingMessage, FUEL_REPORT_DETAIL_DECREMENTAL_CORRECTION_PRICING,
                                              transactionReferenceNumber);

                        result.Add(new InventoryOperation(
                                       inventoryOperationId: operationReference.OperationId,
                                       actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                                       actionDate: DateTime.Now,
                                       actionType: InventoryActionType.Issue,
                                       fuelReportDetailId: fuelReportDetail.Id,
                                       charterId: null));
                    }
                    else
                    {
                        throw new InvalidOperation("FR Decremental Correction Edit", "FueReport  Decremental Correction edit is invalid");
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);
                    }

                    #endregion
                    
                    transaction.Commit();

                    return result;
                }
            }
        }

        //================================================================================

        public List<InventoryOperation> ManageScrap(Scrap scrap, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = new TransactionScope())
                {
                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Issue, CHARTER_IN_START_RECEIPT, scrap.Id.ToString());

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

        private int getScrapStoreType()
        {
            return 18;
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
                        var receiptReference = findInventoryOperationReference(
                            dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIVE,
                            orderItemBalance.FuelReportDetailId.ToString());


                        var receiptReferenceTransactionItem = dbContext.TransactionItems.Where(
                            tip =>
                                tip.TransactionId == (int)receiptReference.OperationId &&
                                tip.GoodId == (int)orderItemBalance.FuelReportDetail.Good.SharedGoodId);



                        var receiptRegisteredPrices = dbContext.TransactionItemPrices.Where(
                            tip =>
                                tip.TransactionItem.TransactionId == (int)receiptReference.OperationId &&
                                tip.TransactionItem.GoodId == (int)orderItemBalance.FuelReportDetail.Good.SharedGoodId);



                        var receiptPriceReferenceNumber = generateOrderItemBalancePricingReferenceNumber(orderItemBalance);

                        var receiptRegisteredPriceReference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIPT_INVOICE, receiptPriceReferenceNumber);






                        receiptRegisteredPrices.FirstOrDefault(p => p.Id == receiptRegisteredPriceReference.OperationId);

                        var referenceNumber = orderItemBalance.FuelReportDetailId;
                    }

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Issue, FUEL_REPORT_DETAIL_RECEIPT_INVOICE, invoice.Id.ToString());

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

        public InventoryOperation ManageOrderItemBalance(OrderItemBalance orderItemBalance, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    //Finding relevant Receipt Item
                    var receiptReferenceNumber = orderItemBalance.FuelReportDetailId.ToString();

                    var receiptReference = findInventoryOperationReference(
                        dbContext, InventoryOperationType.Receipt,
                        FUEL_REPORT_DETAIL_RECEIVE, receiptReferenceNumber);


                    var receiptReferenceTransactionItem = dbContext.TransactionItems.Single(
                        tip =>
                            tip.TransactionId == (int)receiptReference.OperationId &&
                            tip.GoodId == (int)orderItemBalance.FuelReportDetail.Good.SharedGoodId);

                    var receiptPriceReferenceNumber = generateOrderItemBalancePricingReferenceNumber(orderItemBalance);

                    var receiptRegisteredPriceReference = findInventoryOperationReference(dbContext, InventoryOperationType.Pricing, FUEL_REPORT_DETAIL_RECEIPT_INVOICE, receiptPriceReferenceNumber);
                    //There is no mechanism for editing the price of priced receipts portions.


                    var transactionItemPrice = new TransactionItemPrice()
                    {
                        TransactionItemId = receiptReferenceTransactionItem.Id,
                        QuantityUnitId = getMeasurementUnitId(dbContext, orderItemBalance.UnitCode),
                        QuantityAmount = orderItemBalance.QuantityAmountInMainUnit,
                        PriceUnitId = getCurrencyId(dbContext, orderItemBalance.InvoiceItem.Invoice.Currency.Abbreviation),
                        Fee = orderItemBalance.InvoiceItem.Fee,
                        RegistrationDate = DateTime.Now,
                        Description = "Received Good Pricing > " + orderItemBalance.FuelReportDetail.Good.Code,
                        UserCreatorId = userId
                    };

                    string pricingMessage;

                    priceTransactionItemManually(dbContext, transactionItemPrice, userId, out pricingMessage, FUEL_REPORT_DETAIL_RECEIPT_INVOICE, receiptPriceReferenceNumber);

                    var pricingOperationReference = findInventoryOperationReference(dbContext, InventoryOperationType.Pricing, FUEL_REPORT_DETAIL_RECEIPT_INVOICE, receiptPriceReferenceNumber);

                    var result = new InventoryOperation(
                        pricingOperationReference.OperationId,
                           actionNumber: string.Format("{0}/{1}", (InventoryOperationType)pricingOperationReference.OperationType, pricingOperationReference.OperationId),
                           actionDate: DateTime.Now,
                           actionType: InventoryActionType.Pricing,
                           fuelReportDetailId: null,
                           charterId: null);

                    transaction.Commit();

                    return result;
                }
            }
        }

        //================================================================================

        private string generateOrderItemBalancePricingReferenceNumber(Domain.Model.DomainObjects.OrderAggreate.OrderItemBalance orderItemBalance)
        {
            return string.Format("{0},{1}", orderItemBalance.FuelReportDetailId, orderItemBalance.InvoiceItemId);
        }

        //================================================================================

        private int convertCharterInTypeToStoreType(CharterIn charterIn)
        {
            switch (charterIn.CharterType)
            {
                case CharterType.Start:
                    return 1;

                case CharterType.End:
                    return 17;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int convertCharterOutTypeToStoreType(CharterOut charterOut)
        {
            switch (charterOut.CharterType)
            {
                case CharterType.Start:
                    return 9;

                case CharterType.End:
                    return 8;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public List<InventoryOperation> ManageCharterInStart(CharterIn charterInStart, int userId)
        {
            if (charterInStart.CharterType != CharterType.Start)
                throw new InvalidArgument("The given entity is not Charter In Start", "charterInStart");

            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var result = new List<Domain.Model.DomainObjects.InventoryOperation>();

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterInStart.Id.ToString());


                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {
                        activateWarehouse(dbContext, (int)charterInStart.VesselInCompany.VesselInInventory.Id, userId);

                        string transactionCode, transactionMessage;


                        var operationReference = receipt(
                                  dbContext,
                                  (int)charterInStart.VesselInCompany.CompanyId,
                                  (int)charterInStart.VesselInCompany.VesselInInventory.Id,
                                  1,
                                  convertCharterInTypeToStoreType(charterInStart),
                                    null,
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
                                                     Description = "Charter In Start > " + charterItem.Good.Code,
                                                     QuantityAmount = charterItem.Rob,
                                                     QuantityUnitId = getMeasurementUnitId(dbContext, charterItem.GoodUnit.Abbreviation),
                                                     TransactionId = (int)operationReference.OperationId,
                                                     UserCreatorId = userId
                                                 });
                        }

                        addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);


                        //Manual Items Pricing
                        var registeredTransaction = dbContext.Transactions.Single(t => t.Id == (int)operationReference.OperationId);


                        var transactionItemPrices = new List<TransactionItemPrice>();

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
                                    Description = "Charter In Start Pricing > " + charterItem.Good.Code,
                                    UserCreatorId = userId
                                };

                            transactionItemPrices.Add(transactionItemPrice);
                            //priceTransactionItemsManually(dbContext, transactionItemPrice, userId, out pricingMessage, CHARTER_IN_START_RECEIPT_PRICING, charterItem.Id.ToString());
                        }

                        string pricingMessage;
                        priceTransactionItemsManually(dbContext, transactionItemPrices, userId, out pricingMessage, CHARTER_IN_START_RECEIPT_PRICING, charterInStart.Id.ToString());

                        result.Add(new InventoryOperation(
                            inventoryOperationId: operationReference.OperationId,
                            actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                            actionDate: DateTime.Now,
                            actionType: InventoryActionType.Receipt,
                            fuelReportDetailId: null,
                            charterId: charterInStart.Id));
                    }
                    else
                    {
                        throw new InvalidOperation("CharterInStart disapprovement", "CharterInStart disapprovement is invalid.");

                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }

                    transaction.Commit();

                    return result;
                }
            }
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
                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterInEnd.Id.ToString());

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

        public InventoryOperation ManageCharterOutStart(CharterOut charterOutStart, int userId)
        {
            if (charterOutStart.CharterType != CharterType.Start)
                throw new InvalidArgument("The given entity is not Charter Out Start", "charterOutStart");

            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    //try
                    //{
                    //    dbContext.Database.ExecuteSqlCommand(
                    //                                         TransactionalBehavior.EnsureTransaction,
                    //                                         "dbo.[IssueItemPricesOperation]");
                    //}
                    //catch (Exception)
                    //{
                    //    throw;
                    //}


                    InventoryOperation result = null;

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Issue, CHARTER_OUT_START_ISSUE, charterOutStart.Id.ToString());

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {
                        string transactionCode, transactionMessage;

                        var operationReference = issue(
                                  dbContext,
                                  (int)charterOutStart.VesselInCompany.CompanyId,
                                  (int)charterOutStart.VesselInCompany.VesselInInventory.Id,
                                  1,
                                  convertCharterOutTypeToStoreType(charterOutStart),
                                    null,
                                  CHARTER_OUT_START_ISSUE,
                                  charterOutStart.Id.ToString(),
                                  userId,
                                  out transactionCode,
                                  out transactionMessage);

                        string transactionItemMessage;

                        var transactionItems = new List<TransactionItem>();

                        foreach (var charterItem in charterOutStart.CharterItems)
                        {

                            transactionItems.Add(new TransactionItem()
                            {
                                GoodId = (int)charterItem.Good.SharedGoodId,
                                CreateDate = DateTime.Now,
                                Description = "Charter Out Start > " + charterItem.Good.Code,
                                QuantityAmount = charterItem.Rob,
                                QuantityUnitId = getMeasurementUnitId(dbContext, charterItem.GoodUnit.Abbreviation),
                                TransactionId = (int)operationReference.OperationId,
                                UserCreatorId = userId
                            });
                        }

                        var registeredTransactionIds = addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                        string issuedItemsPricingMessage;

                        var pricingTransactionIds = registeredTransactionIds.Select(id => new TransactionItemPricingId() { Id = id, Description = "Charter-Out Start FIFO Pricing" });

                        priceIssuedItemsInFIFO(dbContext, pricingTransactionIds, userId, out issuedItemsPricingMessage, CHARTER_OUT_START_ISSUE_PRICING, charterOutStart.Id.ToString());

                        deactivateWarehouse(dbContext, (int)charterOutStart.VesselInCompany.VesselInInventory.Id, userId);

                        result = new InventoryOperation(
                                       inventoryOperationId: operationReference.OperationId,
                                       actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, operationReference.OperationId),
                                       actionDate: DateTime.Now,
                                       actionType: InventoryActionType.Issue,
                                       fuelReportDetailId: null,
                                       charterId: null);
                    }
                    else
                    {
                        throw new InvalidOperation("CharterOutStart disapprovement", "CharterOutStart disapprovement is invalid.");

                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);


                    }

                    transaction.Commit();

                    return result;
                }
            }
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
                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, CHARTER_IN_START_RECEIPT, charterOutEnd.Id.ToString());

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

        public Transaction GetTransaction(long transactionId, InventoryOperationType operationType)
        {
            TransactionActionType transactionType = mapOperationTypeToTransactionActionType(operationType);

            using (var dbContext = new InventoryDbContext())
            {
                var foundTransaction = dbContext.Transactions.First(t => t.Id == transactionId && t.Action == (byte)transactionType);

                if (foundTransaction == null)
                    throw new ObjectNotFound("InventoryTransaction", transactionId);

                return foundTransaction;
            }
        }

        public OperationReference GetFueReportDetailReceiveOperationReference(FuelReportDetail fuelReportDetail)
        {
            var transactionReferenceNumber = fuelReportDetail.Id.ToString();

            using (var dbContext = new InventoryDbContext())
            {
                var operationReference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_RECEIVE, transactionReferenceNumber);

                return operationReference;
            }
        }

        private TransactionActionType mapOperationTypeToTransactionActionType(InventoryOperationType operationType)
        {
            switch (operationType)
            {
                case InventoryOperationType.Receipt:
                    return TransactionActionType.Receipt;
                case InventoryOperationType.Issue:
                    return TransactionActionType.Issue;
                default:
                    throw new ArgumentOutOfRangeException("operationType");
            }
        }

        //================================================================================

        public decimal GetAveragePrice(long transactionId, TransactionActionType actionType, long goodId, long unitId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                var transactionItems = dbContext.TransactionItems.Where(
                    ti =>
                        ti.Transaction.Action == (byte)actionType &&
                        ti.TransactionId == transactionId && ti.GoodId == goodId &&
                        ti.Transaction.Status == (byte)TransactionStatus.FullPriced);

                if (transactionItems == null || transactionItems.Count() == 0)
                    throw new ObjectNotFound("FullPricedTransactionItems", transactionId);

                var transactionItemPrices = transactionItems.SelectMany(ti => ti.TransactionItemPrices);

                var totalTransactionQuantity = transactionItemPrices.Sum(tip => tip.QuantityAmount.Value);

                var totalTransactionPrice = transactionItemPrices.Sum(tip => tip.QuantityAmount.Value * tip.FeeInMainCurrency.Value);

                return totalTransactionPrice / totalTransactionQuantity;
            }
        }

        //================================================================================

        public List<InventoryOperation> ManageFuelReportDetailIncrementalCorrectionUsingReferencePricing(FuelReportDetail fuelReportDetail, long pricingReferenceId, string pricingReferenceType, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var result = new List<Domain.Model.DomainObjects.InventoryOperation>();

                    #region Incremental Correction

                    var transactionReferenceNumber = fuelReportDetail.Id.ToString();

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION, transactionReferenceNumber);

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {
                        string transactionCode, transactionMessage;

                        var operationReference =
                            receipt(
                                  dbContext,
                                  (int)fuelReportDetail.FuelReport.VesselInCompany.CompanyId,
                                  (int)fuelReportDetail.FuelReport.VesselInCompany.VesselInInventory.Id,
                                  1,
                                  convertFuelReportCorrectionTypeToStoreType(fuelReportDetail),
                                    (int)pricingReferenceId,
                                  FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION,
                                  transactionReferenceNumber,
                                  userId,
                                  out transactionCode,
                                  out transactionMessage);

                        string transactionItemMessage;

                        var transactionItems = new List<TransactionItem>();
                        transactionItems.Add(new TransactionItem()
                        {
                            GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                            CreateDate = DateTime.Now,
                            Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                            QuantityAmount = (decimal?)fuelReportDetail.Correction,
                            QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                            TransactionId = (int)operationReference.OperationId,
                            UserCreatorId = userId
                        });

                        var transactionItemIds = addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                        string pricingMessage;

                        priceSuspendedTransactionUsingReference(dbContext, (int)operationReference.OperationId, pricingReferenceType, userId, out pricingMessage, FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION_PRICING, transactionReferenceNumber);

                        result.Add(new InventoryOperation(
                                       inventoryOperationId: operationReference.OperationId,
                                       actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                                       actionDate: DateTime.Now,
                                       actionType: InventoryActionType.Receipt,
                                       fuelReportDetailId: fuelReportDetail.Id,
                                       charterId: null));

                    }
                    else
                    {
                        throw new InvalidOperation("FR Incremental Correction Edit", "FueReport  Incremental Correction edit is invalid");
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);
                    }

                    #endregion

                    transaction.Commit();

                    return result;
                }
            }
        }

        //================================================================================

        public List<InventoryOperation> ManageFuelReportDetailIncrementalCorrectionDirectPricing(FuelReportDetail fuelReportDetail, int userId)
        {
            using (var dbContext = new InventoryDbContext())
            {
                using (var transaction = dbContext.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    var result = new List<Domain.Model.DomainObjects.InventoryOperation>();

                    #region Incremental Correction

                    var transactionReferenceNumber = fuelReportDetail.Id.ToString();

                    var reference = findInventoryOperationReference(dbContext, InventoryOperationType.Receipt, FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION, transactionReferenceNumber);

                    //if (reference.OperationId == INVALID_ID)
                    if (reference == null)
                    {
                        string transactionCode, transactionMessage;

                        var operationReference =
                            receipt(
                                  dbContext,
                                  (int)fuelReportDetail.FuelReport.VesselInCompany.CompanyId,
                                  (int)fuelReportDetail.FuelReport.VesselInCompany.VesselInInventory.Id,
                                  1,
                                  convertFuelReportCorrectionTypeToStoreType(fuelReportDetail),
                                    null,
                                  FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION,
                                  transactionReferenceNumber,
                                  userId,
                                  out transactionCode,
                                  out transactionMessage);

                        string transactionItemMessage;

                        var transactionItems = new List<TransactionItem>();
                        transactionItems.Add(new TransactionItem()
                                             {
                                                 GoodId = (int)fuelReportDetail.Good.SharedGoodId,
                                                 CreateDate = DateTime.Now,
                                                 Description = fuelReportDetail.FuelReport.FuelReportType.ToString(),
                                                 QuantityAmount = (decimal?)fuelReportDetail.Correction,
                                                 QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                                                 TransactionId = (int)operationReference.OperationId,
                                                 UserCreatorId = userId
                                             });

                        var transactionItemIds = addTransactionItems(dbContext, (int)operationReference.OperationId, transactionItems, userId, out transactionItemMessage);

                        //TODO: Items Pricing.

                        var transactionItemPrice = new TransactionItemPrice()
                        {
                            TransactionItemId = transactionItemIds[0],
                            QuantityUnitId = getMeasurementUnitId(dbContext, fuelReportDetail.MeasuringUnit.Abbreviation),
                            QuantityAmount = (decimal?)fuelReportDetail.Correction,
                            PriceUnitId = getCurrencyId(dbContext, fuelReportDetail.CorrectionPriceCurrency.Abbreviation),
                            Fee = fuelReportDetail.CorrectionPrice,
                            RegistrationDate = DateTime.Now,
                            Description = "Incremental Correction Direct Pricing > " + fuelReportDetail.Good.Code,
                            UserCreatorId = userId
                        };

                        string pricingMessage;

                        priceTransactionItemManually(dbContext, transactionItemPrice, userId, out pricingMessage, FUEL_REPORT_DETAIL_INCREMENTAL_CORRECTION_PRICING, transactionReferenceNumber);

                        result.Add(new InventoryOperation(
                                       inventoryOperationId: operationReference.OperationId,
                                       actionNumber: string.Format("{0}/{1}", (InventoryOperationType)operationReference.OperationType, transactionCode),
                                       actionDate: DateTime.Now,
                                       actionType: InventoryActionType.Receipt,
                                       fuelReportDetailId: fuelReportDetail.Id,
                                       charterId: null));
                    }
                    else
                    {
                        throw new InvalidOperation("FR Incremental Correction Edit", "FueReport  Incremental Correction edit is invalid");
                        var transactionItems = dbContext.TransactionItems.Where(ti => ti.TransactionId == reference.OperationId);
                    }

                    #endregion

                    transaction.Commit();

                    return result;
                }
            }
        }

        //================================================================================

        private class TransactionItemPricingId
        {
            public int Id;

            public string Description;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Integration.Inventory.Data.ReversePOCO;

namespace MITD.Fuel.Integration.Inventory
{
    public interface IInventoryOperationManager
    {
        InventoryOperation ManageFuelReportConsumption(FuelReport fuelReport, int userId);

        List<InventoryOperation> ManageFuelReportDetail(FuelReportDetail fuelReportDetail, int userId);

        List<InventoryOperation> ManageFuelReportDetailIncrementalCorrectionUsingReferencePricing(FuelReportDetail fuelReportDetail, long pricingReferenceId, string pricingReferenceType, int userId);

        List<InventoryOperation> ManageFuelReportDetailIncrementalCorrectionDirectPricing(FuelReportDetail fuelReportDetail, int userId);

        List<InventoryOperation> ManageScrap(Scrap scrap, int userId);

        //List<InventoryOperation> ManageInvoice(Invoice invoice, int userId);

        InventoryOperation ManageOrderItemBalance(OrderItemBalance orderItemBalance, int userId);

        List<InventoryOperation> ManageCharterInStart(CharterIn charterInStart, int userId);

        List<InventoryOperation> ManageCharterInEnd(CharterIn charterInEnd, int userId);

        InventoryOperation ManageCharterOutStart(CharterOut charterOutStart, int userId);

        List<InventoryOperation> ManageCharterOutEnd(CharterOut charterOutEnd, int userId);

        Transaction GetTransaction(long transactionId, InventoryOperationType operationType);
        
        OperationReference GetFueReportDetailReceiveOperationReference(FuelReportDetail fuelReportDetail);

        decimal GetAveragePrice(long transactionId, MITD.Fuel.Integration.Inventory.InventoryOperationManager.TransactionActionType actionType, long goodId, long unitId);
    }
}

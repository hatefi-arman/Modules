using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;

namespace MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations
{
    public interface IInventoryOperationNotifier : IEventNotifier
    {
        InventoryOperation NotifySubmittingFuelReportConsumption(FuelReport fuelReport);

        List<InventoryOperation> NotifySubmittingFuelReportDetail(FuelReportDetail source, IFuelReportDomainService fuelReportDomainService);

        List<InventoryOperation> NotifySubmittingScrap(Scrap source);

        List<InventoryOperation> NotifySubmittingInvoice(Invoice source);
        
        InventoryOperation NotifySubmittingOrderItemBalance(OrderItemBalance orderItemBalance);

        List<InventoryOperation> NotifySubmittingCharterInStart(CharterIn charterInStart);
        
        List<InventoryOperation> NotifySubmittingCharterInEnd(CharterIn charterInEnd);
        
        List<InventoryOperation> NotifySubmittingCharterOutStart(CharterOut charterOutStart);
        
        List<InventoryOperation> NotifySubmittingCharterOutEnd(CharterOut charterOutEnd);
    }
}
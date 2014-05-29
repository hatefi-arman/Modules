using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations
{
    public interface IInventoryOperationNotifier : IEventNotifier
    {
        List<InventoryOperation> NotifySubmittingFuelReportDetail(FuelReportDetail source);

        List<InventoryOperation> NotifySubmittingScrap(Scrap source);

        List<InventoryOperation> NotifySubmittingInvoice(Invoice source);

        List<InventoryOperation> NotifySubmittingCharterInStart(CharterIn charterInStart);
        
        List<InventoryOperation> NotifySubmittingCharterInEnd(CharterIn charterInEnd);
        
        List<InventoryOperation> NotifySubmittingCharterOutStart(CharterOut charterOutStart);
        
        List<InventoryOperation> NotifySubmittingCharterOutEnd(CharterOut charterOutEnd);
    }
}
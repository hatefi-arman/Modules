using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;

namespace MITD.Fuel.Integration.Inventory
{
    public interface IInventoryOperationManagement
    {
        List<InventoryOperation> ManageFuelReportDetail(FuelReportDetail fuelReportDetail, int userId);

        List<InventoryOperation> ManageScrap(Scrap scrap, int userId);

        List<InventoryOperation> ManageInvoice(MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.Invoice invoice, int userId);

        List<InventoryOperation> ManageCharterInStart(CharterIn charterInStart, int userId);

        List<InventoryOperation> ManageCharterInEnd(CharterIn charterInEnd, int userId);

        List<InventoryOperation> ManageCharterOutStart(CharterOut charterOutStart, int userId);

        List<InventoryOperation> ManageCharterOutEnd(CharterOut charterOutEnd, int userId);
    }
}

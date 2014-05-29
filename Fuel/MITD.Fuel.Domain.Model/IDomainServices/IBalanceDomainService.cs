using System.Collections.Generic;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface IBalanceDomainService : IDomainService
    {
        void DeleteInvoiceItemRefrencesFromBalance(long id);

        IEnumerable<InvoiceItem> GenerateInvoiceItemFromOrders(List<long> orderList);
        void CreateBalanceRecordForInvoiceItem(InvoiceItem invoiceItem, List<Order> orderRefrences);
        void SetReceivedData(long orderId, long fuelReportDetailId, long goodId, long unitId, decimal receivedQuantity);

    }
}
#region

using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.DomainService
{

    public class InvoiceDomainService : IInvoiceDomainService
    {
        private readonly IInvoiceRepository invoiceRepository;

        public InvoiceDomainService(IInvoiceRepository invoiceRepository)
        {
            this.invoiceRepository = invoiceRepository;
        }

        public bool IsInvoiceNumberUniqueForCompnay(long Id, string invoiceNumber, long? supplier, long? transporter)
        {
            var uniq = invoiceRepository.Count
                (
                    c => (Id == 0 || c.Id != Id) &&
                        ((supplier == null || c.SupplierId == supplier) ||
                        (transporter == null || c.TransporterId == transporter))
                            && c.InvoiceNumber.ToLower() == invoiceNumber.ToLower()) == 0;
            return uniq;
        }
    }
}
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;

namespace MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.InvoiceStates
{
    public class OpenState : InvoiceState
    {
        private readonly IInvoiceItemDomainService invoiceItemDomainService;
        private readonly IBalanceDomainService balanceDomainService;
        private readonly IInventoryOperationNotifier inventoryOperationNotifier;
        private readonly IInvoiceStateFactory invoiceStateFactory;

        public OpenState(
            IInvoiceItemDomainService invoiceItemDomainService,
            IBalanceDomainService balanceDomainService,
            IInventoryOperationNotifier inventoryOperationNotifier, 
            IInvoiceStateFactory invoiceStateFactory)
        {
            this.invoiceItemDomainService = invoiceItemDomainService;
            this.balanceDomainService = balanceDomainService;
            this.inventoryOperationNotifier = inventoryOperationNotifier;
            this.invoiceStateFactory = invoiceStateFactory;
        }

        public override void ApproveInvoice(Invoice invoice)
        {
            invoice.SetInvoiceStateType(invoiceStateFactory.CreateSubmitState());

            invoice.SubmitInvoice(
                this.invoiceItemDomainService,
                this.balanceDomainService,
                this.inventoryOperationNotifier);
        }
    }
}
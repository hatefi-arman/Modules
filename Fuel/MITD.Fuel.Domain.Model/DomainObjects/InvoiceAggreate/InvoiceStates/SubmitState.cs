using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;

namespace MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.InvoiceStates
{
    public class SubmitState : InvoiceState
    {

        private readonly IInventoryOperationDomainService inventoryOperationDomainService;
        private readonly IInvoiceStateFactory invoiceStateFactory;
        private readonly IInventoryOperationNotifier inventoryOperationNotifier;

        public SubmitState(IInventoryOperationDomainService inventoryOperationDomainService,
            IInvoiceStateFactory invoiceStateFactory, IInventoryOperationNotifier inventoryOperationNotifier)
        {
            this.inventoryOperationDomainService = inventoryOperationDomainService;
            this.invoiceStateFactory = invoiceStateFactory;
            this.inventoryOperationNotifier = inventoryOperationNotifier;
        }

        //public override void ApproveInvoice(Invoice invoice)
        //{
        //    invoice.SetInvoiceStateType(invoiceStateFactory.CreateCloseState());
        //    invoice.SubmitInvoice(inventoryOperationNotifier);
        //}

        public override void RejectInvoice(Invoice invoice)
        {
            invoice.SetInvoiceStateType(invoiceStateFactory.CreateOpenState());
            invoice.CloseBackInvoice();
        }

        public override void CancelInvoice(Invoice invoice)
        {
            invoice.SetInvoiceStateType(invoiceStateFactory.CreateCancelState());
            invoice.CancelInvoice(inventoryOperationDomainService);
        }

    }
}
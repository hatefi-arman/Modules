using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.InvoiceStates;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;

namespace MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.Factories
{
    public class InvoiceStateFactory : IInvoiceStateFactory
    {

        private readonly IInventoryOperationDomainService inventoryOperationDomainService;
        private readonly IInventoryOperationNotifier inventoryOperationNotifier;
        private readonly IInvoiceItemDomainService invoiceItemDomainService;
        private readonly IBalanceDomainService balanceDomainService;

        public InvoiceStateFactory(
            IInventoryOperationDomainService inventoryOperationDomainService, 
            IInventoryOperationNotifier inventoryOperationNotifier,
            IInvoiceItemDomainService invoiceItemDomainService,
            IBalanceDomainService balanceDomainService
            )
        {
            this.inventoryOperationDomainService = inventoryOperationDomainService;
            this.inventoryOperationNotifier = inventoryOperationNotifier;
            this.invoiceItemDomainService = invoiceItemDomainService;
            this.balanceDomainService = balanceDomainService;
        }
        public InvoiceState CreateSubmitState()
        {
            return new SubmitState(inventoryOperationDomainService, this, inventoryOperationNotifier);
        }

        public InvoiceState CreateOpenState()
        {

            return new OpenState(
                this.invoiceItemDomainService,
                this.balanceDomainService,
                this.inventoryOperationNotifier, this);
        }

        public InvoiceState CreateCloseState()
        {
            return new CloseState();
        }

        public InvoiceState CreateCancelState()
        {
            return new CancelState();
        }
    }
}
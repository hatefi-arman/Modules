#region

using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Fuel.Domain.Model.DomainObjects.ApproveFlow;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.DomainService;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.Enums;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.InvoiceStates;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.InvoiceType;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.Specifications;
using MITD.Fuel.Domain.Model.DomainServices;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.IDomainServices.Events.InventoryOperations;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate
{
    public class Invoice
    {
        #region Properties

        private InvoiceBaseType invoiceBaseType;

        private IEntityConfigurator<Invoice> invoiceConfigurator;
        private readonly IInvoiceAdditionalPriceDomainService invoiceAdditionalPriceDomainService;

        public long Id { get; set; }


        public DateTime InvoiceDate { get; set; }

        public long CurrencyId { get; set; }

        public States State { get; set; }

        public InvoiceState InvoiceState { get; set; }

        public string Description { get; private set; }

        public DivisionMethods DivisionMethod { get; private set; }

        public string InvoiceNumber { get; set; }

        public AccountingTypes AccountingType { get; set; }
        public long? InvoiceRefrenceId { get; set; }


        public InvoiceTypes InvoiceType { get; set; }

        public long? TransporterId { get; set; }

        public long? SupplierId { get; set; }

        public byte[] TimeStamp { get; set; }

        public long OwnerId { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual Company Owner { get; set; }

        public virtual Invoice InvoiceRefrence { get; set; }

        public virtual List<Order> OrderRefrences { get; set; }

        public virtual List<InvoiceItem> InvoiceItems { get; set; }

        public virtual List<InvoiceWorkflowLog> ApproveWorkFlows { get; private set; }

        public virtual Company Supplier { get; set; }

        public virtual Company Transporter { get; set; }

        public virtual List<InvoiceAdditionalPrice> AdditionalPrices { get; set; }

        public bool IsCreditor { get; set; }

        public decimal TotalOfDivisionPrice { get; set; }

        #endregion

        #region Ctor

        public Invoice()
        {
            InvoiceItems = new List<InvoiceItem>();
            OrderRefrences = new List<Order>();
            AdditionalPrices = new List<InvoiceAdditionalPrice>();
            ApproveWorkFlows = new List<InvoiceWorkflowLog>();
        }


        public Invoice(InvoiceTypes invoiceType,
                       string invoiceNumber,
                       Company owner,
                       DateTime invoiceDate,
                       DivisionMethods divisionMethod,
                       AccountingTypes accountType,
                       Invoice invoiceRefrence,
                       List<Order> orderRefrences,
                       Currency currency,
                       bool isCreditor,
                       Company transporter,
                       Company supplier,
                       string description,
                       List<InvoiceItem> list,
                       List<InvoiceAdditionalPrice> invoiceAdditionalPriceList,
                       IEntityConfigurator<Invoice> invoiceConfigurator,
                       IInvoiceDomainService invoiceDomainService,
                       IInvoiceItemDomainService invoiceItemDomainService,
                       IGoodUnitConvertorDomainService goodUnitConvertorDomainService,
                       IInvoiceAdditionalPriceDomainService invoiceAdditionalPriceDomainService,
                       IBalanceDomainService balanceDomainService)
            : this()
        {
            // TODO: Complete member initialization
            InvoiceType = invoiceType;
            InvoiceNumber = invoiceNumber;
            Owner = owner;
            InvoiceDate = invoiceDate;
            DivisionMethod = divisionMethod;
            AccountingType = accountType;
            InvoiceRefrence = invoiceRefrence;
            OrderRefrences = orderRefrences;
            Currency = currency;
            IsCreditor = isCreditor;
            Transporter = transporter;
            Supplier = supplier;


            TransporterId = Transporter == null ? (long?)null : Transporter.Id;
            SupplierId = Supplier == null ? (long?)null : Supplier.Id;
            InvoiceRefrenceId = InvoiceRefrence == null ? (long?)null : InvoiceRefrence.Id;
            Description = description;

            UpdateInvoiceItems(list, null, balanceDomainService);
            UpdateInvoiceAdditionalPrice(invoiceAdditionalPriceList, null);

            this.invoiceConfigurator = invoiceConfigurator;
            this.invoiceAdditionalPriceDomainService = invoiceAdditionalPriceDomainService;
            invoiceConfigurator.Configure(this);
            invoiceBaseType.ValidateType(this);


            checkInvoiceNumberToBeUnique(invoiceDomainService);
            CheckInvoiceHaveInvoiceItem();
            invoiceBaseType.CheckInvoiceItemValidateQuantityAndRefrence(this, invoiceItemDomainService, goodUnitConvertorDomainService);
            invoiceAdditionalPriceDomainService.CalculateAdditionalPrice(this);

        }

        public void Update(string invoiceNumber,
                           DateTime invoiceDate,
                           DivisionMethods divisionMethod,
                           Invoice invoiceRefrence,
                           List<Order> orderRefrences,
                           Currency currency,
                           bool isCreditor,
                           Company transporter,
                           Company supplier,
                           string description,
                           List<InvoiceItem> invoiceItems,
                           List<InvoiceAdditionalPrice> invoiceAdditionalPriceList,
                           IInvoiceDomainService invoiceDomainService,
                           IInvoiceItemDomainService invoiceItemDomainService,
                           IGoodUnitConvertorDomainService goodUnitConvertorDomainService,
                           IInvoiceAdditionalPriceDomainService invoiceAdditionalPriceDomainService,
                           IBalanceDomainService balanceDomainService)
        {
            InvoiceNumber = invoiceNumber;
            InvoiceDate = invoiceDate;
            DivisionMethod = divisionMethod;
            InvoiceRefrence = invoiceRefrence;
            OrderRefrences = orderRefrences;
            Currency = currency;
            Transporter = transporter;
            Supplier = supplier;
            Description = description;
            IsCreditor = isCreditor;
            UpdateInvoiceItems(invoiceItems, invoiceItemDomainService, balanceDomainService);
            UpdateInvoiceAdditionalPrice(invoiceAdditionalPriceList, invoiceAdditionalPriceDomainService);

            TransporterId = Transporter == null ? (long?)null : Transporter.Id;
            SupplierId = Supplier == null ? (long?)null : Supplier.Id;
            InvoiceRefrenceId = InvoiceRefrence == null ? (long?)null : InvoiceRefrence.Id;

            // this.invoiceConfigurator = invoiceConfigurator;
            //                        invoiceConfigurator.Configure(this);


            checkInvoiceNumberToBeUnique(invoiceDomainService);
            CheckInvoiceHaveInvoiceItem();
            invoiceBaseType.CheckInvoiceItemValidateQuantityAndRefrence(this, invoiceItemDomainService, goodUnitConvertorDomainService);
            invoiceBaseType.ValidateType(this);
            invoiceAdditionalPriceDomainService.CalculateAdditionalPrice(this);
            ApproveWorkFlows = new List<InvoiceWorkflowLog>();

        }

        private void UpdateInvoiceItems(List<InvoiceItem> invoiceItems,
                                        IInvoiceItemDomainService invoiceItemDomainService,
                                        IBalanceDomainService balanceDomainService)
        {
            if (InvoiceType == InvoiceTypes.Attach)
            {
                if (InvoiceRefrence == null)
                    throw new BusinessRuleException("", "Reference not Set");

                //<A.H> Moved to the outside of if block.
                //foreach (var invoiceItem in InvoiceItems.ToList())
                //{
                //    invoiceItemDomainService.DeleteInvoiceItem(invoiceItem);
                //}

                //<A.H> Moved to the outside of if block.
                //InvoiceItems = invoiceItems;
            }

            for (int index = 0; index < InvoiceItems.Count; index++)
            {
                invoiceItemDomainService.DeleteInvoiceItem(InvoiceItems[index]);
            }
            InvoiceItems = invoiceItems;

            //<A.H> : The implementation has been moved to Submit method.
            //else
            //{
            //    if (OrderRefrences == null)
            //        throw new BusinessRuleException("", "Reference not Set");
            //    var c = new CalculateChangeInOrderBlance(invoiceItemDomainService, balanceDomainService);
            //    InvoiceItems = c.Process(this, invoiceItems, OrderRefrences);
            //}
        }

        private void UpdateInvoiceAdditionalPrice(List<InvoiceAdditionalPrice> additionalPrice,
                                                  IInvoiceAdditionalPriceDomainService invoiceAdditionalPriceDomainService)
        {
            if (AdditionalPrices != null)
            {
                var list = this.AdditionalPrices.ToList();
                foreach (var item in list)
                {
                    invoiceAdditionalPriceDomainService.DeleteInvoiceAdditionalPriceItem(item);
                }

            }

            if (additionalPrice.GroupBy(c => c.EffectiveFactorId).Count() != additionalPrice.Count)
                throw new BusinessRuleException("", "duplicate Additional Price Item Exception");
            AdditionalPrices = additionalPrice;
        }

        #endregion

        #region Bussiness Rules

        //BR_IN1
        public void CheckReferencesToBeValid()
        {
            invoiceBaseType.CheckRefrenceIsValid(this);
        }

        //BR_IN3
        private void checkInvoiceNumberToBeUnique(IInvoiceDomainService invoiceDomainService)
        {
            if (!invoiceDomainService.IsInvoiceNumberUniqueForCompnay(Id, InvoiceNumber, SupplierId, TransporterId))
                throw new BusinessRuleException("BRIN3", "Invoice Number Must Be Unique For Company");
        }





        //BR_IN35
        public void CheckInvoiceHaveInvoiceItem()
        {
            if (InvoiceItems.Count == 0)
                throw new BusinessRuleException("Br_35", "Invoice Must Have Items");
        }

        #endregion

        #region Methods

        #endregion

        #region InvoiceState

        public void SubmitInvoice(
                        IInvoiceItemDomainService invoiceItemDomainService,
                        IBalanceDomainService balanceDomainService,
                        IInventoryOperationNotifier inventoryOperationNotifier)
        {
            //  CheckInvoiceAnyInvoiceItem();

            if (OrderRefrences == null)
                throw new BusinessRuleException("", "Reference not Set");
            var c = new CalculateChangeInOrderBlance(invoiceItemDomainService, balanceDomainService);
            InvoiceItems = c.Process(this, this.InvoiceItems, this.OrderRefrences);

            var inventoryResul = inventoryOperationNotifier.NotifySubmittingInvoice(this);

            if (inventoryResul == null)
                throw new InvalidOperation("SubmitInvoice", "Submit the invoice to Inventory resulted to an error.");

            State = States.Submitted;
        }

        public void CloseInvoice()
        {
            State = States.Closed;
        }


        public void CancelInvoice(IInventoryOperationDomainService inventoryOperationDomainService)
        {
            //            if (inventoryOperationDomainService.HasInvoiceAnyReceipt(Id))
            //                throw new BusinessRuleException("a1");
            //            if (invoiceDomainService.InvoiceHaveAnyInvoices(Id))
            //                throw new BusinessRuleException("a2");
            State = States.Closed;
        }

        public void CloseBackInvoice()
        {
            State = States.Open;
        }

        public void SetInvoiceStateType(InvoiceState type)
        {
            InvoiceState = type;
        }

        internal void SetInvoiceType(InvoiceBaseType type)
        {
            invoiceBaseType = type;
        }

        #endregion

        //        public void DeleteAllItems(IInvoiceItemDomainService invoiceItemDomainService)
        //        {
        //            for (int index = 0; index < InvoiceItems.Count; index++)
        //            {
        //                
        //            }
        //        }

        public void CheckDeleteRules(IBalanceDomainService balanceDomainService, IInvoiceItemDomainService invoiceItemDomainService)
        {
            if (!(State == States.Open || State == States.SubmitRejected))
                throw new BusinessRuleException("BR_IN7", "Invoice Is Open");

            foreach (var invoiceItem in InvoiceItems.ToList())
            {
                balanceDomainService.DeleteInvoiceItemRefrencesFromBalance(invoiceItem.Id);
                invoiceItemDomainService.DeleteInvoiceItem(invoiceItem);
            }
        }
    }

    internal class CalculateChangeInOrderBlance
    {
        private readonly IInvoiceItemDomainService invoiceItemDomainService;
        private readonly IBalanceDomainService balanceDomainService;

        public CalculateChangeInOrderBlance(IInvoiceItemDomainService invoiceItemDomainService,
            IBalanceDomainService balanceDomainService)
        {
            this.invoiceItemDomainService = invoiceItemDomainService;
            this.balanceDomainService = balanceDomainService;
        }

        //public List<InvoiceItem> Process(Invoice invoice, List<InvoiceItem> invoiceItems, List<Order> orderRefrences)
        //{
        //    foreach (var item in invoice.InvoiceItems.ToList())
        //    {
        //        balanceDomainService.DeleteInvoiceItemRefrencesFromBalance(item.Id);
        //        invoiceItemDomainService.DeleteInvoiceItem(item);
        //    }

        //    foreach (var invoiceItem in invoiceItems)
        //    {
        //        balanceDomainService.CreateBalanceRecordForInvoiceItem(invoiceItem, orderRefrences);
        //    }
        //    return invoiceItems;
        //}

        public List<InvoiceItem> Process(Invoice invoice, List<InvoiceItem> invoiceItems, List<Order> orderRefrences)
        {
            foreach (var invoiceItem in invoiceItems)
            {
                balanceDomainService.CreateBalanceRecordForInvoiceItem(invoiceItem, orderRefrences);
            }
            return invoiceItems;
        }
    }
}
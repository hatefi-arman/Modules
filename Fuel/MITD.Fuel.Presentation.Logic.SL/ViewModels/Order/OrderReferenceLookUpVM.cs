#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Castle.Core;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.Extensions;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;

#endregion

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class OrderReferenceLookUpVM : WorkspaceViewModel
    {
        #region ctor

        public OrderReferenceLookUpVM(IOrderController controller,
                                      IFuelController mainController,
                                      OrderListVM invoiceListVM,
                                      IOrderServiceWrapper serviceWrapper)
        {
            OrderListVM = invoiceListVM;
            //  OrderListVM.IsVisibleFilter = false;

            this.controller = controller;
            this.mainController = mainController;
            this.serviceWrapper = serviceWrapper;
            DisplayName = "انتخاب  سفارش ";
            AvailableOrders = new PagedSortableCollectionView<OrderDto>() { PageSize = 10 };
            SelectedOrders = new PagedSortableCollectionView<OrderDto>();

            AvailableOrders.PageChanged += DataPageChanged;
                
        }

        private void DataPageChanged(object sender, EventArgs e)
        {
            LoadOrdersByFilters();
        }

        #endregion

        #region props

        //
        private readonly IFuelController mainController;
        private readonly IOrderServiceWrapper serviceWrapper;
        private IOrderController controller;
        private CompanyDto currentCompany;

        private DateTime fromDateFilter;
        private UserDto invoiceCreatorsFilterSelected;

        private string code;
        private DateTime toDateFilter;

        public Guid UniqId
        {
            get { return Guid.NewGuid(); }
        }

        protected DateTime UpperDate { get; set; }

        public string Code
        {
            get { return code; }
            set { this.SetField(p => p.Code, ref code, value); }
        }

        public CompanyDto CurrentCompany
        {
            get { return currentCompany; }
            set { this.SetField(d => d.CurrentCompany, ref currentCompany, value); }
        }

        public PagedSortableCollectionView<OrderDto> AvailableOrders { get; set; }

        public PagedSortableCollectionView<OrderDto> SelectedOrders { get; set; }

        public OrderListVM OrderListVM { get; set; }

        public OrderDto AddedOrder { get; set; }
        public OrderDto DeletedOrder { get; set; }

        public DateTime FromDateFilter
        {
            get { return fromDateFilter; }
            set { this.SetField(v => v.FromDateFilter, ref fromDateFilter, value); }
        }

        public DateTime ToDateFilter
        {
            get { return toDateFilter; }
            set
            {
                if (UpperDate.Ticks < value.Ticks)
                    MessageBox.Show("زمان نمی تواند از تاریخ صورتحساب بیشتر باشد");
                this.SetField(v => v.ToDateFilter, ref toDateFilter, value);
            }
        }

        #region Command

        private long? SupplierId;
        private long? TransporterId;
        private CommandViewModel addCommand;
        private CommandViewModel deleteCommand;
        private CommandViewModel returnCommand;
        private CommandViewModel searchCommand;

        public CommandViewModel ReturnCommand
        {
            get
            {
                return returnCommand ?? (returnCommand = new CommandViewModel
                    (
                    "برگشت", new DelegateCommand
                                 (
                                 () =>
                                 {
                                     mainController.Close(this);
                                     mainController.Publish
                                         (
                                             new RefrencedOrderEvent { ReferencedOrders = new ObservableCollection<OrderDto>(SelectedOrders), UniqId = UniqId,Changed=changed });
                                 })));
            }
        }

         bool changed;


        public CommandViewModel DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new CommandViewModel
                    (
                    "حذف از لیست", new DelegateCommand
                                       (
                                       () =>
                                       {
                                           if (DeletedOrder == null)
                                           {
                                               MessageBox.Show(" صورتحساب مورد نظر خود را انتخاب نمایید");
                                               return;
                                           }
                                           SelectedOrders.Remove(DeletedOrder);
                                           changed = true;
                                           if (SelectedOrders.Count == 0)
                                           {
                                               UpdatePartnerCompany(null, null);
                                               LoadOrdersByFilters();
                                           }
                                       })));
            }
        }


        public CommandViewModel AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new CommandViewModel
                    (
                    "اضافه", new DelegateCommand
                                 (
                                 () =>
                                 {
                                     if (AddedOrder == null)
                                     {
                                         MessageBox.Show(" صورتحساب مورد نظر خود را انتخاب نمایید");
                                         return;
                                     }
                                     if (SelectedOrders.Count(c => c.Id == AddedOrder.Id) > 0)
                                     {
                                         MessageBox.Show("این صورتحساب قبلا اضافه شده است");
                                         return;
                                     }
                                     if (SelectedOrders.Count == 0)
                                     {
                                         //                                                      FirstOrder = AddedOrder;
                                         UpdatePartnerCompany(AddedOrder);

                                         LoadOrdersByFilters();
                                     }
                                     changed = true;
                                     SelectedOrders.Add(AddedOrder);
                                 })));
            }
        }

        public CommandViewModel SearchCommand
        {
            get { return searchCommand ?? (searchCommand = new CommandViewModel("جستجو ... ", new DelegateCommand(() => LoadOrdersByFilters()))); }
        }

        #endregion

        #endregion

        #region methods

        private InvoiceTypeEnum invoiceType;

        private void Delete()
        {
        }


        protected override void OnRequestClose()
        {
            base.OnRequestClose();
            mainController.Close(this);
        }

        private void UpdatePartnerCompany(long? supplierId, long? transporterId)
        {
            SupplierId = supplierId;
            TransporterId = transporterId;
        }

        private void UpdatePartnerCompany(OrderDto order)
        {
            SupplierId = order.Supplier == null ? (long?)null : order.Supplier.Id;
            TransporterId = order.Transporter == null ? (long?)null : order.Transporter.Id;
        }

        public void Load(CompanyDto selectedCompany, InvoiceDto invoice)
        {
            CurrentCompany = selectedCompany;
            OrderListVM.Load();
            var uid = Guid.NewGuid();

            UpperDate = invoice.InvoiceDate;
            UpdatePartnerCompany(invoice.SupplierId, invoice.TransporterId);
            ToDateFilter = invoice.InvoiceDate;
            FromDateFilter = invoice.InvoiceDate.AddMonths(-3);

            invoiceType = invoice.InvoiceType;
            if (invoice.OrderRefrences == null)
                invoice.OrderRefrences = new ObservableCollection<OrderDto>();
            else
            {
                foreach (var order in invoice.OrderRefrences)
                    SelectedOrders.Add(order);
                LoadOrdersByFilters2();
            }
            changed = false;
        }


        private void LoadOrdersByFilters()
        {
            if (FromDateFilter == DateTime.MinValue || ToDateFilter == DateTime.MinValue)
                return;

            ShowBusyIndicator("درحال دريافت اطلاعات ............");


            var orderTypes = GetOrderTypes();


            serviceWrapper.GetByFilter
                (
                    (res, exp) => mainController.BeginInvokeOnDispatcher
                        (
                            () =>
                                {
                                    if (exp == null)
                                    {
                                        AvailableOrders.SourceCollection = res.Result.ToList();
                                        AvailableOrders.TotalItemCount = res.TotalCount;
                                    }
                                    else
                                    {
                                        mainController.HandleException(exp);
                                    }
                                    HideBusyIndicator();
                                }), currentCompany.Id, 0, FromDateFilter, ToDateFilter, AvailableOrders.PageSize, AvailableOrders.PageIndex, orderTypes, SupplierId, TransporterId, true,orderCode: Code,submitedState: true);

        }
        private void LoadOrdersByFilters2(int pageIndex = 0)
        {
            if (SelectedOrders.Count == 0)
                return;
            ShowBusyIndicator("درحال دريافت اطلاعات ............");


            var ids = "";
            SelectedOrders.Select(c => ids += c.Id + ",").ToList();
            ids = ids.Remove(ids.Length - 1);

            serviceWrapper.GetByFilter
                (
                    (res, exp) => mainController.BeginInvokeOnDispatcher
                        (
                            () =>
                                {
                                    if (exp == null)
                                    {
                                 
                                        SelectedOrders.SourceCollection = res.Result.ToList();
                                        SelectedOrders.TotalItemCount = res.TotalCount;
                                        SelectedOrders.PageIndex = Math.Max(0, res.CurrentPage - 1);
                                    }
                                    else
                                    {
                                        mainController.HandleException(exp);
                                    }
                                    HideBusyIndicator();
                                }), 0, 0, FromDateFilter, ToDateFilter, 10, pageIndex, "", null, null, true, ids, Code, true);
        }

        private string GetOrderTypes()
        {
            if (SelectedOrders != null && SelectedOrders.Count > 0)
            {

                return ((int)SelectedOrders.First().OrderType).ToString();
            }
            switch (invoiceType)
            {
                case InvoiceTypeEnum.Purchase:
                    return ((int)OrderTypeEnum.Purchase).ToString() + "," + ((int)OrderTypeEnum.PurchaseWithTransfer).ToString();
                case InvoiceTypeEnum.Transfer:
                    return ((int)OrderTypeEnum.InternalTransfer).ToString() + "," + ((int)OrderTypeEnum.PurchaseWithTransfer).ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
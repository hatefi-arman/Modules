﻿#region

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;

#endregion

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels.Invoice
{
    public class InvoiceReferenceLookUpVM : WorkspaceViewModel
    {
        #region ctor

        public InvoiceReferenceLookUpVM(IInvoiceController controller, IFuelController mainController, IInvoiceServiceWrapper serviceWrapper)
        {
            this.controller = controller;
            this.mainController = mainController;
            this.serviceWrapper = serviceWrapper;
            DisplayName = "انتخاب  صورتحساب ";
            AvailableInvoices = new PagedSortableCollectionView<InvoiceDto>();
        }

        #endregion

        #region props

        //
        private readonly IFuelController mainController;
        private readonly IInvoiceServiceWrapper serviceWrapper;
        private IInvoiceController controller;
        private CompanyDto currentCompany;
        private InvoiceDto currentInvoice;

        private DateTime fromDateFilter;
        private UserDto invoiceCreatorsFilterSelected;

        private string invoiceNumber;
        private DateTime toDateFilter;

        public Guid UniqId
        {
            get { return Guid.NewGuid(); }
        }

        public string InvoiceNumber
        {
            get { return invoiceNumber; }
            set { this.SetField(p => p.InvoiceNumber, ref invoiceNumber, value); }
        }

        public CompanyDto CurrentCompany
        {
            get { return currentCompany; }
            set { this.SetField(d => d.CurrentCompany, ref currentCompany, value); }
        }

        public PagedSortableCollectionView<InvoiceDto> AvailableInvoices { get; set; }


        public InvoiceDto AddedInvoice { get; set; }

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
                if (CurrentInvoice.InvoiceDate.Ticks < value.Ticks)
                    MessageBox.Show("زمان نمی تواند از تاریخ صورتحساب بیشتر باشد");
                this.SetField(v => v.ToDateFilter, ref toDateFilter, value);
            }
        }

        public InvoiceDto CurrentInvoice
        {
            get { return currentInvoice; }
            set { this.SetField(v => v.CurrentInvoice, ref currentInvoice, value); }
        }

        #region Command

        private CommandViewModel addCommand;
        private CommandViewModel returnCommand;
        private CommandViewModel searchCommand;

        public CommandViewModel AddCommand
        {
            get
            {
                return addCommand ?? (addCommand = new CommandViewModel
                    (
                    "انتخاب", new DelegateCommand
                                  (
                                  () =>
                                      {
                                          if (AddedInvoice == null)
                                              MessageBox.Show(" صورتحساب مورد نظر خود را انتخاب نمایید");

                                          mainController.Close(this);
                                          mainController.Publish(new RefrencedInvoiceEvent {ReferencedInvoice = AddedInvoice, UniqId = UniqId});
                                      })));
            }
        }

        public CommandViewModel ReturnCommand
        {
            get { return returnCommand ?? (returnCommand = new CommandViewModel("برگشت", new DelegateCommand(() => mainController.Close(this)))); }
        }

        public CommandViewModel SearchCommand
        {
            get { return searchCommand ?? (searchCommand = new CommandViewModel("جستجو ... ", new DelegateCommand(() => LoadInvoicesByFilters()))); }
        }

        #endregion

        #endregion

        #region methods

        private void Delete()
        {
        }


        protected override void OnRequestClose()
        {
            base.OnRequestClose();
            mainController.Close(this);
        }

        public void Load(CompanyDto selectedCompany, InvoiceDto invoice)
        {
            CurrentCompany = selectedCompany;

            CurrentInvoice = invoice;
            ToDateFilter = invoice.InvoiceDate;
            FromDateFilter = invoice.InvoiceDate.AddMonths(-3);
            var uid = Guid.NewGuid();
        }


        private void LoadInvoicesByFilters(int pageIndex = 0)
        {
            if (FromDateFilter == DateTime.MinValue || ToDateFilter == DateTime.MinValue)
                return;

            ShowBusyIndicator("درحال دريافت اطلاعات ............");
            serviceWrapper.GetByFilter
                (
                    (res, exp) => mainController.BeginInvokeOnDispatcher
                        (
                            () =>
                                {
                                    if (exp == null)
                                    {
                                        AvailableInvoices.SourceCollection = res.Result.ToList();
                                        AvailableInvoices.TotalItemCount = res.TotalCount;
                                        AvailableInvoices.PageIndex = Math.Max(0, res.CurrentPage - 1);
                                    }
                                    else
                                    {
                                        mainController.HandleException(exp);
                                    }
                                    HideBusyIndicator();
                                }), currentCompany.Id, FromDateFilter, ToDateFilter, InvoiceNumber, 20, pageIndex, true);
        }

        #endregion
    }
}
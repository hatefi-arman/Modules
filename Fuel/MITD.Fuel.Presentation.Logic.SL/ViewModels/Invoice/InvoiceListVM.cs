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
    public class InvoiceListVM : WorkspaceViewModel//, IEventHandler<InvoiceFindChangeArg>
    {
        #region props

        #region injected fields

        private IApprovalFlowServiceWrapper approcalServiceWrapper;
        private ICompanyServiceWrapper companyServiceWrapper;
        private IInvoiceController controller;
        private IFuelController mainController;
        private IInvoiceServiceWrapper serviceWrapper;
        private IUserServiceWrapper userServiceWrapper;

        #endregion

        #region filter

        private CompanyDto companiesFilterSelected;
        private DateTime fromDateFilter;
        private UserDto invoiceCreatorsFilterSelected;
        private DateTime toDateFilter;

        //filter props
        public List<CompanyDto> CompaniesFilter { get; set; }

        public CompanyDto CompaniesFilterSelected
        {
            get { return companiesFilterSelected; }
            set { this.SetField(d => d.CompaniesFilterSelected, ref companiesFilterSelected, value); }
        }

        public ObservableCollection<UserDto> InvoiceCreatorsFilter { get; set; }

        public UserDto InvoiceCreatorsFilterSelected
        {
            get { return invoiceCreatorsFilterSelected; }
            set { this.SetField(d => d.InvoiceCreatorsFilterSelected, ref invoiceCreatorsFilterSelected, value); }
        }

        // public EnumVM<InvoiceTypeEnum> InvoiceTypesVM { get; private set; }

        public DateTime FromDateFilter
        {
            get { return fromDateFilter; }
            set { this.SetField(v => v.FromDateFilter, ref fromDateFilter, value); }
        }

        public DateTime ToDateFilter
        {
            get { return toDateFilter; }
            set { this.SetField(v => v.ToDateFilter, ref toDateFilter, value); }
        }

        #endregion

        #region selected & main data

        private PagedSortableCollectionView<InvoiceDto> data;
        private InvoiceDto selectedInvoice;

        public InvoiceDto SelectedInvoice
        {
            get { return selectedInvoice; }
            set
            {
                this.SetField(p => p.SelectedInvoice, ref selectedInvoice, value);
                mainController.Publish(new InvoiceListSelectedIndexChangeEvent {Entity = value,UniqId = this.UniqId});
            }
        }

        public PagedSortableCollectionView<InvoiceDto> Data
        {
            get { return data; }
            set { this.SetField(p => p.Data, ref data, value); }
        }

        #endregion

        #region commands

        private CommandViewModel addCommand;
        private CommandViewModel approveCommand;
        private CommandViewModel canceledCommand;
        private CommandViewModel deleteCommand;
        private CommandViewModel editCommand;
        private CommandViewModel nextPageCommand;
        private CommandViewModel rejectCommand;
        private CommandViewModel searchCommand;
        //command props


        public CommandViewModel SearchCommand
        {
            get
            { return searchCommand ?? (searchCommand = new CommandViewModel("جستجو ... ", new DelegateCommand(() => LoadInvoicesByFilters()))); }
        }

        public CommandViewModel EditCommand
        {
            get
            {
                return editCommand ?? (editCommand = new CommandViewModel
                    (
                    "ویرایش", new DelegateCommand
                                  (
                                  () =>
                                      {
                                          if (!CheckIsSelected()) return;
                                          controller.Edit(SelectedInvoice,CompaniesFilter);
                                      })));
            }
        }
        
        public CommandViewModel AddCommand
        {
            get
            {
                return addCommand ?? (
                    addCommand = new CommandViewModel("افزودن",
                        new DelegateCommand(
                            () => controller.Add(CompaniesFilter))));
            }
        }

        public CommandViewModel DeleteCommand
        {
            get
            {
                return deleteCommand ?? (deleteCommand = new CommandViewModel
                    (
                    "حذف", new DelegateCommand
                               (
                               () =>
                                   {
                                       if (!CheckIsSelected()) return;


                                       if (!mainController.ShowConfirmationBox("آیا برای حذف مطمئن هستید ", "اخطار")) return;

                                       ShowBusyIndicator("درحال انجام حذف");

                                       serviceWrapper.Delete
                                           (
                                               (res, exp) => mainController.BeginInvokeOnDispatcher
                                                   (
                                                       () =>
                                                           {
                                                               if (exp != null) mainController.HandleException(exp);

                                                               HideBusyIndicator();
                                                               LoadInvoicesByFilters();
                                                               mainController.Publish(new InvoiceListSelectedIndexChangeEvent() { Entity = null, UniqId = this.UniqId });

                                                           }), SelectedInvoice.Id);
                                   })));
            }
        }


        public CommandViewModel ApproveCommand
        {
            get
            {
                return approveCommand ?? (approveCommand = new CommandViewModel
                    (
                    "تایید", new DelegateCommand
                                 (
                                 () =>
                                     {
                                         if (!CheckIsSelected()) return;
                                         ShowBusyIndicator();
                                         if (MessageBox.Show("آیا مطمئن هستید ؟", "Approve Confirm", MessageBoxButton.OKCancel)
                                             == MessageBoxResult.Cancel)
                                         {
                                             HideBusyIndicator();
                                             return;
                                         }
                                         approcalServiceWrapper.ActApproveFlow
                                             (
                                                 (res, exp) => mainController.BeginInvokeOnDispatcher
                                                     (
                                                         () =>
                                                             {
                                                                 HideBusyIndicator();
                                                                 if (exp != null)
                                                                     mainController.HandleException(exp);
                                                                 else 
                                                                     LoadInvoicesByFilters();
                                                             }), SelectedInvoice.Id, ActionEntityTypeEnum.Invoice);
                                     })));
            }
        }

        public CommandViewModel RejectCommand
        {
            get
            {
                return rejectCommand ?? (rejectCommand = new CommandViewModel
                    (
                    "رد", new DelegateCommand
                              (
                              () =>
                                  {
                                      if (!CheckIsSelected()) return;
                                      ShowBusyIndicator();
                                      if (MessageBox.Show("آیا مطمئن هستید ؟", "Reject Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                      {
                                          HideBusyIndicator();
                                          return;
                                      }
                                      approcalServiceWrapper.ActRejectFlow
                                          (
                                              (res, exp) => mainController.BeginInvokeOnDispatcher
                                                  (
                                                      () =>
                                                          {
                                                              HideBusyIndicator();
                                                              if (exp != null) mainController.HandleException(exp);
                                                              else LoadInvoicesByFilters();

                                                          }), SelectedInvoice.Id, ActionEntityTypeEnum.Invoice);
                                  })));
            }
        }


        #endregion

        #region inline editing

        #endregion

        #region column visibility

        private bool isFromVesselVisible;
        private bool _isReceiverVisible;
        private bool _isSupplierVisible;
        private bool _isToVesselVisible;
        private bool isVisibleFilter;



        public bool IsVisibleFilter
        {
            get { return isVisibleFilter; }
            set { this.SetField(p => p.IsVisibleFilter, ref isVisibleFilter, value); }
        }



        public bool IsSupplierVisible
        {
            get { return _isSupplierVisible; }
            set { this.SetField(p => p.IsSupplierVisible, ref _isSupplierVisible, value); }
        }


        public bool IsReceiverVisible
        {
            get { return _isReceiverVisible; }
            set { this.SetField(p => p.IsReceiverVisible, ref _isReceiverVisible, value); }
        }

        public bool IsFromVesselVisible
        {
            get { return isFromVesselVisible; }
            set { this.SetField(p => p.IsFromVesselVisible, ref isFromVesselVisible, value); }
        }

        public bool IsToVesselVisible
        {
            get { return _isToVesselVisible; }
            set { this.SetField(p => p.IsToVesselVisible, ref _isToVesselVisible, value); }
        }

        #endregion

        #endregion

        #region ctor

        public InvoiceListVM()
        {
            //FromDateFilter = DateTime.Now;
        }

        // vesselServiceWrapper must be added ***********************
        public InvoiceListVM(IInvoiceController controller,
                             IFuelController mainController,
                             IInvoiceServiceWrapper serviceWrapper,
                             ICompanyServiceWrapper companyServiceWrapper,
                             IUserServiceWrapper userServiceWrapper,
                          
                             // EnumVM<InvoiceTypeEnum> InvoiceTypeEnum
                              IApprovalFlowServiceWrapper approcalServiceWrapper)
        {

              this.controller = controller;
            this.serviceWrapper = serviceWrapper;
            this.mainController = mainController;
            this.companyServiceWrapper = companyServiceWrapper;
            this.userServiceWrapper = userServiceWrapper;
            this.approcalServiceWrapper = approcalServiceWrapper;

            // InvoiceTypesVM = new EnumVM<InvoiceTypeEnum>();

            DisplayName = "صورتحساب";
            Data = new PagedSortableCollectionView<InvoiceDto>();
            Data.PageChanged += Data_PageChanged;
            
            //filters
            CompaniesFilter = new List<CompanyDto>();
            InvoiceCreatorsFilter = new ObservableCollection<UserDto>();
            FromDateFilter = DateTime.Now.AddMonths(-2);
            ToDateFilter = DateTime.Now;
            Load();
            


        }

        void Data_PageChanged(object sender, EventArgs e)
        {
            LoadInvoicesByFilters();
        }

        #endregion

        #region methods

        private bool CheckIsSelected()
        {
            if (SelectedInvoice == null)
            {
                mainController.ShowMessage("لطفا سفارش مورد نظر را انتخاب فرمائید");
                return false;
            }
            return true;
        }

        protected override void OnRequestClose()
        {
            base.OnRequestClose();
            mainController.Close(this);
        }

        #region loading data

        public void Load()
        {
            ShowBusyIndicator("درحال دريافت اطلاعات ............");

            #region Companies for filter

            companyServiceWrapper.GetAll
                (
                    (res, exp) => mainController.BeginInvokeOnDispatcher
                        (
                            () =>
                                {
                                    HideBusyIndicator();
                                    if (exp == null)
                                    {
                                        CompaniesFilter.Clear();
                                        foreach (var dto in res.Result)
                                        {
                                            CompaniesFilter.Add(dto);
                                        }
                                        CompaniesFilterSelected = CompaniesFilter.FirstOrDefault();
                                    }
                                    else
                                    {
                                        mainController.HandleException(exp);
                                    }
                                }), "GetAll");

            #endregion

            #region InvoiceCreators for filter

            userServiceWrapper.GetAll
                (
                    (res, exp) => mainController.BeginInvokeOnDispatcher
                        (
                            () =>
                                {
                                    HideBusyIndicator();
                                    if (exp == null)
                                    {
                                        foreach (var dto in res) InvoiceCreatorsFilter.Add(dto);

                                        InvoiceCreatorsFilterSelected = InvoiceCreatorsFilter.FirstOrDefault();
                                    }
                                    else
                                    {
                                        mainController.HandleException(exp);
                                    }
                                }), "GetAll");

            #endregion
        }

        public Guid UniqId { get; set; }

        private void LoadInvoicesByFilters()
        {
            if (companiesFilterSelected == null || InvoiceCreatorsFilterSelected == null || FromDateFilter == DateTime.MinValue
                || ToDateFilter == DateTime.MinValue)
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
                                        Data.SourceCollection = res.Result.ToList();
                                        Data.TotalItemCount = res.TotalCount;
                                    }
                                    else
                                    {
                                        mainController.HandleException(exp);
                                    }
                                    HideBusyIndicator();
                                }), companiesFilterSelected.Id, FromDateFilter, ToDateFilter, "", Data.PageSize, Data.PageIndex, false);
        }

        #endregion

        #endregion
    }
}
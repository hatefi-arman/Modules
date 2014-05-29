using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.FuelApp.Logic.SL.ViewModels
{
    public class FuelReportListVM : WorkspaceViewModel, IEventHandler<FuelReportListChangeArg>
    {
        #region props

        private CompanyDto _companiesFilterSelected;
        private IGoodServiceWrapper _goodServiceWrapper;
        private VesselDto _vesselsFilterSelected;
        private IApprovalFlowServiceWrapper approcalServiceWrapper;
        private CommandViewModel approveCommand;
        private IFuelReportController controller;
        private PagedSortableCollectionView<FuelReportDto> data;
        private CommandViewModel deleteCommand;
        private CommandViewModel editCommand;
        private IFuelController mainController;
        private CommandViewModel rejectCommand;
        private CommandViewModel searchCommand;
        private FuelReportDto selected;
        private IFuelReportServiceWrapper serviceWrapper;
        private IResolver<FuelReportVM> FuelReportVMResolver { get; set; }
        private IFuelReportCompanyServiceWrapper CompanyServiceWrapper { get; set; }
        private IVoyageServiceWrapper VoyageServiceWrapper { get; set; }
        private IInventoryOperationServiceWrapper InventoryOperationServiceWrapper { get; set; }


        public PagedSortableCollectionView<FuelReportDto> Data
        {
            get { return this.data; }
            set { this.SetField(p => p.Data, ref this.data, value); }
        }


        public FuelReportDto Selected
        {
            get { return this.selected; }
            set
            {
                this.SetField(p => p.Selected, ref this.selected, value);

                this.FuelReportDetailListVm.FuelReportVMSelected = this.selected == null ? null : this.CreateItem(this.selected);
            }
        }

        public CommandViewModel EditCommand
        {
            get
            {
                if (this.editCommand == null)
                {
                    this.editCommand = new CommandViewModel("ویرایش", new DelegateCommand(
                                                                          () =>
                                                                          {
                                                                              if (!this.checkIsSelected())
                                                                              {
                                                                                  return;
                                                                              }
                                                                              this.controller.Edit(this.Selected);
                                                                          }
                                                                          ));
                }
                return this.editCommand;
            }
        }


        public CommandViewModel DeleteCommand
        {
            get
            {
                if (this.deleteCommand == null)
                {
                    this.deleteCommand = new CommandViewModel("حذف",
                                                              new DelegateCommand(() =>
                                                                                  {
                                                                                      if (!this.checkIsSelected())
                                                                                      {
                                                                                          return;
                                                                                      }
                                                                                      this.ShowBusyIndicator();
                                                                                      this.serviceWrapper.Delete(
                                                                                                                 (res, exp) =>
                                                                                                                     this.mainController.
                                                                                                                          BeginInvokeOnDispatcher(
                                                                                                                                                  () =>
                                                                                                                                                  {
                                                                                                                                                      this.HideBusyIndicator
                                                                                                                                                          ();
                                                                                                                                                      if (exp != null)
                                                                                                                                                      {
                                                                                                                                                          this.mainController
                                                                                                                                                              .
                                                                                                                                                               HandleException
                                                                                                                                                              (exp);
                                                                                                                                                      }
                                                                                                                                                      else
                                                                                                                                                      {
                                                                                                                                                          this.Load();
                                                                                                                                                      }
                                                                                                                                                  }), this.Selected.Id);
                                                                                  }));
                }
                return this.deleteCommand;
            }
        }

        //public CommandViewModel NextPageCommand
        //{
        //    get
        //    {
        //        if (this.nextPageCommand == null)
        //        {
        //            this.nextPageCommand = new CommandViewModel("صفحه بعد", new DelegateCommand(
        //                                                                        () =>
        //                                                                        {
        //                                                                            this.Data.PageIndex++;
        //                                                                            this.LoadFuelReportsByFilters();
        //                                                                        }
        //                                                                        ));
        //        }
        //        return this.nextPageCommand;
        //    }
        //}

        public CommandViewModel ApproveCommand
        {
            get
            {
                string caption = "تایید";
                if (this.Selected != null)
                {
                    if (this.Selected.IsNextActionFinalApprove)
                    {
                        MessageBox.Show("  کاربر گرامی می بایست مراجع تصحیح،انتقال،دریافت را برای جزئیات گزارش انتخاب نمایید. ");
                        caption = "تایید نهایی";
                    }
                }

                if (this.approveCommand == null)
                {
                    this.approveCommand = new CommandViewModel(caption,
                                                               new DelegateCommand(() =>
                                                                                   {
                                                                                       if (!this.checkIsSelected())
                                                                                       {
                                                                                           return;
                                                                                       }

                                                                                       if (MessageBox.Show("آیا مطمئن هستید ؟", "Approve Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                                                                       {
                                                                                           this.HideBusyIndicator();
                                                                                           return;
                                                                                       }

                                                                                       this.ShowBusyIndicator();

                                                                                       this.approcalServiceWrapper.ActApproveFlow(
                                                                                                                                  (res, exp) =>
                                                                                                                                      this.mainController.
                                                                                                                                           BeginInvokeOnDispatcher(
                                                                                                                                                                   () =>
                                                                                                                                                                   {
                                                                                                                                                                       this.HideBusyIndicator
                                                                                                                                                                           ();
                                                                                                                                                                       if (exp != null)
                                                                                                                                                                       {
                                                                                                                                                                           this.mainController
                                                                                                                                                                               .
                                                                                                                                                                                HandleException
                                                                                                                                                                               (exp);
                                                                                                                                                                       }
                                                                                                                                                                       else
                                                                                                                                                                       {
                                                                                                                                                                           this.LoadFuelReportsByFilters();
                                                                                                                                                                       }
                                                                                                                                                                   }),
                                                                                                                                  this.Selected.Id, ActionEntityTypeEnum.FuelReport);
                                                                                   }));
                }

                return this.approveCommand;
            }
        }

        public CommandViewModel RejectCommand
        {
            get
            {
                if (this.rejectCommand == null)
                {
                    this.rejectCommand = new CommandViewModel("رد",
                                                              new DelegateCommand(() =>
                                                                                  {
                                                                                      if (!this.checkIsSelected())
                                                                                      {
                                                                                          return;
                                                                                      }
                                                                                      this.ShowBusyIndicator();
                                                                                      if (MessageBox.Show("آیا مطمئن هستید ؟", "Reject Confirm", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                                                                                      {
                                                                                          this.HideBusyIndicator();
                                                                                          return;
                                                                                      }
                                                                                      this.approcalServiceWrapper.ActRejectFlow(
                                                                                                                                (res, exp) =>
                                                                                                                                    this.mainController.
                                                                                                                                         BeginInvokeOnDispatcher(
                                                                                                                                                                 () =>
                                                                                                                                                                 {
                                                                                                                                                                     this.HideBusyIndicator
                                                                                                                                                                         ();
                                                                                                                                                                     if (exp != null)
                                                                                                                                                                     {
                                                                                                                                                                         this.mainController
                                                                                                                                                                             .
                                                                                                                                                                              HandleException
                                                                                                                                                                             (exp);
                                                                                                                                                                     }
                                                                                                                                                                     else
                                                                                                                                                                     {
                                                                                                                                                                         this.LoadFuelReportsByFilters();
                                                                                                                                                                     }
                                                                                                                                                                 }),
                                                                                                                                this.Selected.Id, ActionEntityTypeEnum.FuelReport);
                                                                                  }));
                }
                return this.rejectCommand;
            }
        }

        public CommandViewModel SearchCommand
        {
            get
            {
                if (this.searchCommand == null)
                {
                    this.searchCommand = new CommandViewModel("جستجو ...", new DelegateCommand(
                                                                               () => { this.LoadFuelReportsByFilters(); }
                                                                               ));
                }
                return this.searchCommand;
            }
        }


        public FuelReportDetailListVM FuelReportDetailListVm { get; set; }

        public CompanyDto CompaniesFilterSelected
        {
            get { return this._companiesFilterSelected; }
            set { this.SetField(d => d.CompaniesFilterSelected, ref this._companiesFilterSelected, value); }
        }

        public ObservableCollection<CompanyDto> CompaniesFilter { get; set; }

        public VesselDto VesselsFilterSelected
        {
            get { return this._vesselsFilterSelected; }
            set { this.SetField(p => p.VesselsFilterSelected, ref this._vesselsFilterSelected, value); }
        }

        #region inline editing

        private ObservableCollection<VoyageDto> _voyages;
        //private CommandViewModel nextPageCommand;
        public EnumVM<FuelReportTypeEnum> FuelReportTypesVM { get; private set; }

        public ObservableCollection<VoyageDto> Voyages
        {
            get { return this._voyages; }
            set { this.SetField(p => p.Voyages, ref this._voyages, value); }
        }

        #endregion

        #endregion

        #region ctor

        public FuelReportListVM()
        {
            this.Data = new PagedSortableCollectionView<FuelReportDto>();
            this.Data.PageChanged += Data_PageChanged;
        }

        public FuelReportListVM(IFuelReportController controller,
                                IFuelController mainController,
                                IFuelReportServiceWrapper serviceWrapper,
                                IFuelReportCompanyServiceWrapper companyServiceWrapper,
                                IInventoryOperationServiceWrapper inventoryOperationServiceWrapper,
                                IApprovalFlowServiceWrapper approcalServiceWrapper,
                                IResolver<FuelReportVM> fuelReportVMResolver,
                                FuelReportDetailListVM fuelReportDetailListVm,
                                IGoodServiceWrapper goodServiceWrapper
            )
            : this()
        {
            this.Init(controller, mainController, serviceWrapper, companyServiceWrapper,
                      fuelReportVMResolver, inventoryOperationServiceWrapper, fuelReportDetailListVm, approcalServiceWrapper, goodServiceWrapper);
        }

        #endregion

        #region methods

        public void Handle(FuelReportListChangeArg eventData)
        {
            this.LoadFuelReportsByFilters();
        }

        void Data_PageChanged(object sender, EventArgs e)
        {
            this.LoadFuelReportsByFilters();
        }

        private void Init(IFuelReportController controller, IFuelController mainController,
                          IFuelReportServiceWrapper serviceWrapper,
                          IFuelReportCompanyServiceWrapper companyServiceWrapper,
                          IResolver<FuelReportVM> fuelReportVmResolver,
                          IInventoryOperationServiceWrapper inventoryOperationServiceWrapper,
                          FuelReportDetailListVM fuelReportDetailListVm,
                          IApprovalFlowServiceWrapper approcalServiceWrapper,
                          IGoodServiceWrapper goodServiceWrapper)
        {
            this.controller = controller;
            this.serviceWrapper = serviceWrapper;
            this.mainController = mainController;
            this._goodServiceWrapper = goodServiceWrapper;
            this.CompanyServiceWrapper = companyServiceWrapper;

            this.InventoryOperationServiceWrapper = inventoryOperationServiceWrapper;
            this.CompaniesFilter = new ObservableCollection<CompanyDto>();
            this.Voyages = new ObservableCollection<VoyageDto>();
            this.FuelReportVMResolver = fuelReportVmResolver;
            this.approcalServiceWrapper = approcalServiceWrapper;
            this.DisplayName = "گزارش سوخت";

            //inline editing 
            this.FuelReportDetailListVm = fuelReportDetailListVm;
        }

        private FuelReportVM CreateItem(FuelReportDto dto)
        {
            FuelReportVM result = this.FuelReportVMResolver.Resolve();
            result.SetMainController(this.mainController);
            result.SetServiceWrapper(this.serviceWrapper);
            result.SetEntity(dto);
            result.SetVoyages(this.Voyages);

            return result;
        }

        private bool checkIsSelected()
        {
            if (this.Selected == null)
            {
                this.mainController.ShowMessage("لطفا گزارش مورد نظر را انتخاب نمایید");
                return false;
            }
            return true;
        }

        protected override void OnRequestClose()
        {
            base.OnRequestClose();
            this.mainController.Close(this);
        }

        #region loading data

        public void Load()
        {
            this.ShowBusyIndicator("درحال دریافت اطلاعات ....");
            this.CompanyServiceWrapper.GetAll(
                                              (res, exp) =>
                                              {
                                                  this.mainController.BeginInvokeOnDispatcher(() =>
                                                                                              {
                                                                                                  this.HideBusyIndicator();
                                                                                                  if (exp == null)
                                                                                                  {
                                                                                                      this.CompaniesFilter.Clear();

                                                                                                      foreach (CompanyDto dto in res)
                                                                                                      {
                                                                                                          this.CompaniesFilter.Add(dto);
                                                                                                      }
                                                                                                      this.CompaniesFilterSelected =
                                                                                                          this.CompaniesFilter.FirstOrDefault();
                                                                                                      this.VesselsFilterSelected =
                                                                                                          this.CompaniesFilterSelected.Vessels.
                                                                                                               FirstOrDefault();
                                                                                                  }
                                                                                                  else
                                                                                                  {
                                                                                                      this.mainController.HandleException(exp);
                                                                                                  }
                                                                                              });
                                              }, this.mainController.GetCurrentUser().Id);
        }

        private void LoadFuelReportsByFilters()
        {
            if (this.CompaniesFilterSelected == null || this.VesselsFilterSelected == null)
            {
                return;
            }

            this.ShowBusyIndicator("درحال دريافت اطلاعات ............");
            this.serviceWrapper.GetByFilter(
                        (res, exp) => this.mainController.BeginInvokeOnDispatcher(() =>
                                            {
                                                this.HideBusyIndicator();
                                                if (exp == null)
                                                {
                                                    this.Data.SourceCollection = res.Result;
                                                    this.Data.TotalItemCount = res.TotalCount;
                                                }
                                                else
                                                {
                                                    this.mainController.HandleException(exp);
                                                }
                                            }),
                        this._companiesFilterSelected.Id,
                        this.VesselsFilterSelected.Id,
                        Data.PageSize, Data.PageIndex);
        }

        #endregion

        #endregion
    }
}

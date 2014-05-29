using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.FuelApp.Logic.SL.ViewModels;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class FuelReportDetailListVM : WorkspaceViewModel, IEventHandler<FuelReportDetailListChangeArg>
    {
        #region Prop

        private IFuelController _fuelController;
        private IFuelReportDetailController _fuelReportDetailController;
        private IFuelReportServiceWrapper _fuelReportServiceWrapper;
        private IInventoryOperationServiceWrapper inventoryOperationServiceWrapper;

        private IGoodServiceWrapper _goodServiceWrapper;
        private CommandViewModel editCommand;

        private PagedSortableCollectionView<FuelReportDetailVM> _fuelReportDetailVms;
        public PagedSortableCollectionView<FuelReportDetailVM> FuelReportDetailVms
        {
            get { return _fuelReportDetailVms; }
            set
            {
                this.SetField(p => p.FuelReportDetailVms, ref _fuelReportDetailVms, value);

            }
        }

        private PagedSortableCollectionView<FuelReportInventoryOperationVM> _dataInventoryOperation;
        public PagedSortableCollectionView<FuelReportInventoryOperationVM> DataInventoryOperation
        {
            get { return _dataInventoryOperation; }
            set { this.SetField(p => p.DataInventoryOperation, ref _dataInventoryOperation, value); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                this.SetField(p => p.IsActive, ref isActive, value);
            }
        }


        private ObservableCollection<FuelReportDetailDto> _fuelReportDetailDtos;
        public ObservableCollection<FuelReportDetailDto> FuelReportDetailDtos
        {
            get { return _fuelReportDetailDtos; }
            set
            {
                this.SetField(p => p.FuelReportDetailDtos, ref _fuelReportDetailDtos, value);

            }
        }

        //private ObservableCollection<FuelReportCorrectionReferenceNoDto> _fuelReportCorrectionReferenceNoDtos;
        //public ObservableCollection<FuelReportCorrectionReferenceNoDto> FuelReportCorrectionReferenceNoDtos
        //{
        //    get { return _fuelReportCorrectionReferenceNoDtos; }
        //    set
        //    {
        //        this.SetField(p => p.FuelReportCorrectionReferenceNoDtos, ref _fuelReportCorrectionReferenceNoDtos, value);

        //    }
        //}


        private FuelReportDetailVM selectedFuelReportDetailVm;
        public FuelReportDetailVM SelectedFuelReportDetailVm
        {
            get { return selectedFuelReportDetailVm; }
            set
            {
                this.SetField(p => p.SelectedFuelReportDetailVm, ref selectedFuelReportDetailVm, value);
                LoadInventoryOperations();


            }
        }

        private FuelReportVM fuelReportVMSelected;


        public FuelReportVM FuelReportVMSelected
        {
            get { return fuelReportVMSelected; }
            set
            {
                this.SetField(p => p.FuelReportVMSelected, ref fuelReportVMSelected, value);
                if (fuelReportVMSelected != null)
                    if (fuelReportVMSelected.Entity.Id != -1)
                    {

                        Load();

                    }


            }
        }



        public CommandViewModel EditCommand
        {
            get
            {

                if (editCommand == null)
                {

                    editCommand = new CommandViewModel("ویرایش", new DelegateCommand(
                                                                            () =>
                                                                            {
                                                                                if (!checkIsSelected()) return;
                                                                                _fuelReportDetailController.Update(SelectedFuelReportDetailVm.Entity);
                                                                                //this.FuelReportDetailVms.PageIndex++;
                                                                                //this.FuelReportDetailVms.Refresh();
                                                                            }));


                }
                return editCommand;

            }
        }

        #endregion

        #region Ctor

        public FuelReportDetailListVM()
        {


        }
        public FuelReportDetailListVM(IFuelReportServiceWrapper serviceWrapper, IFuelController fuelController, IFuelReportDetailController fuelReportDetailController, IGoodServiceWrapper goodServiceWrapper, IInventoryOperationServiceWrapper inventoryOperationServiceWrapper)
        {
            this._fuelReportServiceWrapper = serviceWrapper;
            this._fuelController = fuelController;
            this._fuelReportDetailController = fuelReportDetailController;
            this._goodServiceWrapper = goodServiceWrapper;
            this.FuelReportDetailDtos = new ObservableCollection<FuelReportDetailDto>();
            this.inventoryOperationServiceWrapper = inventoryOperationServiceWrapper;
            DataInventoryOperation = new PagedSortableCollectionView<FuelReportInventoryOperationVM>();

            FuelReportDetailVms = new PagedSortableCollectionView<FuelReportDetailVM>();
            FuelReportDetailVms.OnRefresh += (s, args) => Load();
        }

        #endregion

        #region Method

        public void Load()
        {
            SetCollection();
        }

        private void SetCollection()
        {

            if (fuelReportVMSelected != null)
            {
                this._fuelReportServiceWrapper.GetById((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                                                                                                              {
                                                                                                                  ShowBusyIndicator("در حال دریافت اطلاعات جزئیات گزارش ...");
                                                                                                                  if (exp == null)
                                                                                                                  {
                                                                                                                      FuelReportDetailVms.Clear();
                                                                                                                      FuelReportDetailVms.SourceCollection = createFuelReportDetail(res);
                                                                                                                      IsActive
                                                                                                                          =
                                                                                                                          res
                                                                                                                              .
                                                                                                                              IsActive;
                                                                                                                  }
                                                                                                                  else
                                                                                                                  {
                                                                                                                      _fuelController.HandleException(exp);
                                                                                                                  }
                                                                                                                  HideBusyIndicator();
                                                                                                              }), fuelReportVMSelected.Entity.Id);



            }



        }

        private FuelReportInventoryOperationVM CreateInventoryOperationItem(FuelReportInventoryOperationDto dto)
        {
            var result = new FuelReportInventoryOperationVM(this._fuelController);
            result.SetEntity(dto);
            return result;
        }

        private ObservableCollection<FuelReportDetailVM> createFuelReportDetail(FuelReportDto dto)
        {

            var result = new ObservableCollection<FuelReportDetailVM>();
            if (dto.FuelReportDetail != null && dto.FuelReportDetail.Count > 0)
            {
                dto.FuelReportDetail.ToList().ForEach(c =>
                                                              {

                                                                  var x =
                                                                      ServiceLocator.Current.GetInstance
                                                                          <FuelReportDetailVM>();
                                                                  x.Entity = c;
                                                                  result.Add(x);

                                                              });

            }
            return result;
        }

        private void LoadInventoryOperations()
        {
            DataInventoryOperation.SourceCollection = new List<FuelReportInventoryOperationVM>();

            if (SelectedFuelReportDetailVm != null)
            {
                DataInventoryOperation = new PagedSortableCollectionView<FuelReportInventoryOperationVM>();
                this.inventoryOperationServiceWrapper.GetFuelReportInventoryOperations((res, exp) =>
                                                this._fuelController.BeginInvokeOnDispatcher(() =>
                                                {
                                                    ShowBusyIndicator("در حال دریافت اطلاعات حواله و رسیدها");
                                                    if (exp == null)
                                                    {
                                                        if (res.Count > 0)
                                                        {
                                                            //DataInventoryOperation.SourceCollection = new List<FuelReportInventoryOperationVM>();
                                                            //foreach (var item in res)
                                                            //{
                                                            //    var vm = this.CreateInventoryOperationItem(item);
                                                            //    DataInventoryOperation.SourceCollection.Add(vm);
                                                            //}
                                                            //DataInventoryOperation.SourceCollection = inventoryOperationList;
                                                            DataInventoryOperation.SourceCollection = res.Select(CreateInventoryOperationItem);
                                                            DataInventoryOperation.PageIndex = 0;
                                                        }


                                                    }
                                                    else
                                                    {
                                                        _fuelController.HandleException(exp);
                                                    }
                                                    HideBusyIndicator();
                                                }), SelectedFuelReportDetailVm.Entity.FuelReportId, SelectedFuelReportDetailVm.Entity.Id);
            }
        }

        #endregion


        public void Handle(FuelReportDetailListChangeArg eventData)
        {
            Load();

        }

        private bool checkIsSelected()
        {
            if (SelectedFuelReportDetailVm == null)
            {
                _fuelController.ShowMessage("لطفا جزئیات گزارش مورد نظر را انتخاب نمایید");
                return false;
            }
            else return true;
        }
    }
}

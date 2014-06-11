using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class CharterStartVM : WorkspaceViewModel, IEventHandler<CharterStartItemListChangeArg>
    {
        #region Prop

        private ICharterController _charterController;
        CharterType CurrentCharterType { get; set; }


        private IFuelController _fuelController;
        private ICharterInServiceWrapper _charterInServiceWrapper;
        private ICharterOutServiceWrapper _charterOutServiceWrapper;
        private ICompanyServiceWrapper _companyServiceWrapper;


        private string companyName;
        public string CompanyName
        {
            get { return companyName; }
            set
            {
                this.SetField(p => p.CompanyName, ref companyName, value);

            }
        }

        private bool viewFlag;
        public bool ViewFlag
        {
            get { return viewFlag; }
            set
            {
                this.SetField(p => p.ViewFlag, ref viewFlag, value);

            }
        }

        private CharterDto _charterDto;
        public CharterDto Entity
        {
            get { return _charterDto; }
            set
            {
                this.SetField(p => p.Entity, ref _charterDto, value);
                DataInventoryOperation.Clear();
                if (Entity.InventoryOperationDtos != null)
                {
                    Entity.InventoryOperationDtos.ToList().ForEach(c => DataInventoryOperation.Add(c));
                }

                ViewFlag = Entity.IsFinalApproveVisiblity;


            }
        }

        private ObservableCollection<CurrencyDto> _currencyDtos;
        public ObservableCollection<CurrencyDto> CurrencyDtos
        {
            get { return _currencyDtos; }
            set
            {
                this.SetField(p => p.CurrencyDtos, ref _currencyDtos, value);

            }
        }

        private ObservableCollection<CharterItemDto> _charterItemDtos;
        public ObservableCollection<CharterItemDto> CharterItemDtos
        {
            get { return _charterItemDtos; }
            set
            {
                this.SetField(p => p.CharterItemDtos, ref _charterItemDtos, value);
            }
        }


        private ObservableCollection<FuelReportInventoryOperationDto> _dataInventoryOperation;
        public ObservableCollection<FuelReportInventoryOperationDto> DataInventoryOperation
        {
            get { return _dataInventoryOperation; }
            set { this.SetField(p => p.DataInventoryOperation, ref _dataInventoryOperation, value); }
        }

        private List<ComboBoxItm> offireTypeEnums;
        public List<ComboBoxItm> OffHireTypeEnums
        {
            get { return offireTypeEnums; }
            set
            {
                this.SetField(p => p.OffHireTypeEnums, ref offireTypeEnums, value);
            }
        }

        private string caption;
        public string Caption
        {
            get { return caption; }
            set
            {
                this.SetField(p => p.Caption, ref caption, value);

            }
        }

        private long charterId;
        public long CharterId
        {
            get { return charterId; }
            set
            {
                this.SetField(p => p.CharterId, ref charterId, value);
                if (CharterId > 0)
                {
                    ViewFlag = true;

                    Load();
                }
                else
                {
                    ViewFlag = false;
                    //Entity.IsFinalApproveVisiblity = true;
                }



            }
        }

        private long selectedTypeId;
        public long SelectedTypeId
        {
            get { return selectedTypeId; }
            set
            {
                this.SetField(p => p.SelectedTypeId, ref selectedTypeId, value);

            }
        }


        private long selectedVesselId;
        public long SelectedVesselId
        {
            get { return selectedVesselId; }
            set
            {
                this.SetField(p => p.SelectedVesselId, ref selectedVesselId, value);

            }
        }

        private long selectedCurrencyId;
        public long SelectedCurrencyId
        {
            get { return selectedCurrencyId; }
            set
            {
                this.SetField(p => p.SelectedCurrencyId, ref selectedCurrencyId, value);

            }
        }

        private long selectedOwnerId;
        public long SelectedOwnerId
        {
            get { return selectedOwnerId; }
            set
            {
                this.SetField(p => p.SelectedOwnerId, ref selectedOwnerId, value);

            }
        }


        private ObservableCollection<CompanyDto> companyDtos;
        public ObservableCollection<CompanyDto> CompanyDtos
        {
            get { return companyDtos; }
            set
            {
                this.SetField(p => p.CompanyDtos, ref companyDtos, value);
            }
        }


        private CharterItemDto selectedCharterItem;
        public CharterItemDto SelectedCharterItem
        {
            get { return selectedCharterItem; }
            set
            {
                this.SetField(p => p.SelectedCharterItem, ref selectedCharterItem, value);


            }
        }

        private ObservableCollection<VesselDto> _vesselDtos;
        public ObservableCollection<VesselDto> VesselDtos
        {
            get { return _vesselDtos; }
            set
            {
                this.SetField(p => p.VesselDtos, ref _vesselDtos, value);
            }
        }

        #region Command

        private CommandViewModel addCommand;
        public CommandViewModel AddCommand
        {
            get
            {
                addCommand = new CommandViewModel("افزودن", new DelegateCommand(() =>
                {
                    if (CurrentCharterType == CharterType.In)
                    {
                        _charterController.AddCharterInItem(CharterStateTypeEnum.Start, CharterId, 0);
                    }
                    else
                    {
                        _charterController.AddCharterOutItem(CharterStateTypeEnum.Start, CharterId, 0);
                    }

                }));
                return addCommand;
            }
        }


        private CommandViewModel editCommand;
        public CommandViewModel EditCommand
        {
            get
            {
                editCommand = new CommandViewModel("ویرایش", new DelegateCommand(() =>
                {

                    if (!checkIsSelected()) return;
                    if (CurrentCharterType == CharterType.In)
                    {
                        _charterController.UpdateCharterInItem(CharterStateTypeEnum.Start, CharterId, SelectedCharterItem.Id);

                    }
                    else
                    {
                        _charterController.UpdateCharterOutItem(CharterStateTypeEnum.Start, CharterId, SelectedCharterItem.Id);

                    }

                }));
                return editCommand;
            }

        }


        private CommandViewModel deleteCommand;
        public CommandViewModel DeleteCommand
        {
            get
            {
                deleteCommand = new CommandViewModel("حذف", new DelegateCommand(() =>
                {

                    if (!checkIsSelected()) return;
                    if (_fuelController.ShowConfirmationBox("آیا برای حذف مطمئن هستید ",
                                                                                  "اخطار"))
                    {

                        if (CurrentCharterType == CharterType.In)
                        {
                            _charterInServiceWrapper.DeleteItem((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                                                                                                                      {
                                                                                                                          ShowBusyIndicator("درحال انجام حذف");
                                                                                                                          if (exp == null)
                                                                                                                          { Load(); }
                                                                                                                          else
                                                                                                                          {
                                                                                                                              _fuelController.HandleException(exp);
                                                                                                                          }
                                                                                                                          HideBusyIndicator();
                                                                                                                      }), CharterStateTypeEnum.Start, charterId, SelectedCharterItem.Id);

                        }
                        else
                        {
                            _charterOutServiceWrapper.DeleteItem((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                            {
                                ShowBusyIndicator("درحال انجام حذف");
                                if (exp == null)
                                {
                                    Load();
                                }
                                else
                                {
                                    _fuelController.HandleException(exp);
                                }
                                HideBusyIndicator();
                            }), CharterStateTypeEnum.Start, charterId, SelectedCharterItem.Id);

                        }
                    }



                }));
                return deleteCommand;
            }

        }


        private CommandViewModel cancelCommand;
        public CommandViewModel CancelCommand
        {
            get
            {
                cancelCommand = new CommandViewModel("انصراف", new DelegateCommand(() =>
                {

                    this._fuelController.Close(this);

                }));
                return cancelCommand;
            }

        }


        private CommandViewModel submitCommand;
        public CommandViewModel SubmitCommand
        {
            get
            {
                submitCommand = new CommandViewModel("ذخیره", new DelegateCommand(() =>
                {

                    if (CurrentCharterType == CharterType.In)
                    {

                        SubmitCharterIn();

                    }
                    else
                    {
                        SubmitCharterOut();
                    }

                }));
                return submitCommand;
            }

        }

        #endregion

        #endregion

        #region Ctor

        public CharterStartVM()
        {

            this.DataInventoryOperation = new ObservableCollection<FuelReportInventoryOperationDto>();

            this.Entity = new CharterDto();
            this.Entity.IsFinalApproveVisiblity = true;
            this.Entity.Vessel = new VesselDto();
            this.Entity.Owner = new CompanyDto();
            this.Entity.Currency = new CurrencyDto();

            this.CurrentCharterType = new CharterType();

            this.CurrencyDtos = new ObservableCollection<CurrencyDto>();
            this.CompanyDtos = new ObservableCollection<CompanyDto>();
            this.VesselDtos = new ObservableCollection<VesselDto>();
            this.CharterItemDtos = new ObservableCollection<CharterItemDto>();
            this.OffHireTypeEnums = new List<ComboBoxItm>();
        }

        public CharterStartVM(ICharterController charterController, IFuelController fuelController,
                              ICharterInServiceWrapper charterInServiceWrapper, ICharterOutServiceWrapper charterOutServiceWrapper
                              , ICompanyServiceWrapper companyServiceWrapper)
        {
            this._charterController = charterController;
            this._fuelController = fuelController;
            this._charterInServiceWrapper = charterInServiceWrapper;
            this._charterOutServiceWrapper = charterOutServiceWrapper;
            this._companyServiceWrapper = companyServiceWrapper;

            this.DataInventoryOperation = new ObservableCollection<FuelReportInventoryOperationDto>();

            this.Entity = new CharterDto();
            this.Entity.IsFinalApproveVisiblity = true;
            this.Entity.Vessel = new VesselDto();
            this.Entity.Owner = new CompanyDto();
            this.Entity.Currency = new CurrencyDto();

            this.CurrentCharterType = new CharterType();

            this.CurrencyDtos = new ObservableCollection<CurrencyDto>();
            this.CompanyDtos = new ObservableCollection<CompanyDto>();
            this.VesselDtos = new ObservableCollection<VesselDto>();
            this.CharterItemDtos = new ObservableCollection<CharterItemDto>();
            this.OffHireTypeEnums = new List<ComboBoxItm>();


        }

        #endregion

        #region Method
        public void SetCharterType(CharterType charterType)
        {
            CurrentCharterType = charterType;
            if (charterType == CharterType.In)
            {
                this.DisplayName = "چارتر In";
                Caption = "مالک : ";
            }
            else
            {
                this.DisplayName = "چارتر Out";
                Caption = "چارترر : ";
            }


        }

        void Load()
        {
            if (CurrentCharterType == CharterType.In)
            {
                CharterInLoad();
            }
            else
            {
                CharterOutLoad();
            }

            LoadGeneralItems();
        }

        public void LoadGeneralItems()
        {
            OffHireTypeEnums.Clear();
            OffHireTypeEnums =
                       MITD.Fuel.Presentation.Logic.SL.Infrastructure.EnumHelper.GetItems(typeof(OffHirePricingType));

            _companyServiceWrapper.GetAll((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
            {
                ShowBusyIndicator("درحال دریافت اطلاعات مالکان");
                if (exp == null)
                {
                    companyDtos.Clear();
                    res.Result.ToList()
                        .ForEach(
                            c =>
                            companyDtos
                                .Add(c));

                    //todo bzcomment xxxxx
                   // Entity.Charterer = companyDtos.Where(c => c.Id == _fuelController.GetCurrentUser().CompanyDto.Id).SingleOrDefault();
                    if (Entity.Charterer != null)
                        CompanyName = Entity.Charterer.Name;
                }
                else
                {
                    _fuelController.HandleException(exp);
                }
                HideBusyIndicator();
            }), "");


            //todo bzcomment must change for Out xxxxxxx
            if (CurrentCharterType == CharterType.In)
            {
                ShowBusyIndicator("درحال دریافت اطلاعات کشتی ها");
            //    _charterInServiceWrapper.GetAllIdelVessels((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
            //    {

            //        if (exp == null)
            //        {
            //            VesselDtos.Clear();
            //            res.Result.ToList()
            //                .ForEach(
            //                    c =>
            //                    VesselDtos.
            //                        Add(c));

            //        }
            //        else
            //        {
            //            _fuelController.HandleException(exp);
            //        }
            //        HideBusyIndicator();
            //    }), _fuelController.GetCurrentUser().CompanyDto.Id);
            //}
            //else
            //{
            //    ShowBusyIndicator("درحال دریافت اطلاعات کشتی ها");
            //    _charterOutServiceWrapper.GetAllIdelVessels((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
            //    {

            //        if (exp == null)
            //        {
            //            VesselDtos.Clear();
            //            res.Result.ToList()
            //                .ForEach(
            //                    c =>
            //                    VesselDtos.
            //                        Add(c));

            //        }
            //        else
            //        {
            //            _fuelController.HandleException(exp);
            //        }
            //        HideBusyIndicator();
            //    }), _fuelController.GetCurrentUser().CompanyDto.Id);
            }


            _charterInServiceWrapper.GetAllCurrencies((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
            {
                HideBusyIndicator();
                if (exp == null)
                {
                    CurrencyDtos.Clear();
                    foreach (var cur in res)
                    {
                        CurrencyDtos.Add(cur);
                    }


                }
                else
                {
                    _fuelController
                        .
                        HandleException
                        (exp);
                }
            }));


        }


        void CharterInLoad()
        {
            // todo bz comment
            if (CharterId > 0)
                _charterInServiceWrapper.GetById((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                {
                    ShowBusyIndicator("درحال دریافت اطلاعات...");
                    if (exp == null)
                    {
                        Entity = res;
                        SelectedOwnerId = res.Owner.Id;
                        SelectedVesselId = res.Vessel.Id;
                        SelectedCurrencyId = res.Currency.Id;
                        CompanyName = res.Charterer.Name;

                        SelectedTypeId = (int)res.OffHirePricingType;
                        CharterItemDtos = res.CharterItems;
                        DataInventoryOperation = res.InventoryOperationDtos;
                    }
                    else
                    {
                        _fuelController.HandleException(exp);
                    }

                    HideBusyIndicator();


                }), CharterStateTypeEnum.Start, CharterId);

        }

        void CharterOutLoad()
        {

            //todo bzcomment
            if (CharterId > 0)
                _charterOutServiceWrapper.GetById((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                {
                    ShowBusyIndicator("درحال دریافت اطلاعات...");
                    if (exp == null)
                    {
                        Entity = res;

                        SelectedOwnerId = res.Charterer.Id;
                        SelectedVesselId = res.Vessel.Id;
                        SelectedCurrencyId = res.Currency.Id;
                        CompanyName = res.Owner.Name;
                        SelectedTypeId = (int)res.OffHirePricingType;
                        CharterItemDtos = res.CharterItems;
                        DataInventoryOperation = res.InventoryOperationDtos;
                    }
                    else
                    {
                        _fuelController.HandleException(exp);
                    }

                    HideBusyIndicator();


                }), CharterStateTypeEnum.Start, CharterId);
        }

        private bool checkIsSelected()
        {
            if (SelectedCharterItem == null)
            {
                _fuelController.ShowMessage("لطفا ایتم مورد نظر را انتخاب فرمائید");
                return false;
            }
            else return true;
        }

        public void Handle(CharterStartItemListChangeArg eventData)
        {

            if (CurrentCharterType == CharterType.In)
            {
                CharterInLoad();
            }
            else
            {
                CharterOutLoad();
            }


        }


        void SubmitCharterIn()
        {

            // Todo Must remove bzcomment xxxxxxxxxxxxx

            Entity.Currency.Id = selectedCurrencyId;
            Entity.Vessel.Id = selectedVesselId;
            Entity.Owner.Id = selectedOwnerId;
         //   Entity.Charterer.Id = _fuelController.GetCurrentUser().CompanyDto.Id;
            Entity.OffHirePricingType = (OffHirePricingType)SelectedTypeId;
            Entity.CharterStateType = CharterStateTypeEnum.Start;
            if (!Entity.Validate()) return;

            if (Entity.Id > 0)
            {
                _charterInServiceWrapper.Update((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                {
                    ShowBusyIndicator("درحال بروزرسانی..");
                    if (exp == null)
                    {
                        Entity
                            =
                            res;

                        _fuelController.Publish<CharterListChangeArg>(new CharterListChangeArg());
                        _fuelController.Close(this);
                    }
                    else
                    {
                        _fuelController.HandleException(exp);
                    }
                    HideBusyIndicator();
                }), Entity.Id, Entity);
            }
            else
            {

                _charterInServiceWrapper.Add((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                {
                    ShowBusyIndicator("درحال انجام عملیات..");
                    if (exp == null)
                    {
                        Entity
                            =
                            res;
                        _fuelController.Publish<CharterListChangeArg>(new CharterListChangeArg());
                        _fuelController.Close(this);
                    }
                    else
                    {
                        _fuelController.HandleException(exp);
                    }
                    HideBusyIndicator();
                }), Entity);
            }
        }

        void SubmitCharterOut()
        {



            // Todo Must remove bzcomment

            Entity.Currency.Id = selectedCurrencyId;
            Entity.Vessel.Id = selectedVesselId;
            Entity.Owner.Id = _fuelController.GetCurrentUser().CompanyDto.Id;
            Entity.Charterer.Id = SelectedOwnerId;
            Entity.OffHirePricingType = (OffHirePricingType)SelectedTypeId;
            Entity.CharterStateType = CharterStateTypeEnum.Start;
            if (!Entity.Validate()) return;

            if (Entity.Id > 0)
            {
                _charterOutServiceWrapper.Update((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                {
                    ShowBusyIndicator("درحال بروزرسانی..");
                    if (exp == null)
                    {
                        Entity
                            =
                            res;

                        _fuelController.Publish<CharterListChangeArg>(new CharterListChangeArg());
                        _fuelController.Close(this);
                    }
                    else
                    {
                        _fuelController.HandleException(exp);
                    }
                    HideBusyIndicator();
                }), Entity.Id, Entity);
            }
            else
            {

                _charterOutServiceWrapper.Add((res, exp) => _fuelController.BeginInvokeOnDispatcher(() =>
                {
                    ShowBusyIndicator("درحال انجام عملیات..");
                    if (exp == null)
                    {
                        Entity
                            =
                            res;
                        _fuelController.Publish<CharterListChangeArg>(new CharterListChangeArg());
                        _fuelController.Close(this);
                    }
                    else
                    {
                        _fuelController.HandleException(exp);
                    }
                    HideBusyIndicator();
                }), Entity);
            }
        }

        #endregion
    }
}

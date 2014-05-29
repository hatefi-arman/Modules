using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using MITD.Fuel.Presentation.Contracts;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using MITD.Presentation;

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class FuelReportDetailVM : WorkspaceViewModel
    {
        #region Prop

        private IFuelController mainController;
        private IFuelReportServiceWrapper serviceWrapper;
        private IGoodServiceWrapper _goodServiceWrapper;

        private CommandViewModel submitCommand;
        public CommandViewModel SubmitCommand
        {
            get
            {
                if (submitCommand == null)
                    submitCommand = new CommandViewModel("ذخیره", new DelegateCommand(Save));

                return submitCommand;
            }
        }

        private CommandViewModel cancelCommand;
        public CommandViewModel CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {

                    cancelCommand = new CommandViewModel("انصراف", new DelegateCommand(() =>
                    {
                        this.mainController.Close(this);
                    }));
                }
                return cancelCommand;
            }
        }


        private FuelReportDetailDto entity;
        public FuelReportDetailDto Entity
        {
            get { return entity; }
            set { this.SetField(p => p.Entity, ref entity, value); }
        }

        private bool isFinalApprove;
        public bool IsFinalApprove
        {
            get { return isFinalApprove; }
            set { this.SetField(p => p.IsFinalApprove, ref isFinalApprove, value); }
        }

        private List<ComboBoxItm> correctionTypes;
        public List<ComboBoxItm> CorrectionTypes
        {
            get { return correctionTypes; }
            set
            {
                this.SetField(p => p.CorrectionTypes, ref correctionTypes, value);
            }
        }

        private List<ComboBoxItm> transferTypes;
        public List<ComboBoxItm> TransferTypes
        {
            get { return transferTypes; }
            set
            {
                this.SetField(p => p.TransferTypes, ref transferTypes, value);
            }
        }

        private List<ComboBoxItm> receiveTypes;
        public List<ComboBoxItm> ReceiveTypes
        {
            get { return receiveTypes; }
            set
            {
                this.SetField(p => p.ReceiveTypes, ref receiveTypes, value);
            }
        }

        //private ObservableCollection<CorrectionTypeEnum> correctionTypes;
        //public ObservableCollection<CorrectionTypeEnum> CorrectionTypes
        //{
        //    get { return correctionTypes; }
        //    set
        //    {
        //        this.SetField(p => p.CorrectionTypes, ref correctionTypes, value);
        //    }
        //}

        //private ObservableCollection<TransferTypeEnum> transferTypes;
        //public ObservableCollection<TransferTypeEnum> TransferTypes
        //{
        //    get { return transferTypes; }
        //    set
        //    {
        //        this.SetField(p => p.TransferTypes, ref transferTypes, value);
        //    }
        //}

        //private ObservableCollection<ReceiveTypeEnum> receiveTypes;
        //public ObservableCollection<ReceiveTypeEnum> ReceiveTypes
        //{
        //    get { return receiveTypes; }
        //    set
        //    {
        //        this.SetField(p => p.ReceiveTypes, ref receiveTypes, value);
        //    }
        //}


        private long correctionId;
        public long CorrectionId
        {
            get { return correctionId; }
            set
            {
                this.SetField(p => p.CorrectionId, ref correctionId, value);
                Entity.CorrectionType = (CorrectionTypeEnum)CorrectionId;
            }
        }

        private long transferId;
        public long TransferId
        {
            get { return transferId; }
            set
            {
                this.SetField(p => p.TransferId, ref transferId, value);
                Entity.TransferType = (TransferTypeEnum)TransferId;
            }
        }

        private long receiveId;
        public long ReceiveId
        {
            get { return receiveId; }
            set
            {
                this.SetField(p => p.ReceiveId, ref receiveId, value);
                Entity.ReceiveType = (ReceiveTypeEnum)ReceiveId;
            }
        }

        private ObservableCollection<FuelReportCorrectionReferenceNoDto> _fuelReportCorrectionReferenceNoDtos;
        public ObservableCollection<FuelReportCorrectionReferenceNoDto> FuelReportCorrectionReferenceNoDtos
        {
            get { return _fuelReportCorrectionReferenceNoDtos; }
            set
            {
                this.SetField(p => p.FuelReportCorrectionReferenceNoDtos, ref _fuelReportCorrectionReferenceNoDtos, value);

            }
        }

        private ObservableCollection<FuelReportReciveReferenceNoDto> _fuelReportReciveReferenceNoDtos;
        public ObservableCollection<FuelReportReciveReferenceNoDto> FuelReportReciveReferenceNoDtos
        {
            get { return _fuelReportReciveReferenceNoDtos; }
            set
            {
                this.SetField(p => p.FuelReportReciveReferenceNoDtos, ref _fuelReportReciveReferenceNoDtos, value);

            }
        }

        private ObservableCollection<FuelReportTransferReferenceNoDto> _fuelReportTransferReferenceNoDto;
        public ObservableCollection<FuelReportTransferReferenceNoDto> FuelReportTransferReferenceNoDtos
        {
            get { return _fuelReportTransferReferenceNoDto; }
            set
            {
                this.SetField(p => p.FuelReportTransferReferenceNoDtos, ref _fuelReportTransferReferenceNoDto, value);

            }
        }

        private ObservableCollection<GoodUnitDto> unitDtos;
        public ObservableCollection<GoodUnitDto> UnitDtos
        {


            get
            {

                return unitDtos;
            }
            set
            {
                this.SetField(vm => vm.UnitDtos, ref unitDtos, value);
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

        public string ReceiveTypeName
        {

            get
            {
                var str = "";
                str = Entity.ReceiveType.ToString();
                str = (string.IsNullOrEmpty(str) ? "" : str);

                return str;
            }
        }

        public string TransferTypeName
        {

            get
            {
                var str = "";

                str = Entity.TransferType.ToString();
                str = (string.IsNullOrEmpty(str) ? "" : str);


                return str;
            }
        }

        public string CorrectionName
        {
            get
            {
                var str = "";


                str = Entity.CorrectionType.ToString();
                str = (string.IsNullOrEmpty(str) ? "" : str);


                return str;
            }
        }


        private bool _isTransferTypeActive;
        public bool IsTransferTypeActive
        {
            get { return _isTransferTypeActive; }
            set { this.SetField(p => p.IsTransferTypeActive, ref _isTransferTypeActive, value); }
        }

        private bool _isTransferReferenceActive;
        public bool IsTransferReferenceActive
        {
            get { return _isTransferReferenceActive; }
            set { this.SetField(p => p.IsTransferReferenceActive, ref _isTransferReferenceActive, value); }
        }

        private bool _isReceiveTypeActive;
        public bool IsReceiveTypeActive
        {
            get { return _isReceiveTypeActive; }
            set { this.SetField(p => p.IsReceiveTypeActive, ref _isReceiveTypeActive, value); }
        }

        private bool _isReceiveReferenceActive;
        public bool IsReceiveReferenceActive
        {
            get { return _isReceiveReferenceActive; }
            set { this.SetField(p => p.IsReceiveReferenceActive, ref _isReceiveReferenceActive, value); }
        }

        private bool _isCorrectionTypeActive;
        public bool IsCorrectionTypeActive
        {
            get { return _isCorrectionTypeActive; }
            set { this.SetField(p => p.IsCorrectionTypeActive, ref _isCorrectionTypeActive, value); }
        }

        private bool _isCorrectionReferenceActive;
        public bool IsCorrectionReferenceActive
        {
            get { return _isCorrectionReferenceActive; }
            set { this.SetField(p => p.IsCorrectionReferenceActive, ref _isCorrectionReferenceActive, value); }
        }



        #endregion

        #region Ctor

        public FuelReportDetailVM(IFuelController appController, IFuelReportServiceWrapper fuelReportServiceWrapper, IGoodServiceWrapper goodServiceWrapper)
        {
            this.mainController = appController;
            this.serviceWrapper = fuelReportServiceWrapper;
            this._goodServiceWrapper = goodServiceWrapper;

            //var gg = Enum.GetValues(typeof(CorrectionTypeEnum)) as CorrectionTypeEnum[];

            this.FuelReportTransferReferenceNoDtos = new ObservableCollection<FuelReportTransferReferenceNoDto>();

            UnitDtos = new ObservableCollection<GoodUnitDto>();
            setEntity(new FuelReportDetailDto());
            DisplayName = "جزئیات گزارش ";
        }

        #endregion

        #region Method

        void GetCurrency()
        {
            CurrencyDtos = new ObservableCollection<CurrencyDto>();
            ShowBusyIndicator("دریافت واحد ارز");
            this.serviceWrapper.GetAllCurrency((res, exp) => this.mainController.BeginInvokeOnDispatcher(() =>
            {
                if (exp == null)
                {
                    res.ForEach(c => CurrencyDtos.Add(c));
                }
                else
                {
                    mainController.HandleException(exp);
                }

                HideBusyIndicator();
            }));
        }

        public void CheckEnable()
        {
            if (this.Entity.Recieve.HasValue && this.Entity.Recieve > 0)
            {
                this.IsReceiveTypeActive = this.IsReceiveReferenceActive = true;
            }
            else
            {
                this.IsReceiveTypeActive = this.IsReceiveReferenceActive = false;
                this.ReceiveId = (long)ReceiveTypeEnum.NotDefined;
                Entity.FuelReportReciveReferenceNoDto = null;
            }

            if (this.Entity.Transfer.HasValue && this.Entity.Transfer > 0)
            {
                this.IsTransferTypeActive = this.IsTransferReferenceActive = true;
            }
            else
            {
                this.IsTransferTypeActive = this.IsTransferReferenceActive = false;
                this.TransferId = (long)TransferTypeEnum.NotDefined;
                Entity.FuelReportTransferReferenceNoDto = null;
            }

            if (this.Entity.Correction.HasValue && this.Entity.Correction > 0)
            {
                this.IsCorrectionTypeActive = this.IsCorrectionReferenceActive = true;
            }
            else
            {
                this.IsCorrectionTypeActive = this.IsCorrectionReferenceActive = false;
                this.CorrectionId = (long)CorrectionTypeEnum.NotDefined;
                Entity.CurrencyDto = null;
                Entity.CorrectionPrice = null;
                Entity.FuelReportCorrectionReferenceNoDto = null;
            }

            /* if (IsFinalApprove)
             {
                 if (this.IsCorrectionTypeActive)
                 {
                     this.IsCorrectionReferenceActive = true;
                 }
                 else
                 {
                     this.IsCorrectionReferenceActive = false;
                 }

                 if (this.IsReceiveTypeActive)
                 {
                     this.IsReceiveReferenceActive = true;
                 }
                 else
                 {
                     this.IsReceiveReferenceActive = false;
                 }

                 if (this.IsTransferTypeActive)
                 {
                     this.IsTransferReferenceActive = true;
                 }
                 else
                 {
                     this.IsTransferReferenceActive = false;
                 }
             }*/
        }

        private void Save()
        {
            if (!Entity.Validate()) return;

            Entity.ReceiveType = (ReceiveTypeEnum)ReceiveId;
            Entity.TransferType = (TransferTypeEnum)TransferId;
            Entity.CorrectionType = (CorrectionTypeEnum)CorrectionId;
            //Entity.Recieve = ReceiveValue;
            //Entity.Transfer = TransferValue;
            //Entity.Correction = CorrectionValue;

            serviceWrapper.UpdateFuelReportDetail((res, exp) => mainController.BeginInvokeOnDispatcher(() =>
            {
                HideBusyIndicator();
                if (exp != null)
                    mainController.HandleException(exp);
                else
                {
                    setEntity(res);
                    mainController.Publish(new FuelReportDetailListChangeArg());
                    mainController.Close(this);
                }
            }), Entity);
        }

        public void Load(FuelReportDetailDto fuelReportDetailDto)
        {
            GetCurrency();
            SetCollection();

            ShowBusyIndicator("در حا دریافت جزئیات گزارش");
            this.serviceWrapper.GetById((res, exp) =>
                mainController.BeginInvokeOnDispatcher(() =>
                {
                    if (exp == null)
                    {
                        setEntity(res.FuelReportDetail.SingleOrDefault(c => c.Id == fuelReportDetailDto.Id));
                        ReceiveId = (int)Entity.ReceiveType;
                        TransferId = (int)Entity.TransferType;
                        CorrectionId = (int)Entity.CorrectionType;
                        //ReceiveValue =
                        //    Entity.Recieve.Value;
                        //TransferValue =
                        //    Entity.
                        //        Transfer.
                        //        Value;
                        //CorrectionValue =
                        //    Entity.
                        //        Correction
                        //        .Value;
                        IsFinalApprove = res.IsNextActionFinalApprove;
                        CheckEnable();
                    }
                    else
                    {
                        mainController.HandleException(exp);
                    }


                    HideBusyIndicator();

                }), fuelReportDetailDto.FuelReportId);

        }

        void SetCollection()
        {
            //CorrectionTypes = Infrastructure.EnumHelper.GetItems(typeof(CorrectionTypeEnum));
            //CorrectionTypes.First(cmbitem => cmbitem.Id == (long)CorrectionTypeEnum.NotDefined).Name = " ";

            //TransferTypes = Infrastructure.EnumHelper.GetItems(typeof(TransferTypeEnum));
            //TransferTypes.First(cmbitem => cmbitem.Id == (long)TransferTypeEnum.NotDefined).Name = " ";

            //ReceiveTypes = Infrastructure.EnumHelper.GetItems(typeof(ReceiveTypeEnum));
            //ReceiveTypes.First(cmbitem => cmbitem.Id == (long)ReceiveTypeEnum.NotDefined).Name = " ";

            ReceiveTypes = typeof(ReceiveTypeEnum).ToComboItemList();
            TransferTypes = typeof(TransferTypeEnum).ToComboItemList();
            CorrectionTypes = typeof(CorrectionTypeEnum).ToComboItemList();
        }

        private void setEntity(FuelReportDetailDto dto)
        {
            this.Entity = dto;
            this.Entity.PropertyChanged += Entity_PropertyChanged;

            CheckEnable();

            setTransferReferences();
        }

        void Entity_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CheckEnable();

            if (e.PropertyName == this.Entity.GetPropertyName(p => p.TransferType))
            {
                this.Entity.FuelReportTransferReferenceNoDto = null;
                setTransferReferences();
            }
        }

        private void setTransferReferences()
        {
            this.FuelReportTransferReferenceNoDtos.Clear();

            if (this.Entity.TransferType == TransferTypeEnum.Rejected)
            {
                if (this.Entity.RejectedTransferReferenceNoDtos != null)
                    this.Entity.RejectedTransferReferenceNoDtos.ForEach(this.FuelReportTransferReferenceNoDtos.Add);
            }
            else
            {
                if (this.Entity.FuelReportTransferReferenceNoDtos != null)
                    this.Entity.FuelReportTransferReferenceNoDtos.ForEach(this.FuelReportTransferReferenceNoDtos.Add);
            }
        }

        #endregion
    }
}

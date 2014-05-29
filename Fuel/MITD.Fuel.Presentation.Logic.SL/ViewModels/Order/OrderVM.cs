#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.Extensions;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.Infrastructure;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;

#endregion

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class OrderVM : WorkspaceViewModel
    {
        #region props

        private List<VesselDto> _fromVessels;
        private ObservableCollection<OrderItemVM> _orderItemVms;
        private List<CompanyDto> _receivers;
        private List<CompanyDto> _suppliers;
        private List<VesselDto> _toVessels;
        private IOrderView _view;
        private CommandViewModel cancelCommand;
        private OrderDto entity;
        private IFuelController mainController;
        private long orderTypeId;
        private List<ComboBoxItm> orderTypes;
        private CommandViewModel saveCommand;
        private IOrderServiceWrapper serviceWrapper;

        private CommandViewModel submitCommand;

        #region column visibility

        private bool _isFromVesselVisible;
        private bool _isReceiverVisible;
        private bool _isSupplierVisible;
        private bool _isToVesselVisible;
        private bool _IsTransporterVisible;
        private List<VesselDto> allVessels;
        private long receiverId;
        private List<CompanyDto> transporters;

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
            get { return _isFromVesselVisible; }
            set { this.SetField(p => p.IsFromVesselVisible, ref _isFromVesselVisible, value); }
        }

        public bool IsToVesselVisible
        {
            get { return _isToVesselVisible; }
            set { this.SetField(p => p.IsToVesselVisible, ref _isToVesselVisible, value); }
        }


        public bool IsTransporterVisible
        {
            get { return _IsTransporterVisible; }
            set { this.SetField(p => p.IsTransporterVisible, ref _IsTransporterVisible, value); }
        }

        #endregion

        public CommandViewModel SubmitCommand
        {
            get { return submitCommand ?? (submitCommand = new CommandViewModel("ذخیره", new DelegateCommand(Save))); }
        }

        public CommandViewModel CancelCommand
        {
            get { return cancelCommand ?? (cancelCommand = new CommandViewModel("انصراف", new DelegateCommand(() => mainController.Close(this)))); }
        }

        public OrderDto Entity
        {
            get { return entity; }
            set { this.SetField(p => p.Entity, ref entity, value); }
        }

        public long OrderTypeId
        {
            get { return orderTypeId; }
            set
            {
                if (OrderTypeChanging((OrderTypeEnum)value, false))
                {
                    Entity.OrderType = (OrderTypeEnum)value;
                    this.SetField(p => p.OrderTypeId, ref orderTypeId, value);
                }


            }
        }

        public long ReceiverId
        {
            get { return receiverId; }
            set
            {
                ChangeReciver(value);
                this.SetField(p => p.ReceiverId, ref receiverId, value);
            }
        }

        private void ChangeReciver(long value)
        {
            Entity.Receiver = Receivers.Single(c => c.Id == value);
        }

        public List<ComboBoxItm> OrderTypes
        {
            get { return orderTypes; }
            set { this.SetField(p => p.OrderTypes, ref orderTypes, value); }
        }


        public List<CompanyDto> Suppliers
        {
            get { return _suppliers; }
            set { this.SetField(p => p.Suppliers, ref _suppliers, value); }
        }
        public List<CompanyDto> Transporters
        {
            get { return transporters; }
            set { this.SetField(p => p.Transporters, ref _suppliers, value); }
        }

        public List<CompanyDto> Receivers
        {
            get { return _receivers; }
            set { this.SetField(p => p.Receivers, ref _receivers, value); }
        }

        public List<VesselDto> FromVessels
        {
            get { return _fromVessels; }
            set { this.SetField(p => p.FromVessels, ref _fromVessels, value); }
        }

        public List<VesselDto> ToVessels
        {
            get { return _toVessels; }
            set { this.SetField(p => p.ToVessels, ref _toVessels, value); }
        }

        public List<VesselDto> AllVessels
        {
            get { return allVessels; }
            set { this.SetField(p => p.AllVessels, ref allVessels, value); }
        }


        public ObservableCollection<OrderItemVM> OrderItemVms
        {
            get { return _orderItemVms; }
            set { this.SetField(vm => vm.OrderItemVms, ref _orderItemVms, value); }
        }

        #endregion

        #region ctor

        public OrderVM()
        {
        }


        public OrderVM(IFuelController mainController, IOrderServiceWrapper serviceWrapper)
        {
            Entity = new OrderDto { Id = -1 };


            this.mainController = mainController;
            this.serviceWrapper = serviceWrapper;

            Suppliers = new List<CompanyDto>();
            Receivers = new List<CompanyDto>();
            Transporters = new List<CompanyDto>();
            DisplayName = "افزودن/اصلاح سفارش ";

            RequestClose += OrderVM_RequestClose;
            this.Entity.PropertyChanged += EntityPropertyChanged;
        }

        void EntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Entity.GetPropertyName(o => o.Receiver) == e.PropertyName)
            {
                if (Entity.OrderType == OrderTypeEnum.Transfer)
                {
                    if (Entity.Receiver != null)
                        ToVessels = UpdateVessels(Entity.Receiver.Id);
                    Entity.ToVessel = new VesselDto();
                }
            }
            //            if (Entity.OrderType == OrderTypeEnum.InternalTransfer)
            //            {
            //                if (Entity.GetPropertyName(o => o.FromVessel) == e.PropertyName)
            //                    Entity.Receiver = Entity.Supplier;
            //            }
            base.OnPropertyChanged(e.PropertyName);
        }

        #endregion

        #region methods

        private void Save()
        {
            if (!entity.Validate())
                return;

            ShowBusyIndicator("در حال ذخیره سازی ");

            if (Entity.Id == -1)
            {

                serviceWrapper.Add
                    (
                        (res, exp) => mainController.BeginInvokeOnDispatcher
                            (
                                () =>
                                {
                                    if (exp != null)
                                    {
                                        mainController.HandleException(exp);
                                    }
                                    else
                                    {
                                        mainController.Publish(new OrderListChangeArg());
                                        Entity = res;
                                        mainController.Close(this);
                                    }
                                }), entity);
            }
            else
            {
                serviceWrapper.Update
                    (
                        (res, exp) => mainController.BeginInvokeOnDispatcher
                            (
                                () =>
                                {
                                    if (exp != null)
                                        mainController.HandleException(exp);
                                    else
                                    {
                                        mainController.Publish(new OrderListChangeArg());
                                        Entity = res;
                                        mainController.Close(this);
                                    }
                                }), entity);
            }
            HideBusyIndicator();
        }

        public void Load(OrderDto ent, List<CompanyDto> dtos, List<VesselDto> vesselDtos)
        {
            SetCollection(dtos, vesselDtos);

            ShowBusyIndicator("در حال دریافت اطلاعات سفارش ...");
            serviceWrapper.GetById
                (
                    (res, exp) => mainController.BeginInvokeOnDispatcher
                        (
                            () =>
                            {
                                if (exp == null)
                                {
                                    Entity = res;
                                    OrderTypeId = (int)res.OrderType;
                                    OrderTypeChanging(res.OrderType, true);
                                    this.Entity.PropertyChanged += EntityPropertyChanged;
                                }
                                else
                                {
                                    mainController.HandleException(exp);
                                }


                                HideBusyIndicator();
                            }), ent.Id);
        }

        public void SetCollection(List<CompanyDto> dtos, List<VesselDto> vesselDtos)
        {
            OrderTypes = (typeof(OrderTypeEnum)).ToComboItemList();
            Suppliers = dtos;
            Receivers = dtos;
            Transporters = dtos;
            FromVessels = vesselDtos;
            ToVessels = vesselDtos;
            AllVessels = vesselDtos;

            Entity.Supplier = new CompanyDto();
            Entity.Receiver = new CompanyDto();
            Entity.Transporter = new CompanyDto();
            Entity.Owner = new CompanyDto();
            Entity.FromVessel = new VesselDto();
            Entity.ToVessel = new VesselDto();
        }


        private bool OrderTypeChanging(OrderTypeEnum orderType, bool init)
        {
            //internal transfer

            if (!init && Entity.Id > 0 && orderType != Entity.OrderType)
                if (mainController.ShowMessage("در صورت تغییر آیتم های سفارش حذف خواهد شد", "اخطار", System.Windows.MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return false;
            switch (orderType)
            {
                case OrderTypeEnum.InternalTransfer:
                    IsSupplierVisible = false;

                    Entity.Supplier = new CompanyDto();
                    Entity.Transporter = new CompanyDto();
                    Entity.Receiver = new CompanyDto();

                    IsFromVesselVisible = true;
                    IsToVesselVisible = true;
                    IsTransporterVisible = true;
                    IsSupplierVisible = false;
                    IsReceiverVisible = false;
                    FromVessels = UpdateVessels(Entity.Owner.Id);
                    ToVessels = UpdateVessels(Entity.Owner.Id);
                    Entity.FromVessel = new VesselDto();
                    Entity.ToVessel = new VesselDto();


                    break;

                case OrderTypeEnum.PurchaseWithTransfer:
                    IsSupplierVisible = true;

                    IsReceiverVisible = false;
                    Entity.Receiver = new CompanyDto();

                    IsFromVesselVisible = false;
                    Entity.FromVessel = new VesselDto();

                    IsToVesselVisible = false;
                    Entity.ToVessel = new VesselDto();

                    IsTransporterVisible = true;
                    break;

                case OrderTypeEnum.Purchase:
                    IsSupplierVisible = true;

                    Entity.Receiver = new CompanyDto();
                    IsReceiverVisible = false;

                    Entity.FromVessel = new VesselDto();
                    IsFromVesselVisible = false;

                    Entity.ToVessel = new VesselDto();
                    IsToVesselVisible = false;
                    Entity.Transporter = new CompanyDto();
                    IsTransporterVisible = false;
                    break;

                case OrderTypeEnum.Transfer:

                    Entity.Supplier = new CompanyDto();
                    IsSupplierVisible = false;
                    IsReceiverVisible = true;
                    IsFromVesselVisible = true;
                    IsToVesselVisible = true;
                    IsTransporterVisible = true;
                    FromVessels = UpdateVessels(Entity.Owner.Id);
                    Entity.FromVessel = new VesselDto();
                    break;
                default:
                    break;
            }
            return true;
        }

        private List<VesselDto> UpdateVessels(long companyId)
        {
            var vessels = AllVessels.Where(c => c.CompanyId == companyId).ToList();
            return vessels;
        }

        private void OrderVM_RequestClose(object sender, EventArgs e)
        {
            mainController.Close(this);
        }

        public void SetServiceWrapper(IOrderServiceWrapper serviceWrapper)
        {
            this.serviceWrapper = serviceWrapper;
        }

        public void SetMainController(IFuelController mainController)
        {
            this.mainController = mainController;
        }

        #endregion

        public void NewOrder(List<CompanyDto> dtos, List<VesselDto> vesselDtos)
        {
            SetCollection(dtos, vesselDtos);
            Entity.Owner = mainController.GetCurrentUser().CompanyDto;

        }
    }
}
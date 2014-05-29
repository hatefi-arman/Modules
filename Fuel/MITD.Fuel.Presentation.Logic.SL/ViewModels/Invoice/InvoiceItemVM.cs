#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Events;
using MITD.Fuel.Presentation.Contracts.SL.Extensions;
using MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;
using EnumHelper = MITD.Fuel.Presentation.Logic.SL.Infrastructure.EnumHelper;

#endregion

namespace MITD.Fuel.Presentation.Logic.SL.ViewModels
{
    public class InvoiceItemVM : WorkspaceViewModel
    {
        #region Prop

        private readonly IFuelController mainController;
        private CommandViewModel cancelCommand;
        private InvoiceItemDto entity;
        private bool enableDivisionPrice;
        private IInvoiceServiceWrapper serviceWrapper;

        private CommandViewModel submitCommand;
        private bool enableQuantity;

        public CommandViewModel SubmitCommand
        {
            get { return submitCommand ?? (submitCommand = new CommandViewModel("ذخیره", new DelegateCommand(Save))); }
        }

        public CommandViewModel CancelCommand
        {
            get { return cancelCommand ?? (cancelCommand = new CommandViewModel("انصراف", new DelegateCommand(() => mainController.Close(this)))); }
        }


        public InvoiceItemDto Entity
        {
            get { return entity; }
            set { this.SetField(p => p.Entity, ref entity, value); }
        }

        public bool EnableDivisionPrice
        {
            get { return enableDivisionPrice; }
            set { this.SetField(p => p.EnableDivisionPrice, ref enableDivisionPrice, value); }
        }

        public bool EnableQuantity
        {
            get { return enableQuantity; }
            set { this.SetField(p => p.EnableQuantity, ref enableQuantity, value); }
        }

        #endregion

        #region ctor

        public InvoiceItemVM()
        {
        }

        public InvoiceItemVM(IFuelController appController, IInvoiceServiceWrapper invoiceServiceWrapper, IGoodServiceWrapper goodServiceWrapper)
        {
            mainController = appController;
            serviceWrapper = invoiceServiceWrapper;
            Entity = new InvoiceItemDto();
            DisplayName = "ویرایش ایتم های صورتحساب ";
        }

        #endregion

        #region Method

        public void SetProp(InvoiceDto invoice)
        {
        }

        #endregion

        #region methods


        private void Save()
        {
            if (!entity.Validate())
                return;

            ShowBusyIndicator("درحال ذخیره سازی");
            mainController.Close(this);
        }

        public void Load(InvoiceItemDto invoiceItem, DivisionMethodEnum divisionMethod, decimal currencyToMainCurrencyRate, InvoiceTypeEnum invoiceType)
        {
            Entity = invoiceItem;
            EnableDivisionPrice = divisionMethod == DivisionMethodEnum.Direct;
            EnableQuantity =invoiceType!= InvoiceTypeEnum.Attach;

            Entity.CurrencyToMainCurrencyRate = currencyToMainCurrencyRate;
        }

        protected override void OnRequestClose()
        {
            base.OnRequestClose();
            mainController.Close(this);
        }

        #endregion
    }

}
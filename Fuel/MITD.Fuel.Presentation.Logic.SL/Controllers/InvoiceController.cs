using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.SL.Controllers;
using MITD.Fuel.Presentation.Contracts.SL.Views;
using MITD.Fuel.Presentation.Logic.SL.ViewModels;
using MITD.Fuel.Presentation.Logic.SL.ViewModels.Invoice;
using MITD.Presentation;
using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Logic.SL.Controllers
{
    public class InvoiceController :BaseController, IInvoiceController
    {
        public InvoiceController(IViewManager viewManager, IEventPublisher eventPublisher, IDeploymentManagement deploymentManagement)
            : base(viewManager, eventPublisher, deploymentManagement)
        {
        }

        public void Add(List<CompanyDto> companiesFilter)
        {
            var view = ViewManager.ShowInDialog<IInvoiceView>();
            (view.ViewModel as InvoiceVM).SetCollection(companiesFilter);
        }

        public void Edit(InvoiceDto dto,List<CompanyDto> companies)
        {
            var view = ViewManager.ShowInDialog<IInvoiceView>();
            (view.ViewModel as InvoiceVM).Load(dto, companies);
        }

        public void ShowList()
        {
            var view = this.ViewManager.ShowInTabControl<IInvoiceListView>();
            (view.ViewModel as InvoiceListVM).Load();
        }

        public void ShowInvoiceReference(CompanyDto selectedCompany, InvoiceDto invoice)
        {
            var view = this.ViewManager.ShowInDialog<IInvoiceReferenceLookUp>();
            (view.ViewModel as InvoiceReferenceLookUpVM).Load(selectedCompany, invoice);
        }

        public void ShowOrderReference(CompanyDto selectedCompany, InvoiceDto invoice)
        {
            var view = this.ViewManager.ShowInDialog<IOrderReferenceLookUp>();
            (view.ViewModel as OrderReferenceLookUpVM).Load(selectedCompany, invoice);
        }

        public void EditItem(InvoiceItemDto invoiceItem, DivisionMethodEnum divistionMethod, decimal currencyToMainCurrencyRate, InvoiceTypeEnum invoiceType)
        {
            var view = ViewManager.ShowInDialog<IInvoiceItemView>();
            (view.ViewModel as InvoiceItemVM).Load(invoiceItem, divistionMethod, currencyToMainCurrencyRate,invoiceType);
        }

        public void EditAdditionalPrice(InvoiceAdditionalPriceDto selectedAdditionalPrice, ObservableCollection<EffectiveFactorDto> effectiveFactors, decimal currencyToMainCurrencyRate)
        {
            var view = ViewManager.ShowInDialog<IInvoiceAdditionalPriceView>();
            (view.ViewModel as InvoiceAdditionalPriceVM).Load(selectedAdditionalPrice,effectiveFactors,currencyToMainCurrencyRate);
        }

        public void ManageAdditionalPrice(InvoiceDto invoice, decimal currencyToMainCurrencyRate, Guid uniqId)
        {
            var view = ViewManager.ShowInDialog<IInvoiceAdditionalPriceListView>();
            (view.ViewModel as InvoiceAdditionalPriceListVM).Load(invoice, currencyToMainCurrencyRate,uniqId);
        }

        public void AddAdditionalPrice(ObservableCollection<EffectiveFactorDto> effectiveFactors, decimal currencyToMainCurrencyRate, Guid uniqId)
        {
            var view = ViewManager.ShowInDialog<IInvoiceAdditionalPriceView>();
            (view.ViewModel as InvoiceAdditionalPriceVM).SetCollection(effectiveFactors, currencyToMainCurrencyRate,uniqId);
        }
    }
}

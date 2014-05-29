using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MITD.Core;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.Commands;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.DomainService;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.Enums;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate.Specifications;
using MITD.Fuel.Domain.Model.DomainServices;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;
using Omu.ValueInjecter;

namespace MITD.Fuel.Application.Facade
{
    public partial class InvoiceFacadeService : IInvoiceFacadeService
    {
        private readonly IInvoiceDomainService invoiceDomainService;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IGoodUnitConvertorDomainService goodUnitConvertorDomainService;
        private readonly IMainUnitVlaueTomainUnitVlaueDtoMapper mainUnitVlaueTomainUnitVlaueDtoMapper;
        private readonly IInvoiceItemDomainService invoiceItemDomainService;
        private readonly IEffectiveFactorMapper effectiveFactorMapper;
        private readonly IInvoiceAdditionalPriceDomainService invoiceAdditionalPriceDomainService;
        private readonly IBalanceDomainService balanceDomainService;

        #region props

        private readonly IInvoiceApplicationService invoiceAppService;
        private readonly IInvoiceToDtoMapper invoiceDtoMapper;
        private readonly IInvoiceItemToDtoMapper itemToDtoMapper;

        #endregion

        #region ctor

        //public InvoiceFacadeService()
        //{
        //    try
        //    {
        //        ServiceLocator.Current.GetInstance<IInvoiceDomainService>();
        //        ServiceLocator.Current.GetInstance<IInvoiceApplicationService>();
        //        ServiceLocator.Current.GetInstance<IInvoiceToDtoMapper>();
        //        ServiceLocator.Current.GetInstance<IInvoiceItemToDtoMapper>();
        //        ServiceLocator.Current.GetInstance<IInvoiceRepository>();
        //        ServiceLocator.Current.GetInstance<IGoodUnitConvertorDomainService>();
        //        ServiceLocator.Current.GetInstance<IMainUnitVlaueTomainUnitVlaueDtoMapper>();
        //        ServiceLocator.Current.GetInstance<IUnitOfWorkScope>();
        //        ServiceLocator.Current.GetInstance<IInvoiceItemDomainService>();
        //        ServiceLocator.Current.GetInstance<IEffectiveFactorMapper>();
        //        ServiceLocator.Current.GetInstance<IInvoiceAdditionalPriceDomainService>();

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
        public InvoiceFacadeService(IInvoiceDomainService invoiceDomainService,
            IInvoiceApplicationService invoiceAppService,
            IInvoiceToDtoMapper invoiceDtoMapper,
            IInvoiceItemToDtoMapper itemToDtoMapper,
            IInvoiceRepository invoiceRepository,
            IGoodUnitConvertorDomainService goodUnitConvertorDomainService,
            IMainUnitVlaueTomainUnitVlaueDtoMapper mainUnitVlaueTomainUnitVlaueDtoMapper,
            IInvoiceItemDomainService invoiceItemDomainService,
            IEffectiveFactorMapper effectiveFactorMapper,
            IInvoiceAdditionalPriceDomainService invoiceAdditionalPriceDomainService,
            IBalanceDomainService balanceDomainService
            )
        {
            this.invoiceDomainService = invoiceDomainService;
            this.invoiceRepository = invoiceRepository;
            this.goodUnitConvertorDomainService = goodUnitConvertorDomainService;
            this.mainUnitVlaueTomainUnitVlaueDtoMapper = mainUnitVlaueTomainUnitVlaueDtoMapper;
            this.invoiceItemDomainService = invoiceItemDomainService;

            this.effectiveFactorMapper = effectiveFactorMapper;
            this.invoiceAdditionalPriceDomainService = invoiceAdditionalPriceDomainService;
            this.balanceDomainService = balanceDomainService;
            this.invoiceAppService = invoiceAppService;
            this.invoiceDtoMapper = invoiceDtoMapper;
            this.itemToDtoMapper = itemToDtoMapper;
        }

        #endregion

        #region methods

        public InvoiceDto Add(InvoiceDto data)
        {
            var invoice = invoiceDtoMapper.MapModelToCommandWithAllIncludes(data);
            var added = invoiceAppService.Add(invoice);
            //                (
            //                    data.InvoiceNumber, data.OwnerId, data.InvoiceDate, (DivisionMethods) data.DivisionMethodId, (AccountingTypes) data.AccountTypeId,
            //                    data.InvoiceRefrence==null?(long?) null:data.InvoiceRefrence.Id, data.OrderRefrences.Select(c => c.Id).ToList(), data.CurrencyId,
            //                    data.TransporterId, data.SupplierId, (InvoiceTypes) (int) data.InvoiceType, data.Description,data.InvoiceItems.ToList());

            var result = invoiceDtoMapper.MapToModel(added);

            return result;
        }



        //TODO Sholde Check Type
        public InvoiceDto Update(InvoiceDto data)
        {
            var invoice = invoiceDtoMapper.MapModelToCommandWithAllIncludes(data);
            var updatedEnt = invoiceAppService.Update(invoice);
            var result = invoiceDtoMapper.MapToModel(updatedEnt);
            return result;
        }
        public InvoiceDto CalculateAdditionalPrice(InvoiceDto data)
        {
            var invoice = invoiceDtoMapper.MapModelToCommandWithAllIncludes(data);
            var updatedEnt = invoiceAppService.CalculateAdditionalPrice(invoice);
            var result = invoiceDtoMapper.MapToModelWithAllIncludes(updatedEnt);
            return result;
        }

        public void Delete(InvoiceDto data)
        {
            invoiceAppService.DeleteById(data.Id);
        }

        public InvoiceDto GetById(long id)
        {


            var fetch = new SingleResultFetchStrategy<Invoice>()
                .Include(o => o.InvoiceItems)
                .Include(o => o.Supplier)
                .Include(o => o.Transporter)
                //.Include(o => o.Owner)
                .Include(o => o.InvoiceRefrence)
                .Include(o => o.OrderRefrences)
                .Include(o => o.ApproveWorkFlows)
                .Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep)
                .Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep.ActorUser)
                .Include(c => c.OrderRefrences.Select(d => d.OrderItems))
                .Include(c => c.InvoiceItems.Select(d => d.Good))
                .Include(c => c.AdditionalPrices)
                .Include(c => c.AdditionalPrices.Select(d => d.EffectiveFactor))
                .Include(c => c.InvoiceItems.Select(d => d.Good.GoodUnits));


            var invoicesPageResult = invoiceRepository.Single(o => o.Id == id, fetch);



            var invoiceDtos = invoiceDtoMapper.MapToModelWithAllIncludes(invoicesPageResult);

            return invoiceDtos;

            //var ent = Data.FirstOrDefault(e => e.Id == id);
            //return ent;
        }

        // todo: input parameters must be converted to IPageCritera
        public PageResultDto<InvoiceDto> GetAll(int pageSize, int pageIndex)
        {
            var fetch = new ListFetchStrategy<Invoice>().WithPaging(pageSize, pageIndex);
            invoiceRepository.GetAll(fetch);

            var finalResult = new PageResultDto<InvoiceDto>
                                  {
                                      CurrentPage = pageIndex,
                                      PageSize = pageSize,
                                      Result = invoiceDtoMapper.MapToModel(fetch.PageCriteria.PageResult.Result.ToList()).ToList(),
                                      TotalCount = fetch.PageCriteria.PageResult.TotalCount,
                                      TotalPages = fetch.PageCriteria.PageResult.TotalPages
                                  };

            return finalResult;
        }

        public void DeleteById(int id)
        {
            invoiceAppService.DeleteById(id);
        }


        public PageResultDto<InvoiceDto> GetByFilter(int companyId, DateTime fromDate, DateTime toDate, string invoiceNumber, int pageSize, int pageIndex, bool submitedState)
        {
            var toDateParam = toDate.Date.AddDays(1);

            var fetch = new ListFetchStrategy<Invoice>()
               .Include(o => o.InvoiceItems)
                .Include(o => o.Supplier)
                .Include(o => o.Transporter)
                //.Include(o => o.Owner)
                //                .Include(o => o.InvoiceRefrence)
                //                .Include(o => o.OrderRefrences)
                .Include(o => o.ApproveWorkFlows)
                .Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep)
                .Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep.ActorUser)
                //                .Include(c => c.OrderRefrences.Select(d => d.OrderItems))
                .Include(c => c.InvoiceItems.Select(d => d.Good))
                //                .Include(c => c.AdditionalPrices)
                //                .Include(c => c.AdditionalPrices.Select(d=>d.EffectiveFactor))
                .Include(c => c.InvoiceItems.Select(d => d.Good.GoodUnits))
                .WithPaging(pageSize, pageIndex + 1);

            // var invoiceType = _invoiceDtoMapper.MapInvoiceTypeDtoToInvoiceTypeEntity(invoiceTypeDto);
            invoiceRepository.Find(o =>

                (string.IsNullOrEmpty(invoiceNumber) || o.InvoiceNumber.Contains(invoiceNumber)) &&
                (!submitedState || o.State == States.Submitted) &&
               (companyId == -1 || o.OwnerId == companyId)
                && (fromDate == DateTime.MinValue || o.InvoiceDate >= fromDate.Date)
                                                  && (toDate == DateTime.MinValue || o.InvoiceDate <= toDateParam)
                                  , fetch);

            //  _invoiceRepository.Find(o => o.InvoiceType == invoiceType, fetch);


            var invoicesPageResult = fetch.PageCriteria.PageResult;


            //            var d = _invoiceRepository.GetAll();

            //            var invoiceDtos = _invoiceDtoMapper.MapToModel(d);

            var result = new PageResultDto<InvoiceDto>
                             {
                                 CurrentPage = invoicesPageResult.CurrentPage,
                                 PageSize = invoicesPageResult.PageSize,
                                 Result = invoiceDtoMapper.MapToModelWithAllIncludes(invoicesPageResult.Result).ToList(),
                                 TotalCount = invoicesPageResult.TotalCount,
                                 TotalPages = invoicesPageResult.TotalPages,
                             };
            return result;
        }

        #endregion

        #region InvoiceItem

        //        public InvoiceItemDto UpdateItem(InvoiceItemDto data)
        //        {
        //
        //            return _itemToDtoMapper.MapEntityToDto(
        //                _invoiceAppService.UpdateItem(
        //                data.Id, data.InvoiceId, data.Fee, data.Quantity, data.Description));
        //
        //
        //
        //        }
        //
        //        public void DeleteItem(InvoiceItemDto data)
        //        {
        //            this._invoiceAppService.DeleteItem(data.InvoiceId, data.Id);
        //        }

        public IEnumerable<InvoiceItemDto> GenerateInvoiceItemForOrders(string strOrderList)
        {
            var orderList = strOrderList.Split(',').ToList().Select(long.Parse).ToList();
            var invoiceItemList = balanceDomainService.GenerateInvoiceItemFromOrders(orderList);
            return itemToDtoMapper.MapEntityToDto(invoiceItemList);
        }

        public IEnumerable<EffectiveFactorDto> GetAllEffectiveFactors()
        {
            var factors = invoiceRepository.GetAllEffectiveFactors();
            return this.effectiveFactorMapper.MapToModel(factors);
        }


        public InvoiceItemDto GetInvoiceItemById(long invoiceId, long invoiceItemId)
        {
            var invoice = this.invoiceRepository.FindByKey(invoiceId);
            var invoiceItem = invoice.InvoiceItems.SingleOrDefault(c => c.Id == invoiceItemId);
            return itemToDtoMapper.MapEntityToDto(invoiceItem);
        }

        public MainUnitValueDto GetGoodMainUnit(long goodId, long goodUnitId, decimal value)
        {
            return mainUnitVlaueTomainUnitVlaueDtoMapper.MapToModel(goodUnitConvertorDomainService.GetUnitValueInMainUnit(goodId, goodUnitId, value));
        }

        #endregion
    }

}
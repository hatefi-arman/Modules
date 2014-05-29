using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Domain.Repository;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade
{
    public partial class OrderFacadeService : IOrderFacadeService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IGoodUnitConvertorDomainService _goodUnitConvertorDomainService;
        private readonly IMainUnitVlaueTomainUnitVlaueDtoMapper _mainUnitVlaueTomainUnitVlaueDtoMapper;
        private readonly IUnitOfWorkScope _unitOfWorkScope;

        #region props
        private List<OrderDto> Data;
        private readonly IOrderApplicationService _orderAppService;
        private readonly ICompanyDomainService _companyDomainService;
        private readonly IOrderToDtoMapper _orderDtoMapper;
        private readonly IOrderItemToDtoMapper _itemToDtoMapper;

        #endregion

        #region ctor

        public OrderFacadeService(IOrderApplicationService orderAppService,
                                  ICompanyDomainService companyDomainService,
                                  IOrderToDtoMapper orderDtoMapper,
                                  IOrderItemToDtoMapper itemToDtoMapper,
                                  IOrderRepository orderRepository,
            IGoodUnitConvertorDomainService goodUnitConvertorDomainService,
            IMainUnitVlaueTomainUnitVlaueDtoMapper mainUnitVlaueTomainUnitVlaueDtoMapper
            , IUnitOfWorkScope unitOfWorkScope
)
        {
            _orderRepository = orderRepository;
            _goodUnitConvertorDomainService = goodUnitConvertorDomainService;
            _mainUnitVlaueTomainUnitVlaueDtoMapper = mainUnitVlaueTomainUnitVlaueDtoMapper;
            _unitOfWorkScope = unitOfWorkScope;
            _orderAppService = orderAppService;
            _companyDomainService = companyDomainService;
            _orderDtoMapper = orderDtoMapper;
            _itemToDtoMapper = itemToDtoMapper;
        }

        #endregion

        #region methods

        public OrderDto Add(OrderDto data)
        {
            var updatedEnt = _orderAppService.Add(data.Description,
                                                 data.Owner.Id,
                                                  data.Transporter != null && data.Transporter.Id != 0 ? data.Transporter.Id : (long?)null,
                                                 data.Supplier != null && data.Supplier.Id != 0 ? data.Supplier.Id : (long?)null,
                                                 data.Receiver != null && data.Receiver.Id != 0 ? data.Receiver.Id : (long?)null,
                                                 (OrderTypes)(int)data.OrderType,
                                                 data.FromVessel != null && data.FromVessel.Id != 0 ? data.FromVessel.Id : (long?)null,
                                                 data.ToVessel != null && data.ToVessel.Id != 0 ? data.ToVessel.Id : (long?)null);

            var result = _orderDtoMapper.MapToModel(updatedEnt);

            return result;
        }



        //TODO Sholde Check Type
        public OrderDto Update(OrderDto data)
        {
            var updatedEnt = _orderAppService.Update(data.Id, data.Description,
                                                     (OrderTypes)(int)data.OrderType,
                                                     data.Owner.Id,
                                                     data.Transporter != null && data.Transporter.Id != 0 ? data.Transporter.Id : (long?)null,
                                                     data.Supplier != null && data.Supplier.Id != 0 ? data.Supplier.Id : (long?)null,
                                                     data.Receiver != null && data.Receiver.Id != 0 ? data.Receiver.Id : (long?)null,
                                                     data.FromVessel != null && data.FromVessel.Id != 0 ? data.FromVessel.Id : (long?)null,
                                                     data.ToVessel != null && data.ToVessel.Id != 0 ? data.ToVessel.Id : (long?)null);

            var result = _orderDtoMapper.MapToModel(updatedEnt);

            return result;
        }

        public void Delete(OrderDto data)
        {
            _orderAppService.DeleteById(data.Id);
        }

        public OrderDto GetById(long id)
        {


            var fetch = new SingleResultFetchStrategy<Order>()
                .Include(o => o.FromVesselInCompany)
                .Include(o => o.ToVesselInCompany)
                .Include(o => o.OrderItems)
                .Include(o => o.Supplier)
                .Include(o => o.Receiver)
                .Include(o => o.Transporter)
                .Include(o => o.Owner)
                .Include(o => o.ApproveWorkFlows)
                .Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep)
                .Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep.ActorUser)
                .Include(c => c.OrderItems.Select(d => d.Good))
                .Include(c => c.OrderItems.Select(d => d.Good.GoodUnits));

            _unitOfWorkScope.LazyLoadingEnabled = false;
            _unitOfWorkScope.ProxyCreationEnabled = false;

            var ordersPageResult = _orderRepository.Single(o => o.Id == id, fetch);



            var orderDtos = _orderDtoMapper.MapToModelWithAllIncludes(ordersPageResult);

            return orderDtos;

            //var ent = Data.FirstOrDefault(e => e.Id == id);
            //return ent;
        }

        // todo: input parameters must be converted to IPageCritera
        public PageResultDto<OrderDto> GetAll(int pageSize, int pageIndex)
        {
            var fetch = new ListFetchStrategy<Order>().WithPaging(pageSize, pageIndex);
            _orderRepository.GetAll(fetch);

            var finalResult = new PageResultDto<OrderDto>
                                  {
                                      CurrentPage = pageIndex,
                                      PageSize = pageSize,
                                      Result = _orderDtoMapper.MapToModel(fetch.PageCriteria.PageResult.Result.ToList()).ToList(),
                                      TotalCount = fetch.PageCriteria.PageResult.TotalCount,
                                      TotalPages = fetch.PageCriteria.PageResult.TotalPages
                                  };

            return finalResult;
        }

        public void DeleteById(int id)
        {
            _orderAppService.DeleteById(id);
        }


        public PageResultDto<OrderDto> GetByFilter(int companyId, int orderCreatorId, string orderTypesString, DateTime fromDate, DateTime toDate, int pageSize, int pageIndex, long? supplierId, long? transporterId, bool includeOrderItem, string orderIdListString, string orderCode, bool submitedState)
        {

            _unitOfWorkScope.LazyLoadingEnabled = false;
            _unitOfWorkScope.ProxyCreationEnabled = false;
            var toDateParam = toDate.Date.AddDays(1);
            var fetch =
                new ListFetchStrategy<Order>().Include(o => o.FromVesselInCompany).Include(o => o.ToVesselInCompany).Include(o => o.Supplier).Include(o => o.Receiver).
                    Include(o => o.Transporter).Include(o => o.Owner).Include(o => o.ApproveWorkFlows).Include
                    (o => o.ApproveWorkFlows.Last().CurrentWorkflowStep).Include(o => o.ApproveWorkFlows.Last().CurrentWorkflowStep.ActorUser).
                    WithPaging(pageSize, pageIndex + 1);


            if (includeOrderItem)
                fetch.Include(c => c.OrderItems);
            
                
            var orderTypes = new List<OrderTypes>();
            if (!string.IsNullOrEmpty(orderTypesString))
            {
                var orderTypeDtos = orderTypesString.Split(',');
                orderTypes.AddRange(orderTypeDtos.Where(c => c != "0").Select(item => (OrderTypes) int.Parse(item)));
            }
            var orderlist = new List<long>();
            if (!string.IsNullOrEmpty(orderIdListString))
            {
                var orderArray = orderIdListString.Split(',');
                orderlist.AddRange(orderArray.Where(c => c != "0").Select(long.Parse));
                _orderRepository.Find(o => orderlist.Count == 0 || orderlist.Contains(o.Id), fetch);
            }
            else
            {
                _orderRepository.Find
                    (
                        o =>
                            (orderTypes.Count == 0 || orderTypes.Contains(o.OrderType)) 
                                
                                && (companyId == -1 || o.OwnerId == companyId)
                                && (!submitedState || o.State == States.Submitted)
                                && (fromDate == DateTime.MinValue || o.OrderDate >= fromDate.Date)
                                && (toDate == DateTime.MinValue || o.OrderDate <= toDateParam)
                                && (supplierId == 0 || supplierId == null || o.SupplierId == supplierId)
                                && (transporterId == 0 || transporterId == null || o.TransporterId == transporterId)
                                && (string.IsNullOrEmpty(orderCode) || o.Code == orderCode)
                                , fetch);

            }
            var ordersPageResult = fetch.PageCriteria.PageResult;


            var result = new PageResultDto<OrderDto>
                             {
                                 CurrentPage = ordersPageResult.CurrentPage,
                                 PageSize = ordersPageResult.PageSize,
                                 Result = _orderDtoMapper.MapToModelWithAllIncludes(ordersPageResult.Result).ToList(),
                                 TotalCount = ordersPageResult.TotalCount,
                                 TotalPages = ordersPageResult.TotalPages,
                             };
            return result;
        }

        #endregion

        #region OrderItem
        public OrderItemDto AddItem(OrderItemDto data)
        {
            return _itemToDtoMapper.MapEntityToDto(
                _orderAppService.AddItem(data.OrderId, data.Description, data.Quantity, data.Good.Id, data.Good.Unit.Id, data.AssigneBuessinessPartyForGoodId));
        }

        public OrderItemDto UpdateItem(OrderItemDto data)
        {

            return _itemToDtoMapper.MapEntityToDto(
                _orderAppService.UpdateItem(
                data.Id, data.OrderId, data.Description, data.Quantity, data.Good.Id, data.Good.Unit.Id, 0));



        }

        public void DeleteItem(OrderItemDto data)
        {
            this._orderAppService.DeleteItem(data.OrderId, data.Id);
        }

        public OrderItemDto GetOrderItemById(long orderId, long orderItemId)
        {
            var order = this._orderRepository.FindByKey(orderId);
            var orderItem = order.OrderItems.SingleOrDefault(c => c.Id == orderItemId);
            return _itemToDtoMapper.MapEntityToDto(orderItem);
        }

        public MainUnitValueDto GetGoodMainUnit(long goodId, long goodUnitId, decimal value)
        {
            return _mainUnitVlaueTomainUnitVlaueDtoMapper.MapToModel(_goodUnitConvertorDomainService.GetUnitValueInMainUnit(goodId, goodUnitId, value));
        }

        #endregion
    }


}
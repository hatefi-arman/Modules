using System;
using System.Collections.Generic;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

namespace MITD.Fuel.Presentation.Contracts.FacadeServices
{
    public interface IOrderFacadeService : IFacadeService
    {
        PageResultDto<OrderDto> GetByFilter(int companyId, int orderCreatorId, string orderType, DateTime fromDate, DateTime toDate, int pageSize, int pageIndex, long? supplierId, long? transporterId, bool includeOrderItem, string orderIdList, string orderCode,bool submitedState);

        OrderDto Add(OrderDto data);
        OrderDto Update(OrderDto data);
        void Delete(OrderDto data);
        OrderDto GetById(long id);
        PageResultDto<OrderDto> GetAll(int pageSize, int pageIndex);
        void DeleteById(int id);

        OrderItemDto AddItem(OrderItemDto data);
        OrderItemDto UpdateItem(OrderItemDto data);
        void DeleteItem(OrderItemDto data);


        OrderItemDto GetOrderItemById(long orderId, long orderItemId);
        MainUnitValueDto GetGoodMainUnit(long goodId, long goodUnitId, decimal value);
    }
}

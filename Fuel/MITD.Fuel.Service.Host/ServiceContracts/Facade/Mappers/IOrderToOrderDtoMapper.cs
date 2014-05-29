using System.Collections.Generic;
using MITD.Core;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Services.Facade;
using Omu.ValueInjecter;

namespace MITD.Fuel.Service.Host.ServiceContracts.Facade.Mappers
{
    public interface IOrderToOrderDtoMapper : IFacadeMapper<Order,OrderDto>
    {
        new IEnumerable<OrderDto> MapToModel(IEnumerable<Order> entities);
        OrderTypes MapOrderTypeDtoToOrderTypeEntity(OrderTypeEnum orderTypeEnum);
        OrderTypeEnum MapOrderTypeEntityToOrderTypeDto(OrderTypes orderTypes);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade.Mappers
{
    public class OrderItemToDtoMapper : BaseFacadeMapper<OrderItem, OrderItemDto>, IOrderItemToDtoMapper
    {
        private readonly IGoodToGoodDtoMapper _goodMapper;

        public OrderItemToDtoMapper(IGoodToGoodDtoMapper goodMapper)
        {
            _goodMapper = goodMapper;
        }

        public OrderItemDto MapEntityToDto(OrderItem orderItem)
        {

            var dto = new OrderItemDto
                          {
                              Id = orderItem.Id,
                              Description = orderItem.Description,
                              Quantity = orderItem.Quantity, //.HasValue ? entity.Quantity.Value : 0,                            
             
                              OrderId = orderItem.OrderId
                          };


            if (orderItem.Good == null)
                return dto;
            GoodDto goodDto = _goodMapper.MapEntityToDtoWithUnits(orderItem.Good);

            goodDto.Unit = new GoodUnitDto {Id = orderItem.MeasuringUnit.Id, Name = orderItem.MeasuringUnit.Name};
            dto.Good = goodDto;
            return dto;
        }


        public IEnumerable<OrderItemDto> MapEntityToDto(IEnumerable<OrderItem> entities)
        {
            return entities.Select(MapEntityToDto);
        }
    }
}
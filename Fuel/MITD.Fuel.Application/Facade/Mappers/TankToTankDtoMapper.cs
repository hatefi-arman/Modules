using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade.Mappers
{
    public class TankToTankDtoMapper : BaseFacadeMapper<Tank, TankDto>, ITankToTankDtoMapper
    {
        private readonly IVesselInInventoryToVesselDtoMapper vesselMapper;

        public TankToTankDtoMapper(IVesselInInventoryToVesselDtoMapper  vesselMapper)
        {
            this.vesselMapper = vesselMapper;
        }

        public override TankDto MapToModel(Tank entity)
        {
            var dto = base.MapToModel(entity);
            dto.Code = entity.Name;
            dto.VesselDto = 
                
                vesselMapper.MapToModel(entity.VesselInInventory);

            return dto;
        }


    }
}

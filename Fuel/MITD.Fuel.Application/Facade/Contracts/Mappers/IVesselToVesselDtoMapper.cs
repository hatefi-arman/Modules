﻿using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Services.Facade;

namespace MITD.Fuel.Application.Facade.Contracts.Mappers
{
    public interface IVesselToVesselDtoMapper : IFacadeMapper<VesselInCompany, VesselDto>
    {
        
    }

    public interface IVesselInInventoryToVesselDtoMapper : IFacadeMapper<VesselInInventory, VesselDto>
    {

    }
}
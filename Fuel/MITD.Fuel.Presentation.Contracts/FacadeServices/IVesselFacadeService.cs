using System.Collections.Generic;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

namespace MITD.Fuel.Presentation.Contracts.FacadeServices
{
    public interface IVesselFacadeService : IFacadeService
    {
        List<VesselDto> GetAll();
        List<VesselDto> GetOwnedVessels(long companyId);
        List<VesselDto> GetOwnedOrCharterInVessels(long companyId);
        VesselDto GetById(long id);
        List<VesselDto> GetCompanyVessels(long enterpriseId);

    }
}

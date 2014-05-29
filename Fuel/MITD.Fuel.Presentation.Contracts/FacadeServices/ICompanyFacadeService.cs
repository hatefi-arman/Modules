using System.Collections.Generic;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

namespace MITD.Fuel.Presentation.Contracts.FacadeServices
{
    public interface ICompanyFacadeService : IFacadeService
    {

        List<CompanyDto> GetAll();
        //bool CanBePoGood(int goodId, int companyId);
        PageResultDto<VesselDto> GetOwnedVessels(long companyId);
    }
}

using System.Collections.Generic;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Services.Application;

namespace MITD.Fuel.Application.Service.Contracts
{
    public interface IVoyageApplicationService: IApplicationService
    {
        List<Voyage> GetAll();
        List<Voyage> Get(List<long> IDs);
        List<Voyage> GetByFilter(long companyId, long vesselId);
    }
}

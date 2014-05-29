#region

using System;
using System.Collections.Generic;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;

#endregion

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface IVesselInCompanyDomainService : IDomainService<VesselInCompany>
    {
        List<Voyage> Find(long companyId, long vesselInCompanyId);
        List<VesselInCompany> Get(List<long> idList);
        List<VesselInCompany> GetCompanyVessels(long companyId);
        VesselInCompany GetVesselInCompany(long companyId, string vesselCode);
        List<VesselInCompany> GetOwnedOrCharterInVessels(long companyId);
        List<VesselInCompany> GetInactiveVessels(long companyId);
        List<VesselInCompany> GetOwnedVessels(long companyId);
        VesselInCompany GetWithTanks(long id);
        //void ChangeVesselStateBackToOwner();

        VesselStates GetVesselCurrentState(long id);
        List<VesselStates> GetVesselStatesLog(long id, DateTime? startDate, DateTime? toDate);

        void ScrapVessel(long vesselInCompanyId);

        void ActivateVessel(long vesselInCompanyId);

        void DeactivateVessel(long vesselInCompanyId);

        void StartCharterInVessel(long vesselInCompanyId);

        void EndCharterInVessel(long vesselInCompanyId);

        void StartCharterOutVessel(long vesselInCompanyId);

        void EndCharterOutVessel(long vesselInCompanyId);
    }
}
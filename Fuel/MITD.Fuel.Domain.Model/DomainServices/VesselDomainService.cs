using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Domain.Model.DomainServices
{
    public class VesselInCompanyDomainService : IVesselInCompanyDomainService
    {
        private readonly IVesselInCompanyRepository vesselInCompanyRepository;

        private readonly IVoyageRepository voyageRepository;

        public VesselInCompanyDomainService(
            IVesselInCompanyRepository vesselInCompanyRepository,
            IVoyageRepository voyageRepository)
        {
            this.vesselInCompanyRepository = vesselInCompanyRepository;
            this.voyageRepository = voyageRepository;
        }

        public List<VesselInCompany> Get(List<long> IDs)
        {
            return this.vesselInCompanyRepository.Find(c => IDs.Contains(c.Id)).ToList();
        }

        public List<VesselInCompany> GetCompanyVessels(long companyId)
        {
            return this.vesselInCompanyRepository.Find(v => v.CompanyId == companyId).ToList();
        }

        public VesselInCompany GetVesselInCompany(long companyId, string vesselCode)
        {
            return this.vesselInCompanyRepository.Single(v => v.CompanyId == companyId && v.Vessel.Code == vesselCode);
        }


        public List<VesselInCompany> GetAll()
        {
            return this.vesselInCompanyRepository.GetAll().ToList();
        }

        public VesselInCompany Get(long id)
        {
            return this.vesselInCompanyRepository.Find(c => c.Id == id).FirstOrDefault();
        }

        public VesselInCompany GetWithTanks(long id)
        {
            return this.vesselInCompanyRepository.Find(c => c.Id == id).FirstOrDefault();
        }

        //public bool CompanyHaveValidVessel(long? vesselInCompanyId, long companyId)
        //{
        //    return vesselFakeRepository.Count(v => v.Id == vesselInCompanyId.Value && v.CompanyId == companyId) == 1;
        //}

        //public void IsValid(long value)
        //{
        //    vesselFakeRepository.Find(v => v.Id == value);
        //}

        public List<Voyage> Find(long companyId, long vesselInCompanyId)
        {
            return this.voyageRepository.Find(voy => voy.CompanyId == companyId && voy.VesselInCompanyId == vesselInCompanyId).ToList();
        }


        public List<VesselInCompany> GetInactiveVessels(long companyId)
        {
            return this.vesselInCompanyRepository.Find(v => v.CompanyId == companyId && v.VesselStateCode == VesselStates.Inactive).ToList();
        }

        public List<VesselInCompany> GetOwnedOrCharterInVessels(long companyId)
        {
            return this.vesselInCompanyRepository.Find(v => v.CompanyId == companyId &&
                                                 (v.VesselStateCode == VesselStates.CharterIn
                                                  || v.VesselStateCode == VesselStates.Owned)).ToList();
        }

        public List<VesselInCompany> GetOwnedVessels(long companyId)
        {
            return this.vesselInCompanyRepository.Find(v => v.CompanyId == companyId && v.VesselStateCode == VesselStates.Owned).ToList();
        }

        public VesselStates GetVesselCurrentState(long id)
        {
            return this.vesselInCompanyRepository.First(v => v.Id == id).VesselStateCode;
        }

        public List<VesselStates> GetVesselStatesLog(long id, DateTime? fromDate, DateTime? toDate)
        {
            return new List<VesselStates>() { VesselStates.Owned };
        }

        public void ScrapVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.Scrap();
        }


        public void ActivateVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.Activate();
        }

        public void DeactivateVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.Deactivate();
        }

        public void StartCharterInVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.StartCharterIn();
        }

        public void EndCharterInVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.EndCharterIn();
        }

        public void StartCharterOutVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.StartCharterOut();
        }

        public void EndCharterOutVessel(long vesselInCompanyId)
        {
            var vesselInCompany = this.vesselInCompanyRepository.First(v => v.Id == vesselInCompanyId);
            vesselInCompany.EndCharterOut();
        }
    }
}

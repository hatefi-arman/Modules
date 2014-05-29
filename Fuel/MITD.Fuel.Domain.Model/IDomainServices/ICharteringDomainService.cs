using System;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface ICharteringDomainService : IDomainService
    {
        CharterOut GetCharterOutStart(Company company, VesselInCompany vesselInCompany, DateTime date);
        CharterIn GetCharterInStart(Company company, VesselInCompany vesselInCompany, DateTime date);
        CharterOut GetCharterOutEnd(Company company, VesselInCompany vesselInCompany, DateTime date);

        //OffHirePricingType GetPricingType(Company company, Vessel vessel, CharteringPartyType partyType, DateTime date);
        //string GetCharteringReferenceNumber(Company company, Vessel vessel, CharteringPartyType partyType, DateTime date);
    }
}
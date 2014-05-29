using System;
using System.Linq;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.Fuel.Domain.Model.DomainServices.CharterAggregate
{
    public class CharteringDomainService : ICharteringDomainService
    {
        private ICharterInRepository charterInRepository;
        private ICharterOutRepository charterOutRepository;

        public CharteringDomainService(ICharterInRepository charterInRepository, ICharterOutRepository charterOutRepository)
        {
            this.charterInRepository = charterInRepository;
            this.charterOutRepository = charterOutRepository;
        }



        public CharterOut GetCharterOutStart(Company company, VesselInCompany vesselInCompany, DateTime date)
        {
            IListFetchStrategy<Charter> fetchStrategy = new ListFetchStrategy<Charter>()
                .Include(c => c.CharterItems).OrderBy(c => c.ActionDate);

            var foundCharterOut = charterOutRepository
                .Find(cho => cho.CharterType == CharterType.Start && date >= cho.ActionDate &&
                    cho.OwnerId == company.Id && cho.VesselInCompanyId == vesselInCompany.Id, fetchStrategy).LastOrDefault() as CharterOut;

            return foundCharterOut;
        }

        public CharterIn GetCharterInStart(Company company, VesselInCompany vesselInCompany, DateTime date)
        {
            IListFetchStrategy<Charter> fetchStrategy = new ListFetchStrategy<Charter>()
                .Include(c => c.CharterItems).OrderBy(c => c.ActionDate);

            var foundCharterIn = charterInRepository
                .Find(chi => chi.CharterType == CharterType.Start && date >= chi.ActionDate &&
                    chi.ChartererId == company.Id && chi.VesselInCompanyId == vesselInCompany.Id, fetchStrategy).LastOrDefault() as CharterIn;

            return foundCharterIn;
        }

        //Added by Hatefi
        public CharterOut GetCharterOutEnd(Company company, VesselInCompany vesselInCompany, DateTime date)
        {
            IListFetchStrategy<Charter> fetchStrategy = new ListFetchStrategy<Charter>()
                .Include(c => c.CharterItems).OrderBy(c => c.ActionDate);

            var foundCharterOut = charterOutRepository
                .Find(cho => cho.CharterType == CharterType.End && date >= cho.ActionDate &&
                    cho.OwnerId == company.Id && cho.VesselInCompanyId == vesselInCompany.Id, fetchStrategy).LastOrDefault() as CharterOut;

            return foundCharterOut;
        }
    }
}
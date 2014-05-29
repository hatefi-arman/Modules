using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Domain.Model;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Domain.Model.Specifications;

namespace MITD.Fuel.ACL.StorageSpace.DomainServices
{
    public class VoyageDomainService : IVoyageDomainService
    {
        private readonly IVoyageRepository voyageRepository;
        private readonly IFuelReportRepository fuelReportRepository;

        public VoyageDomainService(IVoyageRepository voyageRepository, IFuelReportRepository fuelReportRepository)
        {
            this.voyageRepository = voyageRepository;
            this.fuelReportRepository = fuelReportRepository;
        }

        public List<Voyage> Get(List<long> ids)
        {
            return this.voyageRepository.Find(v => v.IsActive).Where(v => ids.Contains(v.Id)).ToList();
        }

        public Voyage Get(long id)
        {
            var fetchStrategy = new SingleResultFetchStrategy<Voyage>();
            fetchStrategy.Include(v => v.VesselInCompany).Include(v => v.Company);

            return this.voyageRepository.First(v => v.Id == id && v.IsActive, fetchStrategy);
        }

        public List<Voyage> GetAll()
        {
            var fetchStrategy = new ListFetchStrategy<Voyage>();
            fetchStrategy.Include(v => v.VesselInCompany).Include(v => v.Company);

            return this.voyageRepository.GetAll(fetchStrategy).ToList();
        }

        public bool IsVoyageAvailable(long voyageId)
        {
            return this.voyageRepository.Find(v => v.Id == voyageId && v.IsActive).Count == 1;
        }


        public List<Voyage> GetVoyagesEndedBefore(DateTime dateTime)
        {
            var fetchStrategy = new ListFetchStrategy<Voyage>();
            fetchStrategy.Include(v => v.VesselInCompany).Include(v => v.Company);

            return this.voyageRepository.Find(v => v.EndDate < dateTime && v.IsActive, fetchStrategy).ToList();
        }

        public List<Voyage> GetByFilter(long companyId, long vesselInCompanyId)
        {
            var fetchStrategy = new ListFetchStrategy<Voyage>();
            fetchStrategy.Include(v => v.VesselInCompany).Include(v => v.Company);

            var result = this.voyageRepository.Find(
                v => v.IsActive && v.CompanyId == companyId && v.VesselInCompanyId == vesselInCompanyId, fetchStrategy).ToList();

            return result;
        }

        public PageResult<Voyage> GetPagedData(int pageSize, int pageIndex)
        {
            var pageNumber = pageIndex + 1;

            var fetchStrategy = new ListFetchStrategy<Voyage>(nolock: true);
            fetchStrategy.Include(v => v.VesselInCompany).Include(v => v.Company);

            fetchStrategy.WithPaging(pageSize, pageNumber);

            this.voyageRepository.Find(v => v.IsActive, fetchStrategy);

            return fetchStrategy.PageCriteria.PageResult;
        }

        public PageResult<Voyage> GetPagedDataByFilter(long companyId, long vesselInCompanyId, int pageSize, int pageIndex)
        {
            var pageNumber = pageIndex + 1;

            var fetchStrategy = new ListFetchStrategy<Voyage>(nolock: true);
            fetchStrategy.Include(v => v.VesselInCompany).Include(v => v.Company);

            fetchStrategy.WithPaging(pageSize, pageNumber);

            this.voyageRepository.Find(v => v.IsActive && v.CompanyId == companyId && v.VesselInCompanyId == vesselInCompanyId, fetchStrategy);

            return fetchStrategy.PageCriteria.PageResult;
        }

        public Voyage GetVoyage(Company company, DateTime date)
        {
            var voyage = this.voyageRepository.First(v => v.VesselInCompany.CompanyId == company.Id && v.StartDate <= date && v.EndDate >= date);

            return voyage;
        }

        public Voyage GetVoyageContainingDuration(Company company, DateTime startDateTime, DateTime endDateTime)
        {
            if (startDateTime > endDateTime)
                throw new InvalidArgument("Invlaid duration.", "EndDateTime");

            var voyage = this.voyageRepository.First(v => v.VesselInCompany.CompanyId == company.Id &&
                v.StartDate <= startDateTime && v.EndDate >= endDateTime);

            return voyage;
        }

        public FuelReport GetVoyageValidEndOfVoyageFuelReport(long voyageId)
        {
            var isFuelReportNotCancelled = new IsFuelReportNotCancelled();

            var endOfVoyageFuelReport = this.fuelReportRepository.First(
                Extensions.And(isFuelReportNotCancelled.Predicate, fr => fr.FuelReportType == FuelReportTypes.EndOfVoyage &&
                        voyageId == fr.VoyageId));

            return endOfVoyageFuelReport;
        }

        //================================================================================
    }
}
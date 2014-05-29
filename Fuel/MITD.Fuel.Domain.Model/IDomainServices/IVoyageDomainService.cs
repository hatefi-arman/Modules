#region

using System;
using System.Collections.Generic;
using MITD.Domain.Model;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;

#endregion

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public interface IVoyageDomainService : IDomainService
    {
        List<Voyage> GetAll();

        bool IsVoyageAvailable(long voyageId);

        List<Voyage> GetVoyagesEndedBefore(DateTime dateTime);

        Voyage Get(long id);

        List<Voyage> GetByFilter(long companyId, long vesselInCompanyId);

        PageResult<Voyage> GetPagedData(int pageSize, int pageIndex);

        PageResult<Voyage> GetPagedDataByFilter(long companyId, long vesselInCompanyId, int pageSize, int pageIndex);

        Voyage GetVoyage(Company company, DateTime date);
        Voyage GetVoyageContainingDuration(Company company, DateTime startDateTime, DateTime endDateTime);

        FuelReport GetVoyageValidEndOfVoyageFuelReport(long voyageId);
    }
}
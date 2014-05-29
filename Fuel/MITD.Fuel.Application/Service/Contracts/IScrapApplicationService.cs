using System;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Presentation.Contracts;
using MITD.Services.Application;

namespace MITD.Fuel.Application.Service.Contracts
{
    public interface IScrapApplicationService : IApplicationService
    {
        Scrap ScrapVessel(long vesselInCompanyId, long secondPartyId, DateTime scrapDate);
        Scrap UpdateScrap(long scrapId, long vesselInCompanyId, long secondPartyId, DateTime scrapDate);
        void DeleteScrap(long scrapId);

        ScrapDetail AddScrapDetail(long scrapId, double rob, double price, long currencyId, long goodId, long unitId, long tankId);
        ScrapDetail UpdateScrapDetail(long scrapId, long scrapDetailId, double rob, double price, long currencyId, long goodId, long unitId, long tankId);
        void DeleteScrapDetail(long scrapId, long scrapDetailId);
    }
}
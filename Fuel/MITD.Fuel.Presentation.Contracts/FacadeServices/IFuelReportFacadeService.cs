using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;
using System.Collections.Generic;
using System;

namespace MITD.Fuel.Presentation.Contracts.FacadeServices
{
    public interface IFuelReportFacadeService : IFacadeService
    {
        FuelReportDto GetById(long id, bool includeReferencesLookup);

        //PageResultDto<FuelReportDto> GetAll(int pageSize, int pageIndex, bool includeReferencesLookup);

        PageResultDto<FuelReportDto> GetByFilter(long companyId, long vesselId, int pageSize, int pageIndex, bool includeReferencesLookup);

        FuelReportDto Update(FuelReportDto fuelReportDto);

        FuelReportDetailDto UpdateFuelReportDetail(long fuelReportId, FuelReportDetailDto fuelReportDetailDto);

        List<CurrencyDto> GetAllCurrency();

        List<FuelReportInventoryOperationDto> GetInventoryOperations(long id, long detailId);
    }
}

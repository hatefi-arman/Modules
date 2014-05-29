
using System;
using System.Collections.Generic;
using MITD.Presentation;
using MITD.Presentation.Contracts;
using MITD.Fuel.Presentation.Contracts.DTOs;



namespace MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper
{
    public interface IFuelReportServiceWrapper : IServiceWrapper
    {
        void GetAll(Action<PageResultDto<FuelReportDto>, Exception> action, string methodName, int pageSize, int pageIndex);

        void GetById(Action<FuelReportDto, Exception> action, long id);

        void Add(Action<FuelReportDto, Exception> action, FuelReportDto ent);

        void Update(Action<FuelReportDto, Exception> action, FuelReportDto ent);

        void Delete(Action<string, Exception> action, long id);
        void UpdateFuelReportDetail(Action<FuelReportDetailDto, Exception> action, FuelReportDetailDto ent);

        void GetAllCurrency(Action<List<CurrencyDto>, Exception> action);


        void GetByFilter(Action<PageResultDto<FuelReportDto>, Exception> action, long companyId, long vesselId, int pageSize, int pageIndex);
    }
}
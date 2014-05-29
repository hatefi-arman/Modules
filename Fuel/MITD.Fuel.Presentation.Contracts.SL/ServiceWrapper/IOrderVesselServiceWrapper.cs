using System;
using System.Collections.Generic;
using MITD.Presentation;
using MITD.Presentation.Contracts;
using MITD.Fuel.Presentation.Contracts.DTOs;

namespace MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper
{
    public interface IVesselServiceWrapper : IServiceWrapper
    {
        void GetAll(Action<List<VesselDto>, Exception> action);

        void GetAll(Action<PageResultDto<VesselDto>, Exception> action, string methodName, int pageSize,
                         int pageIndex);

        void GetById(Action<VesselDto, Exception> action, int id, bool includeCompany = true, bool includeTanks = false);

        void GetPagedDataByFilter(Action<PageResultDto<VesselDto>, Exception> action, long companyId, int? pageSize, int? pageIndex);

        //void Add(Action<VesselDto, Exception> action, VesselDto ent);

        //void Update(Action<VesselDto, Exception> action, VesselDto ent);

        //void Delete(Action<string, Exception> action, int id);
    }
}

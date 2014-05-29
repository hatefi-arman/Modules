using System;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Presentation;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Presentation.Contracts.SL.ServiceWrapper
{
    public interface ICompanyServiceWrapper : IServiceWrapper
    {
        void GetAll(Action<PageResultDto<CompanyDto>, Exception> action, string methodName = null);

        void GetById(Action<CompanyDto, Exception> action, int id);

        void GetOwnedVessels(Action<PageResultDto<VesselDto>, Exception> action, long companyId);


        //void Add(Action<CompanyDto, Exception> action, CompanyDto ent);

        //void Update(Action<CompanyDto, Exception> action, CompanyDto ent);

        //void Delete(Action<string, Exception> action, int id);
    }
}

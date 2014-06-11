
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;
using MITD.Fuel.Presentation.Contracts.FacadeServices.Fuel;

namespace MITD.Fuel.Application.Facade
{
    [Interceptor(typeof(SecurityInterception))]
    public class FuelReportCompanyFacadeService : IFuelReportCompanyFacadeService
    {
        #region props

        private readonly ICompanyDomainService companyDomainService;
        private readonly IFacadeMapper<Company, CompanyDto> companyMapper;
        private readonly IVesselInCompanyDomainService vesselDomainService;
        private readonly IFacadeMapper<VesselInCompany, VesselDto> vesselMapper;
        #endregion

        #region ctor

        public FuelReportCompanyFacadeService(
            ICompanyDomainService companyDomainService,
            IVesselInCompanyDomainService vesselDomainService,
            IFacadeMapper<Company, CompanyDto> companyMapper,
            IFacadeMapper<VesselInCompany, VesselDto> vesselMapper)
        {
            this.companyDomainService = companyDomainService;
            this.companyMapper = companyMapper;
            this.vesselDomainService = vesselDomainService;
            this.vesselMapper = vesselMapper;
        }

        #endregion

        #region methods

        public List<CompanyDto> GetAll()
        {
            //var companyEntities = this.companyDomainService.GetAll();
            //var vesselInCompanyIds = companyEntities.SelectMany(e => e.Vessels).Distinct().ToList();
            //var vessels = this.vesselDomainService.Get(vesselInCompanyIds.Select(v=>v.Id).ToList());

            //var result = new List<CompanyDto>();
            //foreach (var ent in companyEntities)
            //{
            //    var entVessels = vessels.Where(v => ent.Vessels.Any(item => item.Id == v.Id)).ToList();
            //    var dto = this.companyMapper.MapToModel(ent);
            //    var dtoVessels = this.vesselMapper.MapToModel(entVessels);

            //    foreach (var vesselDto in dtoVessels)
            //    {
            //        dto.Vessels.Add(vesselDto);
            //    }
            //    result.Add(dto);
            //}
            //return result;

            var currentUserId = MITD.Fuel.Domain.Model.FakeDomainServices.FakeDomainService.GetUsers().FirstOrDefault().Id;

            var companyEntities = this.companyDomainService.GetUserCompanies(currentUserId);
            var enterprisePartiesVesselInCompanyIds = companyEntities.SelectMany(e => e.VesselsOperationInCompany).Distinct().ToList();
            var vessels = this.vesselDomainService.Get(enterprisePartiesVesselInCompanyIds.Select(v => v.Id).ToList());

            var result = new List<CompanyDto>();
            foreach (var ent in companyEntities)
            {
                var entVessels = vessels.Where(v => ent.VesselsOperationInCompany.Any(item => item.Id == v.Id)).ToList();
                var dto = this.companyMapper.MapToModel(ent);
                var dtoVessels = this.vesselMapper.MapToModel(entVessels);

                dto.Vessels.AddRange(dtoVessels);

                result.Add(dto);
            }
            return result;

        }

        #endregion

        public CompanyDto Add(CompanyDto data)
        {
            return null;
        }

        public CompanyDto Update(CompanyDto data)
        {
            return null;
        }

        public void Delete(CompanyDto data)
        {
        }

        public CompanyDto GetById(int id)
        {
            return null;
        }

        public PageResultDto<CompanyDto> GetAll(int pageSize, int pageIndex)
        {
            return null;
        }

        public void DeleteById(int id)
        {
        }

        public List<CompanyDto> GetByUserId(long userId)
        {
            var companyEntities = this.companyDomainService.GetUserCompanies(userId);
            var companiesVesselInCompanyIds = companyEntities.SelectMany(e => e.VesselsOperationInCompany).Distinct().ToList();
            var vessels = this.vesselDomainService.Get(companiesVesselInCompanyIds.Select(v => v.Id).ToList());

            var result = new List<CompanyDto>();
            foreach (var ent in companyEntities)
            {
                var entVessels = vessels.Where(v => ent.VesselsOperationInCompany.Any(item => item.Id == v.Id)).ToList();
                var dto = this.companyMapper.MapToModel(ent);
                var dtoVessels = this.vesselMapper.MapToModel(entVessels);

                dto.Vessels.AddRange(dtoVessels);

                result.Add(dto);
            }

            return result;
        }
    }
}

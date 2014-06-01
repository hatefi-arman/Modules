#region

using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Fuel.Application.Facade.Contracts.Mappers;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

#endregion

namespace MITD.Fuel.Application.Facade
{
    public class CompanyFacadeService : ICompanyFacadeService
    {
        #region props

        private readonly ICompanyDomainService _companyDomainService;
        private readonly IVesselInCompanyDomainService vesselDomainService;
        private readonly IFacadeMapper<Company, CompanyDto> _mapper;
        private readonly IVesselToVesselDtoMapper vesselDtoMapper;

        #endregion

        #region ctor

        public CompanyFacadeService(
            ICompanyDomainService companyDomainService,
            IVesselInCompanyDomainService vesselDomainService,
            IFacadeMapper<Company, CompanyDto> mapper, IVesselToVesselDtoMapper vesselDtoMapper)
        {
            _companyDomainService = companyDomainService;
            this.vesselDomainService = vesselDomainService;
            _mapper = mapper;
            this.vesselDtoMapper = vesselDtoMapper;
        }

        #endregion

        #region methods

        public List<CompanyDto> GetAll()
        {
            var entities = _companyDomainService.GetAll();
            var dtos =new List<CompanyDto>();
                 entities.ForEach(c =>
                                  {
                                      var dto = new CompanyDto()
                                                {
                                            Id=c.Id,
                                            Code=c.Code,
                                            Name=c.Name
                                           
                                                };
                                      dtos.Add(dto);
                                  });

          //  var dtos = _mapper.MapToModel(entities).ToList();

            return dtos;
        }

        public PageResultDto<VesselDto> GetOwnedVessels(long companyId)
        {
            var ownedVessels = vesselDomainService.GetOwnedVessels(companyId);

            var dtos = vesselDtoMapper.MapToModel(ownedVessels);

            return new PageResultDto<VesselDto>
                    {
                        Result = dtos.ToList()
                    };
        }

        #endregion

    }
}
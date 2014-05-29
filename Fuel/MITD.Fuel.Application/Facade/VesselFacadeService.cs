#region

using System.Collections.Generic;
using System.Linq;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;
using MITD.Services.Facade;

#endregion

namespace MITD.Fuel.Application.Facade
{
    public class VesselFacadeService : IVesselFacadeService
    {

        #region props
        private readonly IVesselInCompanyDomainService _vesselService;
        private readonly ICompanyDomainService _iCompanyDomainService;
        private IFacadeMapper<VesselInCompany, VesselDto> Mapper { get; set; }

        #endregion

        #region ctor


        public VesselFacadeService(IVesselInCompanyDomainService vesselService,
            ICompanyDomainService iCompanyDomainService,
                                        IFacadeMapper<VesselInCompany, VesselDto> mapper)
        {
            _vesselService = vesselService;
            _iCompanyDomainService = iCompanyDomainService;
            Mapper = mapper;
        }

        #endregion

        #region methods

        public List<VesselDto> GetAll()
        {
            var entities = _vesselService.GetAll();
            var dtos = Mapper.MapToModel(entities).ToList();
            return dtos;
        }

        public List<VesselDto> GetCompanyVessels(long enterpriseId)
        {
            var entities = _iCompanyDomainService.GetCompanyVessels(enterpriseId);
            var dtos = Mapper.MapToModel(entities).ToList();
            return dtos;
        }

        public List<VesselDto> GetOwnedVessels(long companyId)
        {
            var entities = _vesselService.GetInactiveVessels(companyId);
            var dtos = Mapper.MapToModel(entities).ToList();
            return dtos;
        }

        public VesselDto GetById(long id)
        {
            var entity = _vesselService.Get(id);
            var dto = Mapper.MapToModel(entity);
            dto.TankDtos = entity.Tanks.Select(t => new TankDto() { Code = t.Name, Id = t.Id }).ToList();

            return dto;
        }

        #endregion





        public List<VesselDto> GetOwnedOrCharterInVessels(long companyId)
        {
            var entities = _vesselService.GetOwnedOrCharterInVessels(companyId);
            var dtos = Mapper.MapToModel(entities).ToList();
            return dtos;
        }
    }
}
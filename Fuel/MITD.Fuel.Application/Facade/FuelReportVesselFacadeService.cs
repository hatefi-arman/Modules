
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
    public class FuelReportVesselFacadeService : IFuelReportVesselFacadeService
    {
        #region props

        private readonly IVesselInCompanyDomainService _domainService;
        private readonly IFacadeMapper<VesselInCompany, VesselDto> _mapper;
        #endregion

        #region ctor

        public FuelReportVesselFacadeService(IVesselInCompanyDomainService appService,
               IFacadeMapper<VesselInCompany, VesselDto> mapper)
        {
            this._domainService = appService;
            this._mapper = mapper;
        }

        #endregion

        #region methods

        public List<VesselDto> GetAll()
        {
            var entities = this._domainService.GetAll();

            var dtos = this._mapper.MapToModel(entities).ToList();

            return dtos;
        }

        #endregion
    }
}

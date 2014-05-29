using System;
using System.Collections.Generic;
using System.Web.Http;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;


namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public class VesselController : ApiController
    {
        #region props
        private IVesselFacadeService FacadeService { get; set; }
        #endregion

        #region ctor

        public VesselController(IVesselFacadeService facadeService)
        {
            if (facadeService == null)
                throw new Exception(" facade service can not be null");

            this.FacadeService = facadeService;
        }
        #endregion

        #region methods

        public List<VesselDto> Get()
        {
            var dtos = this.FacadeService.GetAll();
            return dtos;
        }

        public PageResultDto<VesselDto> Get(long companyId, int? pageSize = null, int? pageIndex = null)
        {
            var dtos = this.FacadeService.GetCompanyVessels(companyId);

            var result = new PageResultDto<VesselDto>
                         {
                             CurrentPage = 0,
                             PageSize = 0,
                             TotalCount = dtos.Count,
                             Result = dtos,
                             TotalPages = 0
                         };

            return result;
        }

        #endregion
    }
}

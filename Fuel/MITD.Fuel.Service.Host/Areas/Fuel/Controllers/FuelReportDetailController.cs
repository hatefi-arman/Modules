using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Presentation.Contracts.FacadeServices.Fuel;
using MITD.Presentation.Contracts;


namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public class FuelReportDetailController : ApiController
    {
        #region props

        private IFuelReportFacadeService FacadeService { get; set; }

        #endregion

        #region ctor

        public FuelReportDetailController()
        {

        }

        public FuelReportDetailController(IFuelReportFacadeService facadeService)
        {
            if (facadeService == null)
                throw new Exception(" facade service can not be null");

            this.FacadeService = facadeService;
        }

        #endregion

        #region methods

        public FuelReportDetailDto Put(long id, long detailId, [FromBody] FuelReportDetailDto dto)
        {
            return this.FacadeService.UpdateFuelReportDetail(id, dto);
        }

        #endregion
    }
}

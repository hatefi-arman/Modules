using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Presentation.Contracts.FacadeServices.Fuel;
using MITD.Presentation.Contracts;


namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public class FuelReportController : ApiController
    {
        #region props

        private IFuelReportFacadeService FacadeService { get; set; }

        #endregion

        #region ctor

        public FuelReportController()
        {

        }

        public FuelReportController(IFuelReportFacadeService facadeService)
        {
            if (facadeService == null)
                throw new Exception(" facade service can not be null");

            this.FacadeService = facadeService;
        }
        #endregion

        #region methods

        public PageResultDto<FuelReportDto> Get(int companyId, int vesselId, int pageSize, int pageIndex)
        {
            var data = this.FacadeService.GetByFilter(companyId, vesselId, pageSize, pageIndex, false);

            return data;

        }

        public FuelReportDto Get(int id)
        {
            var result = this.FacadeService.GetById(id, true);
            return result;
        }

        public ResultFuelReportDto Post([FromBody] FuelReportCommandDto entity)
        {
         
          if(int.Parse(entity.VesselReportReference)%2==0)
            return new ResultFuelReportDto()
                {
                    
                    Type = ResultType.Accept

                };
          else
          {
              return new ResultFuelReportDto()
              {
                  Message = "Buss01",
                  Type = ResultType.Exception

              };
          }
        }


        public FuelReportDto Put([FromBody] FuelReportDto entity)
        {
            var ent = this.FacadeService.Update(entity);
            return ent;
        }

        #endregion
    }
}

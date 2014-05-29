using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public class CharterVesselController : ApiController
    {

        #region Prop

        private IVesselFacadeService _vesselFacadeService;

        #endregion

        #region Ctor

        public CharterVesselController(IVesselFacadeService vesselFacadeService)
        {
            this._vesselFacadeService = vesselFacadeService;
        }
        #endregion

        #region Method

        public PageResultDto<VesselDto> Get(CharterType charterType, long companyId)
        {
            var res = new PageResultDto<VesselDto>();
            if (charterType == CharterType.In)
            {
                res.Result = this._vesselFacadeService.GetOwnedVessels(companyId) as IList<VesselDto>;
            }
            else
            {
                // todo bzcomment
                res.Result = this._vesselFacadeService.GetOwnedOrCharterInVessels(companyId) as IList<VesselDto>;
            }
            return res;

        }

        public VesselDto GetById(CharterType charterType,  long id,bool flag)
        {
            var res = new VesselDto();
            if (charterType == CharterType.In)
            {
                res = this._vesselFacadeService.GetById(id);
            }
            else
            {
                // todo bzcomment
                res = this._vesselFacadeService.GetById(id);
            }
            return res;

        }

        // POST api/chartervessel
        public void Post([FromBody]string value)
        {
        }

        // PUT api/chartervessel/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/chartervessel/5
        public void Delete(int id)
        {
        }

        #endregion


       
    }
}

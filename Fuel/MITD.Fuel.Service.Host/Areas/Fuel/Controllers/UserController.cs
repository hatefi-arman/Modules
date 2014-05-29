using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public class UserController : ApiController
    {
        #region props
        private IUserFacadeService FacadeService { get; set; }
        #endregion

        #region ctor

        public UserController()
        {
            try
            {
                this.FacadeService = ServiceLocator.Current.GetInstance<IUserFacadeService>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public UserController(IUserFacadeService facadeService)
        {
            if (facadeService == null)
                throw new Exception(" facade service can not be null");

            this.FacadeService = facadeService;
        }
        #endregion

        #region methods

        public List<UserDto> Get()
        {
            //TODO: <A.H> Invalid implementation of GetAll
            var data = this.FacadeService.GetAll(10, 1);
            return data;
        }

        public PageResultDto<UserDto> Get(int pageSize, int pageIndex)
        {
            var data = this.FacadeService.GetAll(pageSize, pageIndex);
            var result = new PageResultDto<UserDto>
                             {
                                 CurrentPage = 0,
                                 PageSize = data.Count,
                                 Result = data,
                                 TotalCount = data.Count,
                                 TotalPages = 1
                             };
            return result;
        }

        public UserDto Get(int id)
        {
            var result = this.FacadeService.GetUserWithCompany(id);
            return result;
        }

        public void Post([FromBody] UserDto entity)
        {
            this.FacadeService.Add(entity);
        }

        public void Put(int id, [FromBody] UserDto entity)
        {
            this.FacadeService.Update(entity);
        }

        public void Delete(int id)
        {
            this.FacadeService.DeleteById(id);
        }

        #endregion
    }
}

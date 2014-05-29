using System;
using System.Web.Http;
using MITD.Core;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public class CompanyController : ApiController
    {
        #region props
        private ICompanyFacadeService FacadeService { get; set; }
        #endregion

        #region ctor

        public CompanyController()
        {
            try
            {
                this.FacadeService = ServiceLocator.Current.GetInstance<ICompanyFacadeService>();
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        public CompanyController(ICompanyFacadeService facadeService)
        {
            if (facadeService == null)
                throw new Exception(" facade service can not be null");

            this.FacadeService = facadeService;
        }
        #endregion

        #region methods

        public PageResultDto<CompanyDto> Get()
        {
            var data = this.FacadeService.GetAll();
            var paged = new PageResultDto<CompanyDto>
                            {
                                CurrentPage = 1,
                                PageSize = data.Count,
                                Result = data,
                                TotalCount = data.Count,
                                TotalPages = 1
                            };
            return paged;
        }

        #endregion
    }
}

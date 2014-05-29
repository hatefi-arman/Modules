using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MITD.Core;
using MITD.Domain.Repository;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.Enums;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices.Fuel;
using MITD.Presentation.Contracts;

namespace MITD.Fuel.Service.Host.Areas.Fuel.Controllers
{
    public partial class OrderController : ApiController
    {
        #region props

        private IOrderFacadeService FacadeService { get; set; }

        #endregion

        #region ctor

        public OrderController()
        {
            try
            {
                this.FacadeService = ServiceLocator.Current.GetInstance<IOrderFacadeService>();
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public OrderController(IOrderFacadeService facadeService)
            : base()
        {
            if (facadeService == null)
                throw new Exception(" facade service can not be null");

            this.FacadeService = facadeService;

          
        }
        #endregion

        #region methods

        public PageResultDto<OrderDto> Get(int companyId, int orderCreatorId, DateTime fromDate, DateTime toDate, int pageSize, int pageIndex, string orderTypes, long? supplierId, long? transporterId, bool includeOrderItem, long fake,string orderIdList, string orderCode, bool submitedState)
        {
            var result = this.FacadeService.GetByFilter(companyId, orderCreatorId, orderTypes, fromDate,
                                                        toDate, pageSize, pageIndex, supplierId, transporterId, includeOrderItem, orderIdList, orderCode, submitedState);
            return result;
        }

        public PageResultDto<OrderDto> Get(int pageSize, int pageIndex)
        {
            var data = this.FacadeService.GetAll(pageSize, pageIndex);
            return data;
        }

        public OrderDto Get(int id)
        {
            var result = this.FacadeService.GetById(id);
            return result;
        }

        public OrderDto Post([FromBody] OrderDto entity)
        {
            var result = this.FacadeService.Add(entity);
            return result;
        }

        public OrderDto Put(int id, [FromBody] OrderDto orderEntity)
        {
            var result = this.FacadeService.Update(orderEntity);
            return result;
        }

        public void Delete(int id)
        {
            this.FacadeService.DeleteById(id);
        }

        public void Delete([FromBody] OrderDto entity)
        {
            this.FacadeService.Delete(entity);
        }

        #endregion



    }
}

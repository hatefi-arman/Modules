using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using MITD.Fuel.Application.Facade.Security;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.FuelSecurity.Domain.Model.ErrorException;
using MITD.Presentation;

namespace MITD.Fuel.Application
{
   public class SecurityInterception:IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var securityService = SecurityFacadeServiceFactory.Create();
            try
            {
                var user = ClaimsPrincipal.Current;

                if (invocation.Method.DeclaringType.Name == "IApprovalFlowFacadeService")
                {

                    var dto = invocation.Arguments[0] as ApprovmentDto ;
                    if (securityService.IsAuthorize(dto.ActionEntityType ,dto.DecisionType, user))
                    {
                        invocation.Proceed();
                        //logservice
                    }
                    else
                    {
                        throw new FuelSecurityAccessException(7001, "Access Error");
                    }
                }
                else
                {
                    if (securityService.IsAuthorize(invocation.Method.DeclaringType.Name, invocation.Method.Name, user))
                    {
                        invocation.Proceed();
                        
                    }
                    else
                    {
                        throw new FuelSecurityAccessException(7001, "Access Error");
                    }
                }
               

            }
            finally
            {
                SecurityFacadeServiceFactory.Release(securityService);
            }
        }


       //logservice
    }
}

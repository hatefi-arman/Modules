using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using MITD.Fuel.Application.Facade.Security;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.FuelSecurity.Domain.Model.ErrorException;

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
                if (securityService.IsAuthorize(invocation.Method.DeclaringType.Name,invocation.Method.Name,user))
                {
                    invocation.Proceed();
                    //logservice
                }
                else
                {
                    throw new BusinessRuleException("7001", "Access Error");
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

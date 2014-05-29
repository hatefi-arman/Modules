using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Presentation.Contracts.FacadeServices;

namespace MITD.Fuel.Application.Facade
{
    public class SecurityFacadeService : ISecurityFacadeService
    {
        public bool IsAuthorize(string className, string methodName, System.Security.Claims.ClaimsPrincipal userClaimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public List<FuelSecurity.Domain.Model.ActionType> GetUserAuthorizedActions(System.Security.Claims.ClaimsPrincipal userClaimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public void AddUpdateUser(System.Security.Claims.ClaimsPrincipal userClaimsPrincipal)
        {
            throw new NotImplementedException();
        }

        public FuelSecurity.Domain.Model.User GetLogonUser()
        {
            throw new NotImplementedException();
        }
    }  
}

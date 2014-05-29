using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MITD.FuelSecurity.Domain.Model;
using MITD.Services.Facade;

namespace MITD.Fuel.Presentation.Contracts.FacadeServices
{
   public interface ISecurityFacadeService :IFacadeService
   {
       bool IsAuthorize(string className, string methodName, ClaimsPrincipal userClaimsPrincipal);

       List<ActionType> GetUserAuthorizedActions(ClaimsPrincipal userClaimsPrincipal);
       void AddUpdateUser(ClaimsPrincipal userClaimsPrincipal);

       User GetLogonUser();
   }
}

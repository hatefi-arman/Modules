using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model.Service
{
    public interface ISecurityServiceChecker
    {
        List<ActionType> GetAllAuthorizedActionTypes(List<User> users);
        bool IsAuthorize(List<ActionType> authorizeActions, List<ActionType> actions);
    }
}

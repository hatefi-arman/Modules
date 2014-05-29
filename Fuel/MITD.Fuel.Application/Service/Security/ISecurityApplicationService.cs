using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.FuelSecurity.Domain.Model;

namespace MITD.Fuel.Application.Service.Security
{
    public interface ISecurityApplicationService
    {
        bool IsAuthorize(List<ActionType> userActionTypes,List<ActionType> methodRequiredActionTypes );
        List<ActionType> GetAllAuthorizedActions(List<User> users);

        User GetUser(long id);
        User GetLogonUser();

        User AddUser(string firstName, string lastName, bool isActive, string email,
            Dictionary<int, bool> customActions, List<long> groups);

        User UpdateUser(long id,string firstName, string lastName, bool isActive, string email,
           Dictionary<int, bool> customActions, List<long> groups);

        Group UpdateGroup(long id, string description,Dictionary<int,bool> customActions);
        Group AddGroup(long id, string description, Dictionary<int, bool> customActions);

    }
}

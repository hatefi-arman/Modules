using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using MITD.Core;
using MITD.FuelSecurity.Domain.Model.Repository;
using MITD.FuelSecurity.Domain.Model.Service;
using MITD.FuelSecurity.Domain.Model;

namespace MITD.Fuel.Application.Service.Security
{
    public class SecurityApplicationService : ISecurityApplicationService
    {

        private IUserRepository _userRepository;
        private ISecurityServiceChecker _securityServiceChecker;


        #region  ctor

        public SecurityApplicationService()
        {
            var x = ServiceLocator.Current.GetAllInstances<IUserRepository>();
            var x1 = ServiceLocator.Current.GetAllInstances<ISecurityServiceChecker>();
        }

        public SecurityApplicationService(IUserRepository userRepository, ISecurityServiceChecker securityServiceChecker)
        {
            this._userRepository = userRepository;
            this._securityServiceChecker = securityServiceChecker;
        }

        #endregion

        public bool IsAuthorize(List<ActionType> userActionTypes, List<ActionType> methodRequiredActionTypes)
        {
            _securityServiceChecker=new SecurityServiceChecker(this._userRepository);
            return _securityServiceChecker.IsAuthorize(userActionTypes, methodRequiredActionTypes);
        }

        public List<ActionType> GetAllAuthorizedActions(List<User> users)
        {
            return _securityServiceChecker.GetAllAuthorizedActionTypes(users);
        }

        public User GetUser(long id)
        {
            return _userRepository.GetUserById(id)as User;
        }

        public User GetLogonUser()
        {
            if (ClaimsPrincipal.Current == null)
                return null;
            return GetUser(long.Parse(ClaimsPrincipal.Current.Identity.Name));
        }

        public User AddUser(string firstName, string lastName, bool isActive, string email, Dictionary<int, bool> customActions, List<long> groups)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    var u = new User(0, firstName, lastName, email, "");
                    //assignCustomActionsToParty(u,customActions);
                    //assignUserGroupsToUser(u,groups);
                    //_userRepository.Add(u);
                    //scope.Complete();
                    return u;

                }
            }
            catch (Exception exp)
            {

                throw;
            }
        }

        public User UpdateUser(long id, string firstName, string lastName, bool isActive, string email, Dictionary<int, bool> customActions, List<long> groups)
        {
            using (var scope = new TransactionScope())
            {
                var u = _userRepository.GetUserById(id);
                //u.Update(firstName,lastName,email,isActive,customActions,groups);
                //scope.Complete();
                return u as User;
            }
        }

        public Group UpdateGroup(long id, string description, Dictionary<int, bool> customActions)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    //  var u=
                    return new Group();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Group AddGroup(long id, string description, Dictionary<int, bool> customActions)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    var g = new Group(id, description, "");
                    //assignCustomActionsToParty(g,customActions);
                    //_userRepository.Add(g);
                    //scope.Complete();
                    return g;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private void assignCustomActionsToParty(Party party, Dictionary<int, bool> customActions)
        {
            foreach (var actid in customActions)
            {
                //ActionType act = ActionType.FromValue(actid.Key.ToString());

                //party.AssignCustomActions(act,actid.Value);
            }
        }

        private void assignUserGroupsToUser(User user, List<long> groupIds)
        {
            foreach (var groupId in groupIds)
            {
                var group = _userRepository.GetGroupById(groupId);
                user.AssignGroup(group);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.FuelSecurity.Domain.Model.ErrorException;
using MITD.FuelSecurity.Domain.Model.Repository;

namespace MITD.FuelSecurity.Domain.Model.Service
{
    public class SecurityServiceChecker : ISecurityServiceChecker
    {

        private readonly IUserRepository _userRepository;

        public SecurityServiceChecker(IUserRepository userRepository)
        {
            userRepository = _userRepository;
        }

        public List<ActionType> GetAllAuthorizedActionTypes(List<User> users)
        {
            if (users == null || users.Count == 0)
                throw new FuelSecurityAccessException(701, "You are not authorize to access to system ");

            var userId = users[0].Id;
            if (users.Any(c=>c.Id!=userId))
            throw new Exception("user name must same");

            var user = _userRepository.GetUserById(userId);
            if(user==null)
                throw new NullReferenceException("User");

            var authorizedActionsUser = new List<ActionType>();
            users.ForEach(c=>authorizedActionsUser.AddRange(c.Actions));
            authorizedActionsUser = authorizedActionsUser.Distinct().ToList();

            //user.CustomActions.ToList().ForEach(c =>
            //                                    {
            //                                        var custmAction =
            //                                            ActionType.GetAllActions().Single(a => a.Value == c.Key.ToString());
            //                                        if (c.Value)
            //                                        {
            //                                            if(!authorizedActionsUser.Contains(custmAction))
            //                                                authorizedActionsUser.Add(custmAction);

            //                                        }
            //                                        else if (authorizedActionsUser.Contains(custmAction))
            //                                        {
            //                                            authorizedActionsUser.Remove(custmAction);
            //                                        }
            //                                    });

            return authorizedActionsUser;


        }

        public bool IsAuthorize(List<ActionType> authorizeActions, List<ActionType> actions)
        {
            return actions.All(authorizeActions.Contains);
        }
    }
}

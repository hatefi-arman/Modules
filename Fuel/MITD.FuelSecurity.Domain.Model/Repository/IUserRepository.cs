using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;

namespace MITD.FuelSecurity.Domain.Model.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        IList<User> GetAllUsers();
        void FindUser(Expression<Func<User, bool>> predicatExpression, ListFetchStrategy<User> fetchStrategy);
        User GetUserById(long id);
        IList<Group> GetAllGroups();
        Group GetGroupById(long id);
        void Delete(Party user);
        void Add(Party group);


    }
}

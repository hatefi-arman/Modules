using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;

namespace MITD.FuelSecurity.Domain.Model.Repository
{
    public interface IUserRepository : IRepository<Party>
    {
        IList<Party> GetAllUsers();
        void FindUser(Expression<Func<Party, bool>> predicatExpression, ListFetchStrategy<Party> fetchStrategy);
        Party GetUserById(long id);
        IList<Group> GetAllGroups();
        Group GetGroupById(long id);
        void Delete(Party user);
        void Add(Party group);


    }
}

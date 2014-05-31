using System;
using System.Collections.Generic;

using MITD.DataAccess.EF;
using MITD.Domain.Repository;

using MITD.FuelSecurity.Domain.Model;
using MITD.FuelSecurity.Domain.Model.Repository;

namespace MITD.Fuel.Data.EF.Repositories
{
    public class UserRepository : EFRepository<User>, IUserRepository
    {
        public UserRepository(EFUnitOfWork efUnitOfWork)
            : base(efUnitOfWork)
        {

        }

        public UserRepository(IUnitOfWorkScope iUnitOfWorkScope)
            : base(iUnitOfWorkScope)
        {

        }

        public IList<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public void FindUser(System.Linq.Expressions.Expression<Func<User, bool>> predicatExpression, ListFetchStrategy<User> fetchStrategy)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(long id)
        {
            throw new NotImplementedException();
        }

        public IList<Group> GetAllGroups()
        {
            throw new NotImplementedException();
        }

        public Group GetGroupById(long id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Party user)
        {
            throw new NotImplementedException();
        }

        public void Add(Party group)
        {
            throw new NotImplementedException();
        }
    }
}

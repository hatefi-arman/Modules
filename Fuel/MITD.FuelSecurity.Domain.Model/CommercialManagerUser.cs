using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
    public class CommercialManagerUser:User
    {
        public override List<ActionType> Actions {
            get {return new List<ActionType>(); }
        }

        public CommercialManagerUser(long id, string firstName, string lastName, string email, string userName)
            : base(id, "CommercialManagerUser", firstName, lastName, email, userName)
        {

        }
    }
}

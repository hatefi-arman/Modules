using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
   public  class FinancialManagerUser:User
    {
        public override List<ActionType> Actions { get { return new List<ActionType>(); } }

        public FinancialManagerUser(long id, string firstName, string lastName, string email, string userName)
            : base(id, "FinancialManagerUser", firstName, lastName, email, userName)
        {

        }

    }
}

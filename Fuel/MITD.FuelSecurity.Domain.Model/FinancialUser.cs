using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
   public  class FinancialUser:User
    {
        public override List<ActionType> Actions { get { return new List<ActionType>(); } }

        public FinancialUser(long id, string firstName, string lastName, string email)
            : base(id, "FinancialUser", firstName, lastName, email)
        {

        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
    public class ComercialUser:User
    {
        public override List<ActionType> Actions {
            get {return new List<ActionType>(); }
        }

        public ComercialUser(long id, string firstName, string lastName, string email)
            :base( id,  firstName,  lastName,  email)
        {
            
        }
    }
}

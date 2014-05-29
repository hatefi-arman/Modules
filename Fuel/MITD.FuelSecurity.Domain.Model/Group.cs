using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.FuelSecurity.Domain.Model
{
   public class Group:Party
    {
       private  string _description;
       public virtual string Description { get { return _description; } }
       public Group()
       {
           
       }

       public Group(long id, string description):base(id)
       {
           this._description = description;
       }

       public virtual void Update(string description, Dictionary<ActionType, bool> customActions)
       {
           this._description = description;
           UpdateCustomActions(customActions);
       }
    }
}

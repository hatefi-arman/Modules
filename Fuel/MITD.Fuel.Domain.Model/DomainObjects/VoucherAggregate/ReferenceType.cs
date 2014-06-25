using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class ReferenceType
    {

        #region Prop
        public int Id { get; set; }
        public string Name { get; set; }

        public static ReferenceType CharterIn
        {
            get { return new ReferenceType(1, "CharterIn"); }
        }


        #endregion

        public ReferenceType()
        {

        }

        public ReferenceType(int id, string name)
        {
            this.Name = name;
            this.Id = id;

        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class Account
    {

        #region prop

        public int Id { get; private set; }
        public string Code { get; private set; }
        public string  Name{ get; private set; }
        #endregion


        #region ctor

        public Account(int id,string code,string name)
        {
            this.Id = id;
            this.Code = code;
            this.Name = name;
        }
        #endregion
    }
}

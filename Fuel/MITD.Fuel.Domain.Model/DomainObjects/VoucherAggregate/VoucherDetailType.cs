using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.Enums;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class VoucherDetailType
    {

        #region Prop

        public int Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public VoucherType VoucherType { get; set; }


        public VoucherDetailType CharterInStart { get; set; }

        #endregion


        #region Ctor

        public VoucherDetailType(int id,string name,string code,VoucherType voucherType)
        {
            this.Id = id;
            this.Name = name;
            this.Code = code;
            this.VoucherType = voucherType;
        }

        #endregion



    }
}

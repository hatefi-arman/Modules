using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.Enums;
using NHibernate.Linq;
using Remotion.Linq.Parsing.ExpressionTreeVisitors.Transformation.PredefinedTransformations;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class VoucherDetailType
    {

        #region Prop
        
        public int Id { get; set; }
        public string Name { get; set; }

        public string Code { get; set; }

        public VoucherType VoucherType { get; set; }


        public static VoucherDetailType CharterInStart {
            get {return new VoucherDetailType(1,"CharterInStart","01" ); }
        }

        #endregion


        #region Ctor

        public VoucherDetailType(int id,string name,string code)
        {
            this.Id = id;
            this.Name = name;
            this.Code = code;
            
        
        }

        #endregion

        #region Method

        public  void SetVoucherType(VoucherType voucherType)
        {
            VoucherType = voucherType;
        }

        #endregion


    }
}

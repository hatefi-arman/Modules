using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
   public class AsgnVoucherSegments
    {
        
            #region Prop

            public int Typ { get; set; }

            public int SegmentId { get; set; }

            public int VoucherSetingId { get; set; }

            public bool IsDebit
            {
                get { return (Typ == 1) ? true : false; }
            }
            public bool IsCredit { get { return (Typ == 2) ? true : false; } }



            #endregion


            #region ctor

            public AsgnVoucherSegments()
            {

            }


            public AsgnVoucherSegments(int typ, int segmentId, int voucherSetingId)
            {
                this.Typ = typ;
                this.SegmentId = segmentId;
                this.VoucherSetingId = voucherSetingId;
            }
            #endregion
        }
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class Segment
    {
        #region Prop
        public long Id { get; private set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public SegmentType SegmentType { get; set; }
        public byte[] TimeStamp { get; set; }
        public int SegmentTypeId { get; set; }


        #endregion

        #region Ctor

        public Segment()
        {
            
        }

        public Segment(long id, string name, string code,int segmentTypeId)
        {
            this.Id = id;
            this.Name = name;
            this.Code = code;
            SegmentTypeId = segmentTypeId;

        }

        #endregion


    }
}

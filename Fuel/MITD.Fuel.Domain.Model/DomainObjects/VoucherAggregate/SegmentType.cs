using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
   public class SegmentType
    {
       public string Name { get; set; }
       public string Code { get; set; }

       public SegmentType None {
           get { return new SegmentType("None","0"); }
       }
       public SegmentType Vessel { get { return new SegmentType("Vessel", "1"); } }
       public SegmentType Port { get { return new SegmentType("Port", "2"); } }
       public SegmentType Voayage { get { return new SegmentType("Voayage", "3"); } }
       public SegmentType Company { get { return new SegmentType("Compony", "4"); } }  
       
       
        
       public SegmentType(string name,string code)
       {
           this.Name = name;
           this.Code = code;

       }
    }
}

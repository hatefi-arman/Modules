// ReSharper disable RedundantUsingDirective
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable RedundantNameQualifier

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
//using DatabaseGeneratedOption = System.ComponentModel.DataAnnotations.DatabaseGeneratedOption;

namespace MITD.Fuel.Integration.Inventory.Data.ReversePOCO
{
    // OperationReference
    public partial class OperationReference
    {
        public long Id { get; set; } // Id (Primary key)
        public long OperationId { get; set; } // OperationId
        public int OperationType { get; set; } // OperationType
        public string ReferenceType { get; set; } // ReferenceType
        public string ReferenceNumber { get; set; } // ReferenceNumber
    }

}

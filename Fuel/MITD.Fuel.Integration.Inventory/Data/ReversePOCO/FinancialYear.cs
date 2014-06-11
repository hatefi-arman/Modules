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
    // FinancialYear
    public partial class FinancialYear
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name
        public DateTime StartDate { get; set; } // StartDate
        public DateTime EndDate { get; set; } // EndDate
        public int UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<TimeBucket> TimeBuckets { get; set; } // TimeBucket.FK_TimeBucket_FinancialYearId

        // Foreign keys
        public virtual User User { get; set; } // FK_FinancialYear_UserCreatorId

        public FinancialYear()
        {
            CreateDate = System.DateTime.Now;
            TimeBuckets = new List<TimeBucket>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

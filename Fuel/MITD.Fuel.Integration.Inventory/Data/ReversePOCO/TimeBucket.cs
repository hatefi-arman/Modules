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
    // TimeBucket
    public partial class TimeBucket
    {
        public int Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name
        public DateTime? StartDate { get; set; } // StartDate
        public DateTime? EndDate { get; set; } // EndDate
        public int FinancialYearId { get; set; } // FinancialYearId
        public int UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate
        public bool? Active { get; set; } // Active

        // Foreign keys
        public virtual FinancialYear FinancialYear { get; set; } // FK_TimeBucket_FinancialYearId
        public virtual User User { get; set; } // FK_TimeBucket_UserCreatorId

        public TimeBucket()
        {
            StartDate = System.DateTime.Now;
            EndDate = System.DateTime.Now;
            CreateDate = System.DateTime.Now;
            Active = false;
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

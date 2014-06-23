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
    // UnitConverts
    public partial class UnitConvert
    {
        public int Id { get; set; } // Id (Primary key)
        public int UnitId { get; set; } // UnitId
        public int SubUnitId { get; set; } // SubUnitId
        public decimal Coefficient { get; set; } // Coefficient
        public DateTime? EffectiveDate { get; set; } // EffectiveDate
        public int UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Foreign keys
        public virtual Unit Unit_SubUnitId { get; set; } // FK_UnitConverts_SubUnitId
        public virtual Unit Unit_UnitId { get; set; } // FK_UnitConverts_UnitId
        public virtual User User { get; set; } // FK_UnitConverts_UserCreatorId

        public UnitConvert()
        {
            EffectiveDate = System.DateTime.Now;
            CreateDate = System.DateTime.Now;
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

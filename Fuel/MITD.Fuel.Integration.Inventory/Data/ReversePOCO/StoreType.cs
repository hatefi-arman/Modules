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
    // StoreTypes
    public partial class StoreType
    {
        public int Id { get; set; } // Id (Primary key)
        public short Code { get; set; } // Code
        public byte Type { get; set; } // Type
        public string InputName { get; set; } // InputName
        public string OutputName { get; set; } // OutputName
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<Transaction> Transactions { get; set; } // Transactions.FK_Transaction_StoreTypesId

        // Foreign keys
        public virtual User User { get; set; } // FK_StoreTypes_UserCreatorId

        public StoreType()
        {
            Type = 0;
            CreateDate = System.DateTime.Now;
            Transactions = new List<Transaction>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

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
    // Goods
    public partial class Good
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code
        public string Name { get; set; } // Name
        public bool Active { get; set; } // Active
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<TransactionItem> TransactionItems { get; set; } // TransactionItems.FK_TransactionItems_GoodId

        // Foreign keys
        public virtual User User { get; set; } // FK_Goods_UserCreatorId

        public Good()
        {
            Active = true;
            CreateDate = System.DateTime.Now;
            TransactionItems = new List<TransactionItem>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

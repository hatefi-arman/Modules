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
    // Warehouse
    public partial class Warehouse
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code
        public string Name { get; set; } // Name
        public int CompanyId { get; set; } // CompanyId
        public bool? Active { get; set; } // Active
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<Transaction> Transactions { get; set; } // Transactions.FK_Transaction_WarehouseId

        // Foreign keys
        public virtual Company Company { get; set; } // FK_Warehouse_CompanyId
        public virtual User User { get; set; } // FK_Warehouse_UserCreatorId

        public Warehouse()
        {
            Active = true;
            CreateDate = System.DateTime.Now;
            Transactions = new List<Transaction>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

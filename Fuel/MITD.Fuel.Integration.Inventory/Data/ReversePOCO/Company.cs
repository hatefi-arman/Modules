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
    // Companies
    public partial class Company
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code
        public string Name { get; set; } // Name
        public bool? Active { get; set; } // Active
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<Warehouse> Warehouses { get; set; } // Warehouse.FK_Warehouse_CompanyId

        // Foreign keys
        public virtual User User { get; set; } // FK_Companies_UserCreatorId

        public Company()
        {
            Active = true;
            CreateDate = System.DateTime.Now;
            Warehouses = new List<Warehouse>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

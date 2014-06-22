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
    // Units
    public partial class Unit
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code
        public string Name { get; set; } // Name
        public bool? IsCurrency { get; set; } // IsCurrency
        public bool? IsBaseCurrency { get; set; } // IsBaseCurrency
        public bool Active { get; set; } // Active
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<TransactionItem> TransactionItems { get; set; } // TransactionItems.FK_TransactionItems_QuantityUnitId
        public virtual ICollection<TransactionItemPrice> TransactionItemPrices_PriceUnitId { get; set; } // TransactionItemPrices.FK_TransactionItemPrices_PriceUnitId
        public virtual ICollection<TransactionItemPrice> TransactionItemPrices_QuantityUnitId { get; set; } // TransactionItemPrices.FK_TransactionItemPrices_QuantityUnitId
        public virtual ICollection<UnitConvert> UnitConverts_SubUnitId { get; set; } // UnitConverts.FK_UnitConverts_SubUnitId
        public virtual ICollection<UnitConvert> UnitConverts_UnitId { get; set; } // UnitConverts.FK_UnitConverts_UnitId

        // Foreign keys
        public virtual User User { get; set; } // FK_Units_UserCreatorId

        public Unit()
        {
            IsCurrency = false;
            IsBaseCurrency = false;
            Active = true;
            CreateDate = System.DateTime.Now;
            TransactionItemPrices_PriceUnitId = new List<TransactionItemPrice>();
            TransactionItemPrices_QuantityUnitId = new List<TransactionItemPrice>();
            TransactionItems = new List<TransactionItem>();
            UnitConverts_SubUnitId = new List<UnitConvert>();
            UnitConverts_UnitId = new List<UnitConvert>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

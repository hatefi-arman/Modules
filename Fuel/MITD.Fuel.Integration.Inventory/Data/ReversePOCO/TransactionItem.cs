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
    // TransactionItems
    public partial class TransactionItem
    {
        public int Id { get; set; } // Id (Primary key)
        public short RowVersion { get; set; } // RowVersion
        public int TransactionId { get; set; } // TransactionId
        public int GoodId { get; set; } // GoodId
        public int QuantityUnitId { get; set; } // QuantityUnitId
        public decimal? QuantityAmount { get; set; } // QuantityAmount
        public string Description { get; set; } // Description
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<TransactionItemPrice> TransactionItemPrices { get; set; } // TransactionItemPrices.FK_TransactionItemPrices_TransactionItemsId

        // Foreign keys
        public virtual Good Good { get; set; } // FK_TransactionItems_GoodId
        public virtual Transaction Transaction { get; set; } // FK_TransactionItems_TransactionId
        public virtual Unit Unit { get; set; } // FK_TransactionItems_QuantityUnitId
        public virtual User User { get; set; } // FK_TransactionItems_UserCreatorId

        public TransactionItem()
        {
            QuantityAmount = 0m;
            CreateDate = System.DateTime.Now;
            TransactionItemPrices = new List<TransactionItemPrice>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

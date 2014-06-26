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
    // TransactionItemPrices
    public partial class TransactionItemPrice
    {
        public int Id { get; set; } // Id (Primary key)
        public short RowVersion { get; set; } // RowVersion
        public int TransactionId { get; set; } // TransactionId
        public int TransactionItemId { get; set; } // TransactionItemId
        public string Description { get; set; } // Description
        public int QuantityUnitId { get; set; } // QuantityUnitId
        public decimal? QuantityAmount { get; set; } // QuantityAmount
        public int PriceUnitId { get; set; } // PriceUnitId
        public decimal? Fee { get; set; } // Fee
        public int MainCurrencyUnitId { get; set; } // MainCurrencyUnitId
        public decimal? FeeInMainCurrency { get; set; } // FeeInMainCurrency
        public DateTime? RegistrationDate { get; set; } // RegistrationDate
        public decimal? QuantityAmountUseFifo { get; set; } // QuantityAmountUseFIFO
        public int? TransactionReferenceId { get; set; } // TransactionReferenceId
        public string IssueReferenceIds { get; set; } // IssueReferenceIds
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<TransactionItemPrice> TransactionItemPrices { get; set; } // TransactionItemPrices.FK_TransactionItemPrices_TransactionReferenceId

        // Foreign keys
        public virtual TransactionItem TransactionItem { get; set; } // FK_TransactionItemPrices_TransactionItemsId
        public virtual TransactionItemPrice TransactionItemPrice_TransactionReferenceId { get; set; } // FK_TransactionItemPrices_TransactionReferenceId
        public virtual Unit Unit_PriceUnitId { get; set; } // FK_TransactionItemPrices_PriceUnitId
        public virtual Unit Unit_QuantityUnitId { get; set; } // FK_TransactionItemPrices_QuantityUnitId
        public virtual User User { get; set; } // FK_TransactionItemPrices_UserCreatorId

        public TransactionItemPrice()
        {
            QuantityAmount = 0m;
            Fee = 0m;
            FeeInMainCurrency = 0m;
            RegistrationDate = System.DateTime.Now;
            QuantityAmountUseFifo = 0m;
            IssueReferenceIds = "N''";
            CreateDate = System.DateTime.Now;
            TransactionItemPrices = new List<TransactionItemPrice>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

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
    // Transactions
    public partial class Transaction
    {
        public int Id { get; set; } // Id (Primary key)
        public byte Action { get; set; } // Action
        public decimal? Code { get; set; } // Code
        public string Description { get; set; } // Description
        public int? CrossId { get; set; } // CrossId
        public int WarehouseId { get; set; } // WarehouseId
        public int StoreTypesId { get; set; } // StoreTypesId
        public int TimeBucketId { get; set; } // TimeBucketId
        public byte? Status { get; set; } // Status
        public DateTime? RegistrationDate { get; set; } // RegistrationDate
        public int? SenderReciver { get; set; } // SenderReciver
        public string HardCopyNo { get; set; } // HardCopyNo
        public string ReferenceType { get; set; } // ReferenceType
        public string ReferenceNo { get; set; } // ReferenceNo
        public DateTime? ReferenceDate { get; set; } // ReferenceDate
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<Transaction> Transactions { get; set; } // Transactions.FK_Transaction_CrossId
        public virtual ICollection<TransactionItem> TransactionItems { get; set; } // TransactionItems.FK_TransactionItems_TransactionId

        // Foreign keys
        public virtual StoreType StoreType { get; set; } // FK_Transaction_StoreTypesId
        public virtual Transaction Transaction_CrossId { get; set; } // FK_Transaction_CrossId
        public virtual User User { get; set; } // FK_Transaction_UserCreatorId
        public virtual Warehouse Warehouse { get; set; } // FK_Transaction_WarehouseId

        public Transaction()
        {
            RegistrationDate = System.DateTime.Now;
            ReferenceDate = System.DateTime.Now;
            CreateDate = System.DateTime.Now;
            TransactionItems = new List<TransactionItem>();
            Transactions = new List<Transaction>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

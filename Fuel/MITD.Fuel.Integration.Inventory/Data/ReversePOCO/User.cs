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
    // Users
    public partial class User
    {
        public int Id { get; set; } // Id (Primary key)
        public int Code { get; set; } // Code
        public string Name { get; set; } // Name
        public string UserName { get; set; } // User_Name
        public string Password { get; set; } // Password
        public bool? Active { get; set; } // Active
        public string EmailAddress { get; set; } // Email_Address
        public string IpAddress { get; set; } // IPAddress
        public bool? Login { get; set; } // Login
        public string SessionId { get; set; } // SessionId
        public int? UserCreatorId { get; set; } // UserCreatorId
        public DateTime? CreateDate { get; set; } // CreateDate

        // Reverse navigation
        public virtual ICollection<Company> Companies { get; set; } // Companies.FK_Companies_UserCreatorId
        public virtual ICollection<FinancialYear> FinancialYears { get; set; } // FinancialYear.FK_FinancialYear_UserCreatorId
        public virtual ICollection<Good> Goods { get; set; } // Goods.FK_Goods_UserCreatorId
        public virtual ICollection<StoreType> StoreTypes { get; set; } // StoreTypes.FK_StoreTypes_UserCreatorId
        public virtual ICollection<TimeBucket> TimeBuckets { get; set; } // TimeBucket.FK_TimeBucket_UserCreatorId
        public virtual ICollection<Transaction> Transactions { get; set; } // Transactions.FK_Transaction_UserCreatorId
        public virtual ICollection<TransactionItem> TransactionItems { get; set; } // TransactionItems.FK_TransactionItems_UserCreatorId
        public virtual ICollection<TransactionItemPrice> TransactionItemPrices { get; set; } // TransactionItemPrices.FK_TransactionItemPrices_UserCreatorId
        public virtual ICollection<Unit> Units { get; set; } // Units.FK_Units_UserCreatorId
        public virtual ICollection<User> Users { get; set; } // Users.FK_Users_UserCreatorId
        public virtual ICollection<Warehouse> Warehouses { get; set; } // Warehouse.FK_Warehouse_UserCreatorId

        // Foreign keys
        public virtual User User_UserCreatorId { get; set; } // FK_Users_UserCreatorId

        public User()
        {
            Active = true;
            Login = false;
            CreateDate = System.DateTime.Now;
            Companies = new List<Company>();
            FinancialYears = new List<FinancialYear>();
            Goods = new List<Good>();
            StoreTypes = new List<StoreType>();
            TimeBuckets = new List<TimeBucket>();
            TransactionItemPrices = new List<TransactionItemPrice>();
            TransactionItems = new List<TransactionItem>();
            Transactions = new List<Transaction>();
            Units = new List<Unit>();
            Users = new List<User>();
            Warehouses = new List<Warehouse>();
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

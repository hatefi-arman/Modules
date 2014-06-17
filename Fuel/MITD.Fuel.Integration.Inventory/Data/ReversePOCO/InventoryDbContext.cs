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
    public partial class InventoryDbContext : DbContext, IInventoryDbContext
    {
        public IDbSet<Company> Companies { get; set; } // Companies
        public IDbSet<ErrorMessage> ErrorMessages { get; set; } // ErrorMessages
        public IDbSet<FinancialYear> FinancialYears { get; set; } // FinancialYear
        public IDbSet<Good> Goods { get; set; } // Goods
        public IDbSet<OperationReference> OperationReferences { get; set; } // OperationReference
        public IDbSet<StoreType> StoreTypes { get; set; } // StoreTypes
        public IDbSet<TimeBucket> TimeBuckets { get; set; } // TimeBucket
        public IDbSet<Transaction> Transactions { get; set; } // Transactions
        public IDbSet<TransactionItem> TransactionItems { get; set; } // TransactionItems
        public IDbSet<TransactionItemPrice> TransactionItemPrices { get; set; } // TransactionItemPrices
        public IDbSet<Unit> Units { get; set; } // Units
        public IDbSet<UnitConvert> UnitConverts { get; set; } // UnitConverts
        public IDbSet<User> Users { get; set; } // Users
        public IDbSet<Warehouse> Warehouses { get; set; } // Warehouse

        static InventoryDbContext()
        {
            Database.SetInitializer<InventoryDbContext>(null);
        }

        public InventoryDbContext()
            : base("Name=MITDInventory")
        {
        InitializePartial();
        }

        public InventoryDbContext(string connectionString) : base(connectionString)
        {
        InitializePartial();
        }

        public InventoryDbContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model) : base(connectionString, model)
        {
        InitializePartial();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new CompanyConfiguration());
            modelBuilder.Configurations.Add(new ErrorMessageConfiguration());
            modelBuilder.Configurations.Add(new FinancialYearConfiguration());
            modelBuilder.Configurations.Add(new GoodConfiguration());
            modelBuilder.Configurations.Add(new OperationReferenceConfiguration());
            modelBuilder.Configurations.Add(new StoreTypeConfiguration());
            modelBuilder.Configurations.Add(new TimeBucketConfiguration());
            modelBuilder.Configurations.Add(new TransactionConfiguration());
            modelBuilder.Configurations.Add(new TransactionItemConfiguration());
            modelBuilder.Configurations.Add(new TransactionItemPriceConfiguration());
            modelBuilder.Configurations.Add(new UnitConfiguration());
            modelBuilder.Configurations.Add(new UnitConvertConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new WarehouseConfiguration());
        OnModelCreatingPartial(modelBuilder);
        }

        public static DbModelBuilder CreateModel(DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new CompanyConfiguration(schema));
            modelBuilder.Configurations.Add(new ErrorMessageConfiguration(schema));
            modelBuilder.Configurations.Add(new FinancialYearConfiguration(schema));
            modelBuilder.Configurations.Add(new GoodConfiguration(schema));
            modelBuilder.Configurations.Add(new OperationReferenceConfiguration(schema));
            modelBuilder.Configurations.Add(new StoreTypeConfiguration(schema));
            modelBuilder.Configurations.Add(new TimeBucketConfiguration(schema));
            modelBuilder.Configurations.Add(new TransactionConfiguration(schema));
            modelBuilder.Configurations.Add(new TransactionItemConfiguration(schema));
            modelBuilder.Configurations.Add(new TransactionItemPriceConfiguration(schema));
            modelBuilder.Configurations.Add(new UnitConfiguration(schema));
            modelBuilder.Configurations.Add(new UnitConvertConfiguration(schema));
            modelBuilder.Configurations.Add(new UserConfiguration(schema));
            modelBuilder.Configurations.Add(new WarehouseConfiguration(schema));
            return modelBuilder;
        }

        partial void InitializePartial();
        partial void OnModelCreatingPartial(DbModelBuilder modelBuilder);
    }
}

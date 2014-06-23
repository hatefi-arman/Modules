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
    internal partial class TransactionConfiguration : EntityTypeConfiguration<Transaction>
    {
        public TransactionConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".Transactions");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Action).HasColumnName("Action").IsRequired();
            Property(x => x.Code).HasColumnName("Code").IsOptional().HasPrecision(20,2);
            Property(x => x.Description).HasColumnName("Description").IsOptional();
            Property(x => x.CrossId).HasColumnName("CrossId").IsOptional();
            Property(x => x.WarehouseId).HasColumnName("WarehouseId").IsRequired();
            Property(x => x.StoreTypesId).HasColumnName("StoreTypesId").IsRequired();
            Property(x => x.TimeBucketId).HasColumnName("TimeBucketId").IsRequired();
            Property(x => x.PricingReferenceId).HasColumnName("PricingReferenceId").IsOptional();
            Property(x => x.Status).HasColumnName("Status").IsOptional();
            Property(x => x.RegistrationDate).HasColumnName("RegistrationDate").IsOptional();
            Property(x => x.SenderReciver).HasColumnName("SenderReciver").IsOptional();
            Property(x => x.HardCopyNo).HasColumnName("HardCopyNo").IsOptional().HasMaxLength(10);
            Property(x => x.ReferenceType).HasColumnName("ReferenceType").IsOptional().HasMaxLength(100);
            Property(x => x.ReferenceNo).HasColumnName("ReferenceNo").IsOptional().HasMaxLength(100);
            Property(x => x.ReferenceDate).HasColumnName("ReferenceDate").IsOptional();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasOptional(a => a.Transaction_CrossId).WithMany(b => b.Transactions).HasForeignKey(c => c.CrossId); // FK_Transaction_CrossId
            HasRequired(a => a.Warehouse).WithMany(b => b.Transactions).HasForeignKey(c => c.WarehouseId); // FK_Transaction_WarehouseId
            HasRequired(a => a.StoreType).WithMany(b => b.Transactions).HasForeignKey(c => c.StoreTypesId); // FK_Transaction_StoreTypesId
            HasOptional(a => a.User).WithMany(b => b.Transactions).HasForeignKey(c => c.UserCreatorId); // FK_Transaction_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

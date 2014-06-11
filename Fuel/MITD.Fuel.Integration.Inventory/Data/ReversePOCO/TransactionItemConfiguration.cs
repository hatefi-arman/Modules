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
    internal partial class TransactionItemConfiguration : EntityTypeConfiguration<TransactionItem>
    {
        public TransactionItemConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TransactionItems");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.RowVersion).HasColumnName("RowVersion").IsRequired();
            Property(x => x.TransactionId).HasColumnName("TransactionId").IsRequired();
            Property(x => x.GoodId).HasColumnName("GoodId").IsRequired();
            Property(x => x.QuantityUnitId).HasColumnName("QuantityUnitId").IsRequired();
            Property(x => x.QuantityAmount).HasColumnName("QuantityAmount").IsOptional().HasPrecision(20,3);
            Property(x => x.Description).HasColumnName("Description").IsOptional();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasRequired(a => a.Transaction).WithMany(b => b.TransactionItems).HasForeignKey(c => c.TransactionId); // FK_TransactionItems_TransactionId
            HasRequired(a => a.Good).WithMany(b => b.TransactionItems).HasForeignKey(c => c.GoodId); // FK_TransactionItems_GoodId
            HasRequired(a => a.Unit).WithMany(b => b.TransactionItems).HasForeignKey(c => c.QuantityUnitId); // FK_TransactionItems_QuantityUnitId
            HasOptional(a => a.User).WithMany(b => b.TransactionItems).HasForeignKey(c => c.UserCreatorId); // FK_TransactionItems_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

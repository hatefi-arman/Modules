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
    internal partial class TransactionItemPriceConfiguration : EntityTypeConfiguration<TransactionItemPrice>
    {
        public TransactionItemPriceConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TransactionItemPrices");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.RowVersion).HasColumnName("RowVersion").IsRequired();
            Property(x => x.TransactionId).HasColumnName("TransactionId").IsRequired();
            Property(x => x.TransactionItemId).HasColumnName("TransactionItemId").IsRequired();
            Property(x => x.Description).HasColumnName("Description").IsOptional();
            Property(x => x.QuantityUnitId).HasColumnName("QuantityUnitId").IsRequired();
            Property(x => x.QuantityAmount).HasColumnName("QuantityAmount").IsOptional().HasPrecision(20,3);
            Property(x => x.PriceUnitId).HasColumnName("PriceUnitId").IsRequired();
            Property(x => x.Fee).HasColumnName("Fee").IsOptional().HasPrecision(20,3);
            Property(x => x.MainCurrencyUnitId).HasColumnName("MainCurrencyUnitId").IsRequired();
            Property(x => x.FeeInMainCurrency).HasColumnName("FeeInMainCurrency").IsOptional().HasPrecision(20,3);
            Property(x => x.RegistrationDate).HasColumnName("RegistrationDate").IsOptional();
            Property(x => x.QuantityAmountUseFifo).HasColumnName("QuantityAmountUseFIFO").IsOptional().HasPrecision(20,3);
            Property(x => x.TransactionReferenceId).HasColumnName("TransactionReferenceId").IsOptional();
            Property(x => x.IssueReferenceIds).HasColumnName("IssueReferenceIds").IsOptional();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasRequired(a => a.TransactionItem).WithMany(b => b.TransactionItemPrices).HasForeignKey(c => c.TransactionItemId); // FK_TransactionItemPrices_TransactionItemsId
            HasRequired(a => a.Unit_QuantityUnitId).WithMany(b => b.TransactionItemPrices_QuantityUnitId).HasForeignKey(c => c.QuantityUnitId); // FK_TransactionItemPrices_QuantityUnitId
            HasRequired(a => a.Unit_PriceUnitId).WithMany(b => b.TransactionItemPrices_PriceUnitId).HasForeignKey(c => c.PriceUnitId); // FK_TransactionItemPrices_PriceUnitId
            HasOptional(a => a.TransactionItemPrice_TransactionReferenceId).WithMany(b => b.TransactionItemPrices).HasForeignKey(c => c.TransactionReferenceId); // FK_TransactionItemPrices_TransactionReferenceId
            HasOptional(a => a.User).WithMany(b => b.TransactionItemPrices).HasForeignKey(c => c.UserCreatorId); // FK_TransactionItemPrices_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

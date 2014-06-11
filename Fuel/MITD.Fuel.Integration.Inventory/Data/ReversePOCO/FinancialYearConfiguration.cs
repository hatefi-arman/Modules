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
    // FinancialYear
    internal partial class FinancialYearConfiguration : EntityTypeConfiguration<FinancialYear>
    {
        public FinancialYearConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".FinancialYear");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            Property(x => x.StartDate).HasColumnName("StartDate").IsRequired();
            Property(x => x.EndDate).HasColumnName("EndDate").IsRequired();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsRequired();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasRequired(a => a.User).WithMany(b => b.FinancialYears).HasForeignKey(c => c.UserCreatorId); // FK_FinancialYear_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

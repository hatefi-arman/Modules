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
    // TimeBucket
    internal partial class TimeBucketConfiguration : EntityTypeConfiguration<TimeBucket>
    {
        public TimeBucketConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".TimeBucket");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(256);
            Property(x => x.StartDate).HasColumnName("StartDate").IsOptional();
            Property(x => x.EndDate).HasColumnName("EndDate").IsOptional();
            Property(x => x.FinancialYearId).HasColumnName("FinancialYearId").IsRequired();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsRequired();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();
            Property(x => x.Active).HasColumnName("Active").IsOptional();

            // Foreign keys
            HasRequired(a => a.FinancialYear).WithMany(b => b.TimeBuckets).HasForeignKey(c => c.FinancialYearId); // FK_TimeBucket_FinancialYearId
            HasRequired(a => a.User).WithMany(b => b.TimeBuckets).HasForeignKey(c => c.UserCreatorId); // FK_TimeBucket_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

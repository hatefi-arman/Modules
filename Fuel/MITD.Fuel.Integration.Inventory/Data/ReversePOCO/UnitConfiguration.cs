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
    // Units
    internal partial class UnitConfiguration : EntityTypeConfiguration<Unit>
    {
        public UnitConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".Units");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName("Code").IsRequired().HasMaxLength(15);
            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            Property(x => x.IsCurrency).HasColumnName("IsCurrency").IsOptional();
            Property(x => x.IsBaseCurrency).HasColumnName("IsBaseCurrency").IsOptional();
            Property(x => x.Active).HasColumnName("Active").IsRequired();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasOptional(a => a.User).WithMany(b => b.Units).HasForeignKey(c => c.UserCreatorId); // FK_Units_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

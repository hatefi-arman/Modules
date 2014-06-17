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
    // StoreTypes
    internal partial class StoreTypeConfiguration : EntityTypeConfiguration<StoreType>
    {
        public StoreTypeConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".StoreTypes");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName("Code").IsRequired();
            Property(x => x.Type).HasColumnName("Type").IsRequired();
            Property(x => x.InputName).HasColumnName("InputName").IsOptional().HasMaxLength(100);
            Property(x => x.OutputName).HasColumnName("OutputName").IsOptional().HasMaxLength(100);
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasOptional(a => a.User).WithMany(b => b.StoreTypes).HasForeignKey(c => c.UserCreatorId); // FK_StoreTypes_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

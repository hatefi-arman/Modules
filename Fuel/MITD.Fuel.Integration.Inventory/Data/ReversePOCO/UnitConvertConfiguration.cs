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
    // UnitConverts
    internal partial class UnitConvertConfiguration : EntityTypeConfiguration<UnitConvert>
    {
        public UnitConvertConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".UnitConverts");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UnitId).HasColumnName("UnitId").IsRequired();
            Property(x => x.SubUnitId).HasColumnName("SubUnitId").IsRequired();
            Property(x => x.Coefficient).HasColumnName("Coefficient").IsRequired().HasPrecision(18,3);
            Property(x => x.EffectiveDate).HasColumnName("EffectiveDate").IsOptional();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsRequired();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasRequired(a => a.Unit_UnitId).WithMany(b => b.UnitConverts_UnitId).HasForeignKey(c => c.UnitId); // FK_UnitConverts_UnitId
            HasRequired(a => a.Unit_SubUnitId).WithMany(b => b.UnitConverts_SubUnitId).HasForeignKey(c => c.SubUnitId); // FK_UnitConverts_SubUnitId
            HasRequired(a => a.Unit_UserCreatorId).WithMany(b => b.UnitConverts_UserCreatorId).HasForeignKey(c => c.UserCreatorId); // FK_UnitConverts_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

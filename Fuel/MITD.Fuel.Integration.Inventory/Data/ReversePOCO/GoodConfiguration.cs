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
    // Goods
    internal partial class GoodConfiguration : EntityTypeConfiguration<Good>
    {
        public GoodConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".Goods");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Code).HasColumnName("Code").IsRequired().HasMaxLength(100);
            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(200);
            Property(x => x.Active).HasColumnName("Active").IsRequired();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasOptional(a => a.User).WithMany(b => b.Goods).HasForeignKey(c => c.UserCreatorId); // FK_Goods_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

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
    // Warehouse
    internal partial class WarehouseConfiguration : EntityTypeConfiguration<Warehouse>
    {
        public WarehouseConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".Warehouse");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Code).HasColumnName("Code").IsRequired().HasMaxLength(10);
            Property(x => x.Name).HasColumnName("Name").IsOptional().HasMaxLength(256);
            Property(x => x.CompanyId).HasColumnName("CompanyId").IsRequired();
            Property(x => x.Active).HasColumnName("Active").IsOptional();
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasRequired(a => a.Company).WithMany(b => b.Warehouses).HasForeignKey(c => c.CompanyId); // FK_Warehouse_CompanyId
            HasOptional(a => a.User).WithMany(b => b.Warehouses).HasForeignKey(c => c.UserCreatorId); // FK_Warehouse_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
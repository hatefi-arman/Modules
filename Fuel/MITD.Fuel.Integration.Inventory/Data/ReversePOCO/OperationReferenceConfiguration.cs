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
    // OperationReference
    internal partial class OperationReferenceConfiguration : EntityTypeConfiguration<OperationReference>
    {
        public OperationReferenceConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".OperationReference");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OperationId).HasColumnName("OperationId").IsRequired();
            Property(x => x.OperationType).HasColumnName("OperationType").IsRequired();
            Property(x => x.ReferenceType).HasColumnName("ReferenceType").IsRequired().HasMaxLength(512);
            Property(x => x.ReferenceNumber).HasColumnName("ReferenceNumber").IsRequired().HasMaxLength(256);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

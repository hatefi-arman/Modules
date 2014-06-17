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
    // ErrorMessages
    internal partial class ErrorMessageConfiguration : EntityTypeConfiguration<ErrorMessage>
    {
        public ErrorMessageConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".ErrorMessages");
            HasKey(x => new { x.ErrorMessage_, x.TextMessage, x.Action });

            Property(x => x.ErrorMessage_).HasColumnName("ErrorMessage").IsRequired().HasMaxLength(200).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.TextMessage).HasColumnName("TextMessage").IsRequired().HasMaxLength(200).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Action).HasColumnName("Action").IsRequired().HasMaxLength(20).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

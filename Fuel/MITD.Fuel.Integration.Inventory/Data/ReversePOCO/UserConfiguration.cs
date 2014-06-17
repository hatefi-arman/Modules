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
    // Users
    internal partial class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration(string schema = "dbo")
        {
            ToTable(schema + ".Users");
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Code).HasColumnName("Code").IsRequired();
            Property(x => x.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
            Property(x => x.UserName).HasColumnName("User_Name").IsRequired().HasMaxLength(256);
            Property(x => x.Password).HasColumnName("Password").IsRequired().HasMaxLength(100);
            Property(x => x.Active).HasColumnName("Active").IsOptional();
            Property(x => x.EmailAddress).HasColumnName("Email_Address").IsOptional().HasMaxLength(256);
            Property(x => x.IpAddress).HasColumnName("IPAddress").IsOptional().HasMaxLength(15);
            Property(x => x.Login).HasColumnName("Login").IsOptional();
            Property(x => x.SessionId).HasColumnName("SessionId").IsOptional().HasMaxLength(88);
            Property(x => x.UserCreatorId).HasColumnName("UserCreatorId").IsOptional();
            Property(x => x.CreateDate).HasColumnName("CreateDate").IsOptional();

            // Foreign keys
            HasOptional(a => a.User_UserCreatorId).WithMany(b => b.Users).HasForeignKey(c => c.UserCreatorId); // FK_Users_UserCreatorId
            InitializePartial();
        }
        partial void InitializePartial();
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;

namespace MITD.Fuel.Data.EF.Configurations.Financial
{
   public class JournalEntryConfiguration:EntityTypeConfiguration<JournalEntry>
   {
       public JournalEntryConfiguration()
       {
           ToTable("JournalEntries", "Fuel").HasKey(c => c.Id);
           Property(c => c.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
           HasOptional(c=>c.Segment).WithMany().HasForeignKey(c=>c.Segment.Id).WillCascadeOnDelete(false);
           HasOptional(c => c.Voucher).WithMany().HasForeignKey(c => c.VoucherId).WillCascadeOnDelete(false);
           Property(c => c.TimeStamp).IsRowVersion();

       }
    }
}

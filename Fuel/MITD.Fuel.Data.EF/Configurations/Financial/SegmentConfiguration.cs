using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;

namespace MITD.Fuel.Data.EF.Configurations.Financial
{
    public class SegmentConfiguration : EntityTypeConfiguration<Segment>
    {
        public SegmentConfiguration()
        {
            ToTable("Segments", "Fuel")
                .HasKey(c => c.Id);
            Property(c => c.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Ignore(c => c.SegmentType);
            Property(c => c.TimeStamp).IsRowVersion();
        }
    }
}

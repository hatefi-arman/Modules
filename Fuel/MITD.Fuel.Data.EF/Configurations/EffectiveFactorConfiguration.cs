using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MITD.Fuel.Domain.Model.DomainObjects;

namespace MITD.Fuel.Data.EF.Configurations
{
    public class EffectiveFactorConfiguration : EntityTypeConfiguration<EffectiveFactor>
    {
        public EffectiveFactorConfiguration()
        {
            this.HasKey(p => p.Id).ToTable("EffectiveFactor", "Fuel");
            this.Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.TimeStamp).IsRowVersion();

            this.Property(c => c.Name).HasMaxLength(200);
            this.Property(c => c.EffectiveFactorType);
        }
    }
}
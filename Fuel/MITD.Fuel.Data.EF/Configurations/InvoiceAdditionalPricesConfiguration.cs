using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;


namespace MITD.Fuel.Data.EF.Configurations
{
    public class InvoiceAdditionalPricesConfiguration : EntityTypeConfiguration<InvoiceAdditionalPrice>
    {
        public InvoiceAdditionalPricesConfiguration()
        {
            HasKey(p => p.Id).ToTable("InvoiceAdditionalPrices", "Fuel");
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.TimeStamp).IsRowVersion();

            HasRequired(c => c.Invoice).WithMany(c => c.AdditionalPrices).HasForeignKey(c => c.InvoiceId);
            HasRequired(c => c.EffectiveFactor).WithMany().HasForeignKey(c => c.EffectiveFactorId);
        }
    }
}
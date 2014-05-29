using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;


namespace MITD.Fuel.Data.EF.Configurations
{
    public class OrderItemBalanceConfiguration : EntityTypeConfiguration<OrderItemBalance>
    {
        public OrderItemBalanceConfiguration()
        {
            HasKey(p => p.Id)
                .ToTable("OrderItemBalances", "Fuel");
            Property(p => p.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.TimeStamp).IsRowVersion();

            Property(p => p.QuantityAmountInMainUnit);
            Property(p => p.UnitCode).HasMaxLength(50);

            Ignore(p => p.Quantity);
            HasRequired(c => c.OrderItem)
                .WithMany(c => c.OrderItemBalances)
                .HasForeignKey(c => c.OrderItemId)
                .WillCascadeOnDelete(false);

            HasRequired(c => c.FuelReportDetail)
                .WithMany()
                .HasForeignKey(c => c.FuelReportDetailId)
                .WillCascadeOnDelete(false);

            HasRequired(c => c.InvoiceItem)
                .WithMany()
                .HasForeignKey(c => c.InvoiceItemId)
                .WillCascadeOnDelete(false);

        }
    }
}
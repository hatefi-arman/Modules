using System.Data.Entity.ModelConfiguration;
using MITD.FuelSecurity.Domain.Model;

namespace MITD.Fuel.Data.EF.Configurations.Security
{
    public class GroupConfiguration : EntityTypeConfiguration<Group>
    {
        public GroupConfiguration()
        {
            ToTable("Groups");
            Property(p => p.Id).HasColumnName("Id");
            Property(p => p.Description);
        }
    }
}
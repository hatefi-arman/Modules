using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Integration.Offhire.Data
{
    public class OffhireSystemToFuelSystemMappingDataContext : DbContext
    {
        public virtual DbSet<OffhireFureTypeFuelGoodCode> OffhireFureTypeFuelGoodCodes { get; set; }

        public virtual DbSet<OffhireMeasureTypeFuelMeasureCode> OffhireMeasureTypeFuelMeasureCodes { get; set; }

        public OffhireSystemToFuelSystemMappingDataContext()
            : base("name=DataContainer")
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;

             base.Database.Initialize(true);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new OffhireFureTypeFuelGoodCodeConfiguration());
            modelBuilder.Configurations.Add(new OffhireMeasureTypeFuelMeasureCodeConfiguration());
        }
    }

    public class OffhireFureTypeFuelGoodCode
    {
        public long Id { get; set; }
        public string OffhireFuelType { get; set; }
        public string FuelGoodCode { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
    }

    public class OffhireMeasureTypeFuelMeasureCode
    {
        public long Id { get; set; }
        public string OffhireMeasureType { get; set; }
        public string FuelMeasureCode { get; set; }
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
    }


    public class OffhireFureTypeFuelGoodCodeConfiguration : EntityTypeConfiguration<OffhireFureTypeFuelGoodCode>
    {
        public OffhireFureTypeFuelGoodCodeConfiguration()
        {
            HasKey(k=>k.Id).ToTable("OffhireFureTypeFuelGoodCode", "Offhire");

        }
    }

    public class OffhireMeasureTypeFuelMeasureCodeConfiguration : EntityTypeConfiguration<OffhireMeasureTypeFuelMeasureCode>
    {
        public OffhireMeasureTypeFuelMeasureCodeConfiguration()
        {
            HasKey(k => k.Id).ToTable("OffhireMeasureTypeFuelMeasureCode", "Offhire");

        }
    }
}

using System.Data.Entity;
using MITD.Fuel.Data.EF.Configurations;
using MITD.Fuel.Data.EF.Configurations.BaseInfo;
using MITD.Fuel.Data.EF.Configurations.Security;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.FuelSecurity.Domain.Model;
using User = MITD.FuelSecurity.Domain.Model.User;
using UserConfiguration = MITD.Fuel.Data.EF.Configurations.Security.UserConfiguration;


namespace MITD.Fuel.Data.EF.Context
{
    public class DataContainer : DbContext
    {
        #region Constructors

        public DataContainer() :
            base("Name=DataContainer")
        {
            Configure();
        }

        /*static DataContainer()
        {
            Database.SetInitializer(new DropCreateDatabaseWhenChanges());
        }*/
        public DataContainer(string connectionString) :
            base(connectionString)
        {
            Configure();
        }

        private void Configure()
        {
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;
        }

        #endregion

        public DbSet<Charter> Charters { get; set; }

        public DbSet<CharterIn> CharterIns { get; set; }

        public DbSet<CharterOut> CharterOuts { get; set; }

        public DbSet<CharterItem> CharterItems { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Vessel> Vessels { get; set; }

        public DbSet<VesselInCompany> VesselsInCompanies { get; set; }
        public DbSet<VesselInInventory> VesselsInInventory { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Invoice> Invoices { get; set; }

        public DbSet<Voyage> Voyages { get; set; }
        public DbSet<VoyageLog> VoyagesLog { get; set; }

        public DbSet<FuelReport> FuelReports { get; set; }

        public DbSet<FuelReportDetail> FuelReportDetails { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Good> Goods { get; set; }
        public DbSet<SharedGood> SharedGoods { get; set; }

     

        public DbSet<GoodUnit> GoodUnits { get; set; }
        public DbSet<Unit> Units { get; set; }

        // public DbSet<GoodPartyAssignment> GoodPartyAssignments { get; set; }

        public DbSet<Tank> Tanks { get; set; }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<WorkflowStep> ApproveFlows { get; set; }

        public DbSet<WorkflowLog> ApproveFlowWorks { get; set; }

        public DbSet<InventoryOperation> InventoryOperations { get; set; }

        public DbSet<Scrap> Scraps { get; set; }
        public DbSet<ScrapDetail> ScrapDetails { get; set; }

        public DbSet<EffectiveFactor> EffectiveFactors { get; set; }
        public DbSet<OrderItemBalance> OrderItemBalance { get; set; }

        public DbSet<Offhire> Offhires { get; set; }
        public DbSet<OffhireDetail> OffhireDetails { get; set; }

        public DbSet<ActivityLocation> ActivityLocations { get; set; }

        #region Security

        public DbSet<Party> Parties { get; set; }

        public DbSet<ActionType> ActionTypes { get; set; }

        public DbSet<PartyCustomAction> PartyCustomActions { get; set; }

        public DbSet<User> Users { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ComplexType<OffhirePricingReference>();

            modelBuilder.ComplexType<Reference>();

            #region Basic Info

            modelBuilder.Configurations.Add(new CompanyConfiguration());

            //modelBuilder.Configurations.Add(new GoodPartyAssignmentConfiguration());

            modelBuilder.Configurations.Add(new GoodConfiguration());
            modelBuilder.Configurations.Add(new SharedGoodConfiguration());
            modelBuilder.Configurations.Add(new CompanyGoodUnitConfiguration());
            modelBuilder.Configurations.Add(new UnitConfiguration());
            modelBuilder.Configurations.Add(new CurrencyConfiguration());
            modelBuilder.Configurations.Add(new TankConfiguration());
            modelBuilder.Configurations.Add(new ActivityLocationConfiguration());

            #endregion

            #region Security - Identity

            modelBuilder.Configurations.Add(new PartyConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            modelBuilder.Configurations.Add(new GroupConfiguration());
            modelBuilder.Ignore<AdminUser>();
            modelBuilder.Ignore<CommercialUser>();
            modelBuilder.Ignore<FinancialUser>();

            modelBuilder.Configurations.Add(new ActionTypeConfiguration());

            modelBuilder.Configurations.Add(new PartyCustomActionConfiguration());

            #endregion


            modelBuilder.Configurations.Add(new VesselConfiguration());
            modelBuilder.Configurations.Add(new VesselInCompanyConfiguration());
            modelBuilder.Configurations.Add(new VesselInInventoryConfiguration());


            modelBuilder.Configurations.Add(new OrderConfiguration());
            modelBuilder.Configurations.Add(new OrderItemConfiguration());
            modelBuilder.Configurations.Add(new FuelReportConfiguration());
            modelBuilder.Configurations.Add(new FuelReportDetailConfiguration());
            modelBuilder.Configurations.Add(new VoyageConfiguration());
            modelBuilder.Configurations.Add(new VoyageLogConfiguration());
            modelBuilder.Configurations.Add(new ApproveWorkFlowConfigConfiguration());
            modelBuilder.Configurations.Add(new WorkflowLogConfiguration());
            modelBuilder.Configurations.Add(new OrderApproveWorkFlowConfiguration());
            modelBuilder.Configurations.Add(new FuelReportWorkflowLogConfiguration());
            modelBuilder.Configurations.Add(new InventoryOperationConfiguration());
            modelBuilder.Configurations.Add(new InvoiceConfiguration());
            modelBuilder.Configurations.Add(new InvoiceItemConfiguration());
            modelBuilder.Configurations.Add(new InvoiceAdditionalPricesConfiguration());
            modelBuilder.Configurations.Add(new EffectiveFactorConfiguration());
            modelBuilder.Configurations.Add(new InvoiceApproveWorkFlowConfiguration());
            modelBuilder.Configurations.Add(new ScrapConfiguration());
            modelBuilder.Configurations.Add(new ScrapDetailConfiguration());
            modelBuilder.Configurations.Add(new ScrapWorkflowLogConfiguration());
            modelBuilder.Configurations.Add(new CharterInConfiguration());
            modelBuilder.Configurations.Add(new CharterOutConfiguration());
            modelBuilder.Configurations.Add(new CharterItemConfiguration());
            modelBuilder.Configurations.Add(new OrderItemBalanceConfiguration());
            modelBuilder.Configurations.Add(new CharterConfiguration());
            modelBuilder.Configurations.Add(new CharterApproveWorkFlowConfiguration());
            modelBuilder.Configurations.Add(new OffhireConfiguration());
            modelBuilder.Configurations.Add(new OffhireDetailConfiguration());
            modelBuilder.Configurations.Add(new OffhireWorkflowLogConfiguration());
        }
    }
}
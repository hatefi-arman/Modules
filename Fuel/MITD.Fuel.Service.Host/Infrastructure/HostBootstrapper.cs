using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Releasers;
using Castle.Windsor;
using MITD.Core;
using MITD.Core.Config;
using MITD.DataAccess.Config;
using MITD.DataAccess.EF;
using MITD.Domain.Model;
using MITD.Domain.Repository;
using MITD.Fuel.ACL.Contracts.AutomaticVoucher;
using MITD.Fuel.Application;
using MITD.Fuel.Application.Facade;
using MITD.Fuel.Application.Service.Security;
using MITD.Fuel.Domain.Model.Factories;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Infrastructure.Service;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Fuel.Service.Host.App_Start;
using MITD.FuelSecurity.Domain.Model.Service;
using MITD.Services.AntiCorruption.Contracts;
using MITD.Services.Application;
using MITD.Fuel.Data.EF.Context;
using MITD.Fuel.Domain.Model.IDomainServices.Events;
using MITD.Fuel.Integration.Inventory;
using TIBA.Core;
using EventPublisher = MITD.Core.EventPublisher;
using IEventPublisher = MITD.Core.IEventPublisher;
using IFacadeService = MITD.Services.Facade.IFacadeService;
using IMapper = MITD.Core.IMapper;
using ServiceLocator = MITD.Core.ServiceLocator;

namespace MITD.Fuel.Service.Host.Infrastructure
{
    public class HostBootstrapper : IBootstrapper
    {
        public void Execute()
        {


            #region create WindsorContainer and Locator

            var container = new WindsorContainer();
            container.Kernel.ReleasePolicy = new NoTrackingReleasePolicy();





            container.Register(Component.For<IWindsorContainer>().Instance(container).LifestyleSingleton());
            var serviceLocator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            #endregion

            #region register areas, filters, routes, boundles
            AreaRegistration.RegisterAllAreas();

            container.Register(
              Component.For<IApplicationLogger>()
              .ImplementedBy<ApplicationLogger>().
                  LifestyleSingleton());

            container.Register(
                Component.For<IFuelApplicationExceptionAdapter>()
                .ImplementedBy<FuelApplicationExceptionAdapter>().
                    LifestyleSingleton());

            container.Register(
                        Component.For<GlobalExceptionHandlingAttribute>()
                        .ImplementedBy<GlobalExceptionHandlingAttribute>().
                            LifestyleSingleton());


            #endregion

            #region register data container and repositories
            DataAccessConfigHelper.ConfigureContainer<PerHttpContextUnitOfWorkScope, DataContainer>(container,
               () =>
               {
                   var connectionString = ConfigurationManager.ConnectionStrings["DataContainer"];
                   var ctx = new DataContainer(connectionString.ConnectionString);
                   return ctx;
               });

            #endregion


            #region register MVC

            //register api controllers 
            container.Register(Classes.FromThisAssembly()
                                   .BasedOn<ApiController>()
                                   .WithService.Self()
                                   .LifestyleTransient());
            //register controllers 
            container.Register(Classes.FromThisAssembly()
                                   .BasedOn<Controller>()
                                   .WithService.Self()
                                   .LifestyleTransient());
            #endregion


            #region register IdGenerator

            //container.Register(Component.For<IOrderFactory>().ImplementedBy<OrderFactory>().LifestyleSingleton());


            #endregion


            var fromAssemblyDescriptor = Classes.FromAssemblyInDirectory(new AssemblyFilter("bin", "*Fuel*"));


            // factory
            container.Register(fromAssemblyDescriptor
                       .BasedOn(typeof(IFactory))
                       .WithService.FromInterface()
                       .LifestyleTransient());


            container.Register(fromAssemblyDescriptor
                       .BasedOn(typeof(ICodeGenerator))
                       .WithService.FromInterface()
                       .LifestyleTransient());

            container.Register(fromAssemblyDescriptor
                       .BasedOn(typeof(IEntityConfigurator<>))
                       .WithService.FromInterface()
                       .LifestyleTransient());

            // eventNotifier
            container.Register(fromAssemblyDescriptor
                       .BasedOn(typeof(IEventNotifier))
                       .WithService.FromInterface()
                       .LifestyleTransient());

            #region register AntiCorruption
            //adapters
            container.Register(fromAssemblyDescriptor
                                   .BasedOn(typeof(IAntiCorruptionAdapter))
                                   .WithService.FromInterface()
                                   .LifestyleTransient());

            //ServiceWrapper
            container.Register(fromAssemblyDescriptor
                                  .BasedOn(typeof(IAntiCorruptionServiceWrapper))
                                   .WithServiceAllInterfaces()
                // .WithService.FromInterface()
                                   .LifestyleTransient());

            //mappers
            container.Register(fromAssemblyDescriptor
                                   .BasedOn(typeof(IMapper))
                                      .WithService.FromInterface()
                                   .LifestyleTransient());

            #endregion


            #region register Application

            container.Register(fromAssemblyDescriptor
                                 .BasedOn<IApplicationService>()
                                 .WithServiceFromInterface()
                                 .LifestyleTransient());


            #endregion


            #region register Facade

            container.Register(

                fromAssemblyDescriptor
                    .BasedOn(typeof(IFacadeService))
                    .WithServiceFromInterface().LifestyleTransient());

            container.Register(
                Component.For<IFacadeService>().
                Interceptors(InterceptorReference.ForType<SecurityInterception>())
               .Anywhere, Component.For<SecurityInterception>());
            container.Register(
                Component.For<ISecurityServiceChecker>()
                .ImplementedBy<SecurityServiceChecker>()
                .LifestyleTransient());

            #endregion


            // Log
            container.Register(
               Component.For<ILoggerService>().ImplementedBy<DbLoggerService>().Named("DB").LifeStyle.Transient,
               Component.For<ILoggerService>().ImplementedBy<FileLoggerService>().Named("File").LifeStyle.Transient
              
               );



            //externalHostAddressHelper
            container.Register(Component.For<ExternalHostAddressHelper>()
                                   .ImplementedBy<ExternalHostAddressHelper>()
                                   .LifestyleTransient());

            //domainServices 
            container.Register(
            fromAssemblyDescriptor

                                   .BasedOn(typeof(IDomainService<>))
                                   .WithServiceAllInterfaces()
                                   .LifestyleTransient());
            container.Register(
            fromAssemblyDescriptor

                                   .BasedOn(typeof(IDomainService))
                                   .WithServiceAllInterfaces()
                                   .LifestyleTransient());


            container.Register(fromAssemblyDescriptor
                              .BasedOn<IAutomaticVoucher>()
                              .WithServiceFromInterface()
                              .LifestyleTransient());



            #region register Repositories
            container.Register(Component.For(typeof(MITD.Domain.Repository.IRepository<>))
                                   .ImplementedBy(typeof(EFRepository<>))
                                   .LifestyleTransient());



            container.Register(
              fromAssemblyDescriptor
                                   .BasedOn<MITD.Domain.Repository.IRepository>()
                                   .WithServiceFromInterface()
                                   .LifestyleTransient());

            #endregion


            #region excute Bootstrapper
            foreach (var bootstrapper in this.GetBootstrappers())
            {
                bootstrapper.Execute();
            }
            #endregion


            #region set default resolver
            System.Web.Mvc.IDependencyResolver resolver = new MITD.Core.IocDependencyResolver();
            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = resolver.ToServiceResolver();
            #endregion

            container.Register(Component.For<IEventPublisher>().ImplementedBy<EventPublisher>().LifeStyle.Transient);


            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Buffer the Unit of Measure and Currencies data into the Application.
            //container.Resolve<IUnitOfMeasuresAndCurrenciesFacadeService>().Reload();
            UnitOfMeasuresAndCurrenciesRegsitrar.ReloadData();



        }

        //private void loadUnitOfMeasuresAndCurrencies()
        //{
        //    var session = CurrencyAndMeasurementSession.GetSession();
        //    var uow = new NHUnitOfWork(session);
        //    var transaction = new NHTransactionHandler();
        //    var configurator = new AggregateRootConfigurator(new TIBA.Core.EventPublisher());
        //    //create an uomType before create uom            
        //    var nhExMap = new NHRepositoryExceptionMapper(new List<System.Tuple<DBActionType, string, Exception>>());

        //    var target = new UnitOfMeasureRepository(transaction, uow, nhExMap, configurator);

        //    UnitOfMeasureParserHelper.RegisterParsersFromRepository(target);
        //}

        private List<IBootstrapper> GetBootstrappers()
        {
            //var cc = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\Integration\\MITD.Core.dll");
            //cc.GetTypes();
            //var asm = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\Integration\\MITD.CurrencyAndMeasurement.Domain.Contracts.dll");
            //var ttt = asm.GetTypes();
            var loader = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\bin\\MITD.Services.Castle.Dll");
            //var t = asm.CreateInstance("MITD.CurrencyAndMeasurement.Domain.Contracts.Currency", true, BindingFlags.CreateInstance, null, );

            var baseTypeName = typeof(IBootstrapper).Name;
            var current = this.GetType().FullName;

            var typs = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                                .Where(t => typeof(IBootstrapper).IsAssignableFrom(t) && t.FullName != current && !t.IsInterface));


            var result = typs.Select(t => Activator.CreateInstance(t) as IBootstrapper)
                  .ToList();

            return result;
        }
    }
}
#region

using System;
using System.Linq;
using System.Net.Http;
using System.Transactions;
using MITD.DataAccess.EF;
using MITD.Domain.Repository;
using MITD.Fuel.ACL.StorageSpace.Adapter;
using MITD.Fuel.ACL.StorageSpace.DomainServices;
using MITD.Fuel.ACL.StorageSpace.Mapper;
using MITD.Fuel.ACL.StorageSpace.ServiceWrappers;
using MITD.Fuel.Application.Service;
using MITD.Fuel.Application.Service.Contracts;
using MITD.Fuel.Data.EF.Context;
using MITD.Fuel.Data.EF.Repositories;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainServices;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Factories;
using MITD.Fuel.Domain.Model.FakeDomainServices;
using MITD.Fuel.Infrastructure.Service;
using MITD.Fuel.Presentation.Contracts.DTOs;
using MITD.Services.AntiCorruption;
using MITD.Services.Connectivity;
using MITD.StorageSpace.Presentation.Contracts.DTOs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace MITD.Fuel.Application.Test
{
    [TestClass]
    public class ApprovalServiceTest
    {
        #region Initial

        private Good _good;
        private Good _good2;
        private GoodPartyAssignment _goodPartAssignment;
        private GoodPartyAssignment _goodPartAssignment2;
        private Company _hafiz;
        private Vessel _hafizVessel;
        private Company _imsemCo;
        private Vessel _sapidVessel;
        private Vessel _sapidVessel2;
        private GoodUnit _unit;
        private GoodUnit _unit2;
        private GoodUnit _unit3;
        private OrderRepository _orderRepository;
        private Company _sapid;
        private UnitOfWorkScope _scope;
        private OrderApplicationService _target;
        private TransactionScope _tr;
        private IApproveFlowApplicationService _approveFlowApplicationService;
        private WorkflowLogRepository _approvalWorkFlowRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _scope = new UnitOfWorkScope(new EFUnitOfWorkFactory(() => new DataContainer()));

            var client = new WebClientHelper(new HttpClient());



            _orderRepository = new OrderRepository(_scope, new OrderConfigurator(), new EFRepository<OrderItem>(_scope));
            _orderRepository.GetAll();


            _tr = new TransactionScope();


            GenerateRepositories();
        }

        private void GenerateRepositories()
        {
            _scope = new UnitOfWorkScope(new EFUnitOfWorkFactory(() => new DataContainer()));

            var client = new WebClientHelper(new HttpClient());
            _orderRepository = new OrderRepository(_scope, new OrderConfigurator(), new EFRepository<OrderItem>(_scope));
            _orderRepository.GetAll();
            _tr = new TransactionScope();

            var hostAdapter = new ExternalHostAddressHelper();
            var vesselService = new VesselDomainService(new BaseAntiCorruptionAdapter<Vessel, WarehouseDto>(
                                                            new VesselAntiCorruptionServiceWrapper(
                                                                client, hostAdapter),
                                                            new VesselAntiCorruptionMapper()),
                                                        new VesselRepository(_scope));
            var goodService = new GoodDomainService(new GoodAntiCorruptionAdapter(
                                                        new GoodAntiCorruptionServiceWrapper(client, hostAdapter),
                                                        new GoodAntiCorruptionMapper()), new EFRepository<Good>(_scope));

            var companyService = new CompanyDomainService(
                new CompanyAntiCorruptionAdapter(new CompanyAntiCorruptionServiceWrapper
                                                     (client, hostAdapter),
                                                 new CompanyAntiCorruptionMapper()),
                new CompanyRepository(_scope));

            var orderApplicationService = new OrderApplicationService(
                _orderRepository, _scope,
                new UserRepository(_scope),
                vesselService, goodService,
                new OrderFactory(new OrderCodeGenerator()
                                 , new OrderConfigurator()
                                 , new WorkflowRepository(_scope)),
                companyService,
                new OrderItemDomainService(new EFRepository<OrderItem>(_scope)),
                new OrderConfigurator());
            _approvalWorkFlowRepository = new WorkflowLogRepository(_scope);
            _approveFlowApplicationService = new ApproveFlowApplicationService(_scope, _approvalWorkFlowRepository,
                                                                               new WorkflowRepository(_scope));
        }

        private void RegenerateUow()
        {
            _scope.CurrentUnitOfWork.Dispose();
            _scope = new UnitOfWorkScope(new EFUnitOfWorkFactory(() => new DataContainer()));
            GenerateRepositories();
        }


        [TestCleanup]
        public void TestCleanup()
        {
            _tr.Dispose();
        }
        #endregion

        [TestMethod]
        public void Test1()
        {
            var guid = Guid.NewGuid().ToString();

            try
            {
                _approveFlowApplicationService.ActApprovalFlow(WorkflowActions.Approve, 1, 1, WorkflowEntities.Order,
                                                               guid);
            }
            catch (Exception ex)
            {

                throw;
            }

            RegenerateUow();

            var wfl =
                _approvalWorkFlowRepository.GetQuery().OfType<OrderWorkflowLog>().FirstOrDefault(c => c.OrderId == 1
                    );
            /*
             ,
                                                        new SingleResultFetchStrategy<WorkflowLog>().OrderByDescending(
                                                            c => c.ActionDate));

             */
            Assert.IsTrue(wfl.CurrentWorkflowStepId == 2);
        }

        [TestMethod]
        public void Test2()
        {
            var guid = Guid.NewGuid().ToString();

            try
            {
                _approveFlowApplicationService.ActApprovalFlow(WorkflowActions.Approve, 2, 1, WorkflowEntities.Order,
                                                               guid);
            }
            catch (Exception ex)
            {

                throw;
            }

            RegenerateUow();

          //  var wfl = _approvalWorkFlowRepository.First(c => c.EntityId ==2,
                   //                                      new SingleResultFetchStrategy<WorkflowLog>().OrderByDescending(
               //                                              c => c.ActionDate));
           // Assert.IsTrue(wfl.CurrentWorkflowStepId == 4);
        }
    }
}
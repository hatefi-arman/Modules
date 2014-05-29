using System;
using System.Net.Http;
using System.Transactions;
using MITD.DataAccess.EF;
using MITD.Domain.Repository;
using MITD.Fuel.ACL.StorageSpace.Adapter;
using MITD.Fuel.ACL.StorageSpace.DomainServices;
using MITD.Fuel.ACL.StorageSpace.Mapper;
using MITD.Fuel.ACL.StorageSpace.ServiceWrappers;
using MITD.Fuel.Application.Facade;
using MITD.Fuel.Application.Facade.Mappers;
using MITD.Fuel.Data.EF.Context;
using MITD.Fuel.Data.EF.Repositories;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.DomainServices;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Infrastructure.Service;
using MITD.Fuel.Presentation.Contracts.FacadeServices;
using MITD.Services.Connectivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MITD.Fuel.Application.Test
{
    [TestClass]
    public class OrderDomainServiceTest
    {

        private OrderRepository _orderRepository;
        private UnitOfWorkScope _scope;

        private IOrderDomainService _target;
        [TestInitialize]
        public void TestInitialize()
        {
            _scope = new UnitOfWorkScope(new EFUnitOfWorkFactory(() => new DataContainer()));

            _orderRepository = null;//new OrderRepository(_scope, new OrderConfigurator(new StateFactory(new )), new EFRepository<OrderItem>(_scope));
            _orderRepository.GetAll();

            _target = new OrderDomainService(_orderRepository);

        }
        [TestMethod]
        public void GetCompanyFinalApprovedPrurchaseOrders()
        {
            var list = _target.GetFinalApprovedPrurchaseOrders(1);

            foreach (var order in list)
            {
              //  Assert.IsTrue(order.CurrentApproveFlowId == 3);    
                Assert.IsTrue(order.OwnerId == 1);    
                Assert.IsTrue(order.OrderType == OrderTypes.Purchase);    
            }
            

        }
        [TestMethod]
        public void GetCompanyFinalApprovedInternalTransferOrders()
        {
            var list = _target.GetFinalApprovedInternalTransferOrders(1);

            Assert.IsTrue(list.Count == 1); 
            foreach (var order in list)
            {
               // Assert.IsTrue(order.CurrentApproveFlowId == 3);
                Assert.IsTrue(order.OwnerId == 1);
                Assert.IsTrue(order.OrderType == OrderTypes.InternalTransfer);
                
            }
        }
    }
}

#region

using System;
using System.Collections.Generic;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

#endregion

namespace MITD.Fuel.Domain.Model.Factories
{
    public class OrderFactory : IOrderFactory
    {
        private readonly IOrderCodeGenerator _iOrderCodeGenerator;
        private readonly IEntityConfigurator<Order> _orderConfigurator;
        private readonly IWorkflowRepository _workflowRepository;

        public OrderFactory(IOrderCodeGenerator iOrderCodeGenerator,
                            IEntityConfigurator<Order> orderConfigurator,
            IWorkflowRepository workflowRepository

            )
        {
            _iOrderCodeGenerator = iOrderCodeGenerator;
            _orderConfigurator = orderConfigurator;
            _workflowRepository = workflowRepository;
        }

        #region IOrderFactory Members

        public Order CreateFactoryOrderObject(string description, long ownerId, long? transporter, long? supplier,
                                              long? receiver, OrderTypes orderType, DateTime orderDate,
                                              VesselInCompany fromVesselInCompany, VesselInCompany toVesselInCompany)
        {
            var code = _iOrderCodeGenerator.GenerateNewCode();

            var order = new Order(
                code,
                description,
                ownerId,
                transporter,
                supplier,
                receiver,
                orderType,
                orderDate,
                fromVesselInCompany,
                toVesselInCompany,
                States.Open,
                _orderConfigurator
                );

            var init = _workflowRepository.Single(c => c.WorkflowEntity == WorkflowEntities.Order && c.CurrentWorkflowStage == WorkflowStages.Initial);
            var orderWorkflow = new OrderWorkflowLog(order.Id, WorkflowEntities.Order, DateTime.Now, WorkflowActions.Init,
                //TODO: Fake ActorId
                    1101, "", init.Id, true);

            order.ApproveWorkFlows.Add(orderWorkflow);

            return order;
        }

        public OrderItem CreateFactoryOrderItemObject(Order order, string description, decimal quantity, long goodId,
                                                      long unitId, GoodFullInfo goodFullDetails)
        {
            var orderItem = new OrderItem(description, quantity, goodId, unitId, goodFullDetails);
            return orderItem;
        }

        #endregion
    }
}
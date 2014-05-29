using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.IDomainServices;

namespace MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate
{
    public class SubmitState : OrderState
    {

        private readonly IInventoryOperationDomainService inventoryOperationDomainService;
  
        private readonly IOrderStateFactory orderStateFactory;

        public SubmitState(IInventoryOperationDomainService inventoryOperationDomainService,
            IOrderStateFactory orderStateFactory
            )
        {
            this.inventoryOperationDomainService = inventoryOperationDomainService;
          
            this.orderStateFactory = orderStateFactory;
        }

        public override void ApproveOrder(Order order)
        {
            order.SetOrderState(orderStateFactory.CreateCloseState());
            order.CloseOrder();
        }

        public override void RejectOrder(Order order)
        {
            order.SetOrderState(orderStateFactory.CreateOpenState());
            order.CloseBackOrder();
        }

        public override void CancelOrder(Order order)
        {
            order.SetOrderState(orderStateFactory.CreateCancelState());
            order.CancelOrder(inventoryOperationDomainService);
        }

    }
}
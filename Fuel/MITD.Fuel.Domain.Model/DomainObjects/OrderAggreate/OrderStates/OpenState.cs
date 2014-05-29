using MITD.Fuel.Domain.Model.Exceptions;

namespace MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate
{
    public class OpenState : OrderState
    {
        private readonly IOrderStateFactory orderStateFactory;

        public OpenState(IOrderStateFactory orderStateFactory)
        {
            this.orderStateFactory = orderStateFactory;
        }
        public override void ApproveOrder(Order order)
        {
            order.SetOrderState(orderStateFactory.CreateSubmitState());
            order.SubmiteOrder();
        }
    }
}
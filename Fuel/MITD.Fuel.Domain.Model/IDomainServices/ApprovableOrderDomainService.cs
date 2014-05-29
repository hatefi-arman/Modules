using MITD.Fuel.Domain.Model.DomainObjects;

namespace MITD.Fuel.Domain.Model.IDomainServices
{
    public class ApprovableOrderDomainService : IApprovableOrderDomainService
    {
        public void SubmiteOrder(Order order)
        {
            order.OrderState.ApproveOrder(order);
        }

        public void CloseOrder(Order order)
        {
            order.OrderState.ApproveOrder(order);
        }

        public void CancelOrder(Order order)
        {
            order.OrderState.CancelOrder(order);
        }

    }
}
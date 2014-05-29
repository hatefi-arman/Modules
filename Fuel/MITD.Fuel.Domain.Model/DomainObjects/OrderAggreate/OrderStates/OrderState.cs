using System;
using MITD.Fuel.Domain.Model.Exceptions;

namespace MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate
{
    public abstract class OrderState
    {
        public virtual void ApproveOrder(Order order)
        {
            throw new InvalidStateException("Approve",string.Format("Cannot Approve {0} State",order.State.ToString()));
        }
        public virtual void RejectOrder(Order order)
        {
            throw new InvalidStateException("Reject", string.Format("Cannot Reject {0} State", order.State.ToString()));
        }
        public virtual void CancelOrder(Order order)
        {
            throw new InvalidStateException("Cancel", string.Format("Cannot Cancel {0} State", order.State.ToString()));
        }
    }
}
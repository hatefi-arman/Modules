#region

using System;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;

#endregion

namespace MITD.Fuel.Domain.Model.DomainServices
{
    public class OrderConfigurator : IEntityConfigurator<Order>
    {
        private readonly IOrderStateFactory orderStateFactory;

        #region IEntityConfigurator<Order> Members

        public OrderConfigurator(IOrderStateFactory orderStateFactory)
        {
            this.orderStateFactory = orderStateFactory;
        }

        public Order Configure(Order invoice)
        {
            ConfigureOrderTypes(invoice);
            ConfigureOrderStates(invoice);
            return invoice;
        }

        private void ConfigureOrderStates(Order order)
        {
            switch (order.State)
            {
                case States.Open:
                    order.SetOrderState(orderStateFactory.CreateOpenState());
                    break;
                case States.Submitted:
                    order.SetOrderState(orderStateFactory.CreateSubmitState());
                    break;
                case States.Closed:
                    order.SetOrderState(orderStateFactory.CreateCloseState());

                    break;
                case States.Cancelled:
                    order.SetOrderState(orderStateFactory.CreateCancelState());
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ConfigureOrderTypes(Order order)
        {
            switch (order.OrderType)
            {
                case OrderTypes.Purchase:
                    order.SetOrderType(new PurchaseOrder());
                    break;
                case OrderTypes.Transfer:
                    order.SetOrderType(new TransferOrder());
                    break;
                case OrderTypes.PurchaseWithTransfer:
                    order.SetOrderType(new PurchaseWithTransferOrder());
                    break;
                case OrderTypes.InternalTransfer:
                    order.SetOrderType(new InternalTransferOrder());
                    break;
                default:
                    throw new InvalidArgument("orderType");
            }
        }




        #endregion
    }
}
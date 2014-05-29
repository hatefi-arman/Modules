#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;
using MITD.Fuel.Domain.Model.Specifications;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class Order
    {
        #region ctor

        public Order()
        {
        }


        public Order(string code, string description, long ownerId, long? transporterId, long? supplierId, long? receiverId, OrderTypes _orderTypeClass, DateTime orderDate, VesselInCompany fromVesselInCompany, VesselInCompany toVesselInCompany, States state, IEntityConfigurator<Order> orderConfigurator)
        {
            Code = code;
            Description = description;
            SupplierId = supplierId;
            OwnerId = ownerId;
            TransporterId = transporterId;
            ReceiverId = receiverId;

            OrderDate = orderDate;
            FromVesselInCompanyId = fromVesselInCompany == null ? (long?)null : fromVesselInCompany.Id;
            ToVesselInCompanyId = toVesselInCompany == null ? (long?)null : toVesselInCompany.Id;
            this.OrderType = _orderTypeClass;
            State = state;
            orderConfigurator.Configure(this);
            _orderBaseType.Add(this, fromVesselInCompany, toVesselInCompany);
            ApproveWorkFlows = new List<OrderWorkflowLog>();
        }

        #endregion

        #region Properties

        private OrderTypeBase _orderBaseType;
        public long Id { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public long? SupplierId { get; private set; }
        public long? ReceiverId { get; private set; }
        public long? TransporterId { get; private set; }
        public long OwnerId { get; private set; }

        public OrderTypes OrderType { get; private set; }

        public DateTime OrderDate { get; private set; }

        public OrderState OrderState { get; private set; }

        public long? FromVesselInCompanyId { get; private set; }

        public long? ToVesselInCompanyId { get; private set; }

        public byte[] TimeStamp { get; set; }

        public States State { get; private set; }

        #endregion

        #region Navigation Properties

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual VesselInCompany FromVesselInCompany { get; set; }
        public virtual VesselInCompany ToVesselInCompany { get; set; }

        public virtual Company Transporter { get; set; }
        public virtual Company Receiver { get; set; }
        public virtual Company Supplier { get; set; }
        public virtual Company Owner { get; set; }
        public virtual List<OrderWorkflowLog> ApproveWorkFlows { get; private set; }

        public virtual IList<Invoice> Invoices { get; set; }

        #endregion

        #region Methods

        #region order related

        #region Operation


        #region OrderState
        public void SubmiteOrder()
        {
            CheckOrderAnyOrderItem();
            State = States.Submitted;
        }
        public void CloseOrder()
        {
            State = States.Closed;
        }


        public void CancelOrder(IInventoryOperationDomainService inventoryOperationDomainService)
        {
            if (OrderItems.Sum(c => c.ReceivedInMainUnit) > 0)
                throw new BusinessRuleException("", "can not cancel beacuse have refrenced !");


            State = States.Cancelled;
        }

        public void CloseBackOrder()
        {
            State = States.Open;
        }

        public void SetOrderState(OrderState orderState)
        {
            OrderState = orderState;
        }
        internal void SetOrderType(OrderTypeBase type)
        {
            _orderBaseType = type;
        }
        #endregion

        public void Update(string description, OrderTypes orderType, long ownerId, long? transporterId,
            long? supplierId, long? receiverId, VesselInCompany fromVesselInCompany, VesselInCompany toVesselInCompany, IEntityConfigurator<Order> orderConfigurator, IOrderItemDomainService orderItemDomainService)
        {
            if (OrderType != orderType)
            {

                foreach (var orderItem in OrderItems.ToList())
                {
                    orderItemDomainService.DeleteOrderItem(orderItem);
                }
            }
            OrderType = orderType;
            Description = description;
            OwnerId = ownerId;
            TransporterId = transporterId;
            ReceiverId = receiverId;
            SupplierId = supplierId;
            FromVesselInCompanyId = fromVesselInCompany == null ? (long?)null : fromVesselInCompany.Id;
            ToVesselInCompanyId = toVesselInCompany == null ? (long?)null : toVesselInCompany.Id;
            orderConfigurator.Configure(this);

            IsOnOpenState();

            _orderBaseType.Update(this, fromVesselInCompany, toVesselInCompany);
        }

        #endregion

        #region BusinessRule

        public void CheckDeleteRules()
        {
            IsHaveAnyOrderItem();
        }

        #endregion

        #endregion

        #region orderItem related

        #region Operation

        public void AddItem(OrderItem orderItem, GoodFullInfo goodFullDomainInStorageService)
        {
            OrderItems.Add(orderItem);
            CheckCommonAddAndEditItemRules(orderItem, goodFullDomainInStorageService);
        }

        public OrderItem UpdateItem(long id, string description, decimal quantity, long goodId, long unitId,
                                    long? goodPartyAssignmentId, GoodFullInfo goodFullInfo
            )
        {
            var orderItem = OrderItems.Single(c => c.Id == id);
            if (orderItem == null)
                throw new ObjectNotFound("Order Item", id);

            orderItem.Update(description, quantity, goodId, unitId, goodFullInfo);
            CheckCommonAddAndEditItemRules(orderItem, goodFullInfo);

            return orderItem;
        }

        public void DeleteItem(OrderItem item, IOrderItemDomainService orderItemDomainService)
        {
            // Bussuiness Rules
            IsOnOpenState();


            var orderItem = OrderItems.FirstOrDefault(c => c.Id == item.Id);
            if (orderItem == null)
                throw new ObjectNotFound("OrderItem", item.Id);
            orderItemDomainService.DeleteOrderItem(orderItem);
        }

        #endregion

        #region BusinessRule

        private void CheckCommonAddAndEditItemRules(OrderItem orderItem,
                                                    GoodFullInfo goodFullInfo)
        {
            //BR_PO04
            IsOnOpenState();

            //BR_PO9
            IsNotDuplicate(orderItem);

            // BR_17  developer Dont implement refrence in order

            //BR_PO18
            CanBeOrderThisGood(orderItem, goodFullInfo);

            //BR_PO20
            GoodHaveValidSuplierAndTransporter(orderItem, goodFullInfo);


            //ValidateGoodQuantity(orderItem, goodDomainService);

            //            //BR_PO21
            //            CanBeOrderWithReOrderLevelCheck(orderItem, goodDomainService);

            //            //BR_PO22
            //            MaxOfOrderCheck(orderItem.Quantity, orderItem, goodPartyAssignmentDomainService);
            //
            //            //BR_PO23
            //            FixOfOrderCheck(orderItem.Quantity, orderItem, goodPartyAssignmentDomainService);
        }


        //BR_PO6
        private void IsHaveAnyOrderItem()
        {
            if (OrderItems != null && OrderItems.Count > 0)
                throw new BusinessRuleException("", "this order have  some items for Delete.");
        }

        //BR_PO7
        private void CheckOrderAnyOrderItem()
        {
            if (OrderItems == null || OrderItems.Count == 0)
                throw new BusinessRuleException("", "this order must have  some items for Submit.");
        }


        //BR_PO9
        private void IsNotDuplicate(OrderItem orderItem)
        {
            var q1 = OrderItems.FirstOrDefault(c => c.GoodId == orderItem.GoodId
                // && c.GoodPartyAssignmentId == orderItem.GoodPartyAssignmentId
                                                    && c.Id != orderItem.Id);
            if (q1 != null)
                throw new BusinessRuleException("BR_PO9", " OrderItem Has Duplicate ");
        }


        //BR_PO17
        private bool EquqlReferenceOrderWithOrderItem()
        {
            return true;
        }


        //BR_PO4
        private void IsOnOpenState()
        {
            var finalApprovedState = new OrderIsOpenState();
            if (!finalApprovedState.IsSatisfiedBy(this))
                throw new BusinessRuleException("BR_PO4", "Order in Not Open State");
        }

        //        //BR_PO10
        //        private void IsNotOnMiddleApprovedStates()
        //        {
        //            var isMiddleApprovedState = new IsOrderInMiddleApprovedState();
        //            if (isMiddleApprovedState.IsSatisfiedBy(this))
        //                throw new BusinessRuleException("BR_PO10", "Order in Middle Approve State");
        //        }
        //
        //        //BR_PO13
        //        private void IsOnMiddleApprovedStates()
        //        {
        //            var isMiddleApprovedState = new IsOrderInMiddleApprovedState();
        //            if (isMiddleApprovedState.IsSatisfiedBy(this))
        //                throw new BusinessRuleException("BR_PO10", "Order in Middle Approve State");
        //        }

        //BR_PO18
        private void CanBeOrderThisGood(OrderItem orderItem,
                                        GoodFullInfo goodFullInfo)
        {
            if (!goodFullInfo.CanBeOrderedThisGood)
                throw new BusinessRuleException("BR_PO18", "Good Order Cant Be Order ");
        }


        //BR_PO20
        private void GoodHaveValidSuplierAndTransporter(OrderItem orderItem, GoodFullInfo goodFullInfo)
        {
            _orderBaseType.ValidateGoodSuplierAndTransporter(this, goodFullInfo);
        }

        //Br_PO21 ,22,23
        //        private void ValidateGoodQuantity(OrderItem orderItem,
        //                                                    IGoodDomainService goodDomainService)
        //        {
        //            var list = new List<string>();
        //            var can = goodDomainService.CanBeOrderAmountOfGood(orderItem.GoodId, orderItem.Quantity, list);
        //            if (!can)
        //                throw new BusinessRuleException("Br_PO21", list.SingleOrDefault());
        //        }

        //        //BR_PO21
        //        private void CanBeOrderWithReOrderLevelCheck(OrderItem orderItem,
        //                                                    IGoodPartyAssignmentDomainService goodPartyAssignmentDomainService)
        //        {
        //            if (ToVesselInCompanyId == null) return;
        //
        //            var c =
        //                goodPartyAssignmentDomainService.CanBeOrderWithReOrderLevelCheck(
        //                    ReceiverId.GetValueOrDefault(), orderItem.GoodId,
        //                    ToVesselInCompanyId.GetValueOrDefault());
        //            if (c == false)
        //                throw new BusinessRuleException("Dont order item when Up ReOrderLevel ");
        //        }
        //     
        //        //BR_PO22
        //        private void MaxOfOrderCheck(double quantity, OrderItem orderItem,
        //                                    IGoodPartyAssignmentDomainService GoodPartyAssignmentDomainService)
        //        {
        //            if (ToVesselInCompanyId == null) return;
        //            var result = GoodPartyAssignmentDomainService.IsNotOverMaximumOrder(quantity,
        //                                                                      OwnerId,
        //                                                                      orderItem.GoodId,
        //                                                                      ToVesselInCompanyId.GetValueOrDefault());
        //
        //            if (!result)
        //                throw new BusinessRuleException("Up of Max Order ");
        //        }
        //
        //        //BR_PO23
        //        private void FixOfOrderCheck(double quantity, OrderItem orderItem,
        //                                    IGoodPartyAssignmentDomainService goodPartyAssignmentDomainService)
        //        {
        //            //            if (ToVesselInCompanyId == null)
        //            //                throw new BusinessRuleException("Not Equal With Fix Order");
        //            //
        //            //            var c = GoodPartyAssignmentDomainService.IsEqualFixOrder(quantity,
        //            //                                                                ReceiverId.GetValueOrDefault(),
        //            //                                                                orderItem.GoodId, ToVesselInCompanyId.GetValueOrDefault());
        //            //            if (c)
        //            //                throw new BusinessRuleException("Not Equal With Fix Order");
        //        }

        #endregion

        #endregion

        #endregion

        public void UpdateItemInvoicedQuantity(long orderItemId, decimal quantity)
        {
            var orderItem = OrderItems.SingleOrDefault(c => c.Id == orderItemId);
            if (orderItem == null)
                throw new ObjectNotFound("OrderItem", orderItemId);
            orderItem.UpdateInvoiced(quantity);
        }
    }
}
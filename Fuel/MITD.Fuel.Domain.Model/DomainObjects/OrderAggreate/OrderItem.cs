#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using MITD.Domain.Model;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;

#endregion

namespace MITD.Fuel.Domain.Model.DomainObjects
{
    public class OrderItem
    {
        #region Properties

        public long Id { get; private set; }
        
        public string Description { get; private set; }
        
        public decimal Quantity { get; private set; }
        
        public long MeasuringUnitId { get; private set; }

        public long OrderId { get; private set; }

        public long GoodId { get; private set; }

        public decimal InvoicedInMainUnit { get; private set; }

        public decimal ReceivedInMainUnit { get; private set; }

        #endregion

        #region Navigation Properties

        //        public virtual GoodPartyAssignment GoodPartyAssignment { get; private set; }
        public virtual GoodUnit MeasuringUnit { get; private set; }

        public virtual Good Good { get; private set; }

        public virtual Order Order { get; private set; }

        public byte[] TimeStamp { get; private set; }

        public virtual IList<OrderItemBalance> OrderItemBalances { get; private set; }

        #endregion

        #region ctor

        public OrderItem()
        {
            OrderItemBalances = new List<OrderItemBalance>();
        }

        public OrderItem(string description, decimal quantity, long goodId, long unitId, GoodFullInfo goodFullInfo)
        {
            Description = description;
            Quantity = quantity;
            GoodId = goodId;
            MeasuringUnitId = unitId;
            CommonBussinessRule(goodFullInfo, unitId);
            OrderItemBalances = new List<OrderItemBalance>();
        }


        internal void Update(string description, decimal quantity, long goodId, long unitId, GoodFullInfo goodFullInfo)
        {
            Description = description;
            Quantity = quantity;
            GoodId = goodId;
            MeasuringUnitId = unitId;
            CommonBussinessRule(goodFullInfo, unitId);
        }

        private void CommonBussinessRule(GoodFullInfo goodFullInfo, long unitId)
        {
            //BR_PO5
            IsNotEmpty();
            //BR_PO8
            IsHaveValidQuantity();
            //BR_PO19
            ValidateGoodUnit(goodFullInfo, unitId);
        }

        #endregion



        #region BussinesRules

        //BR_PO5
        private void IsNotEmpty()
        {
            if (GoodId == 0 || MeasuringUnitId == 0)
                throw new BusinessRuleException("BR_PO5", "Unit OR Good Is Empty ");
        }

        //BR_PO8
        private void IsHaveValidQuantity()
        {
            if (Quantity <= 0)
                throw new BusinessRuleException("BR_PO8", "Quantity Is Negative ");
        }


        //BR_PO19
        private void ValidateGoodUnit(GoodFullInfo goodFullInfo, long unitId)
        {
            if (goodFullInfo.CompanyGoodUnits == null || goodFullInfo.CompanyGoodUnits.All(c => c.Id != unitId))
                throw new BusinessRuleException("", " Unit  Dont Definition For Compony ");
        }

        #endregion

        public void UpdateInvoiced(decimal invoicedQuantityInMainUnit)
        {
            if(InvoicedInMainUnit + invoicedQuantityInMainUnit > ReceivedInMainUnit)
                throw new BusinessRuleException("","Invoiced quantity coud not be greater than Received quantity.");

            InvoicedInMainUnit += invoicedQuantityInMainUnit;
        }
        public void UpdateReceived(decimal receivedQuantityInMainUnit, IGoodUnitConvertorDomainService goodUnitConvertorDomainService)
        {
            var orderItemQuantityInMainUnit = goodUnitConvertorDomainService.ConvertUnitValueToMainUnitValue(this.MeasuringUnit, this.Quantity);
            if (ReceivedInMainUnit + receivedQuantityInMainUnit > orderItemQuantityInMainUnit)
                throw new BusinessRuleException("", "Received quantity could not become greater then available ordered quantity.");

            ReceivedInMainUnit += receivedQuantityInMainUnit;
        }

        public decimal GetAvailableForInvoiceInMainUnit()
        {
            return ReceivedInMainUnit-InvoicedInMainUnit;
        }
    }
}
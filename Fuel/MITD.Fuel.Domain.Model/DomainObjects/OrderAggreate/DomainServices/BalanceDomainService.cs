#region

using System;
using System.Collections.Generic;
using System.Linq;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;
using MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate;
using MITD.Fuel.Domain.Model.Enums;
using MITD.Fuel.Domain.Model.Exceptions;
using MITD.Fuel.Domain.Model.IDomainServices;
using MITD.Fuel.Domain.Model.Repositories;

#endregion

namespace MITD.Fuel.Domain.Model.DomainServices
{
    public class BalanceDomainService : IBalanceDomainService
    {
        private readonly IRepository<OrderItemBalance> balanceRepository;
        private readonly IGoodUnitConvertorDomainService goodUnitConvertorDomainService;
        private readonly IOrderRepository orderRepository;
        private readonly IRepository<FuelReportDetail> fuelReportDetailRepository;

        public BalanceDomainService(IOrderRepository orderRepository,
                                    IGoodUnitConvertorDomainService goodUnitConvertorDomainService,
                                    IRepository<OrderItemBalance> balanceRepository,
            IRepository<FuelReportDetail> fuelReportDetailRepository)
        {
            this.orderRepository = orderRepository;
            this.goodUnitConvertorDomainService = goodUnitConvertorDomainService;
            this.balanceRepository = balanceRepository;
            this.fuelReportDetailRepository = fuelReportDetailRepository;
        }

        public void DeleteInvoiceItemRefrencesFromBalance(long id)
        {
            var balances = balanceRepository.Find(c => c.InvoiceItemId == id);
            if (balances == null)
                throw new ObjectDisposedException("Balance");

            foreach (var orderItemBalance in balances)
            {
                var order = orderRepository.Single(c => c.Id == orderItemBalance.OrderId);
                order.UpdateItemInvoicedQuantity(orderItemBalance.OrderItemId, -1 * orderItemBalance.QuantityAmountInMainUnit);
                balanceRepository.Delete(orderItemBalance);
            }
        }

        public IEnumerable<InvoiceItem> GenerateInvoiceItemFromOrders(List<long> orderList)
        {
            var orderRefrences = orderRepository.Find(c => orderList.Contains(c.Id) && c.State == States.Submitted);
            var invoiceItems = new List<InvoiceItem>();
            var orderItemsForInvoicing = orderRefrences.SelectMany(c => c.OrderItems.Where(d => d.GetAvailableForInvoiceInMainUnit() > 0)).ToList();

            foreach (var orderItem in orderItemsForInvoicing)
            {
                var invoiceItem = invoiceItems.SingleOrDefault(c => c.GoodId == orderItem.GoodId);
                var availableForInvoiceInMainUnit = orderItem.GetAvailableForInvoiceInMainUnit();
                if (invoiceItem == null)
                {
                    invoiceItem = new InvoiceItem(availableForInvoiceInMainUnit, 0, orderItem.Good, orderItem.MeasuringUnit.MainGoodUnit, 0, "");
                    invoiceItems.Add(invoiceItem);
                }
                else
                {
                    //<A.H> Commented to be refactored to reflect the unit conversions and support for different unit of measure by Invoice items.
                    //if (orderItem.MeasuringUnitId != invoiceItem.MeasuringUnitId)
                    //    availableForInvoiceInMainUnit = updateInvoiceItemWithHeterogeneousOderItem(invoiceItem, orderItem, availableForInvoiceInMainUnit);
                   
                    invoiceItem.UpdateQuantity(availableForInvoiceInMainUnit);
                }
            }
            return invoiceItems;
        }

        //<A.H> This method is required for unifying the InvoiceItem with relevant OrderItem Unit of Measurement.
        private decimal updateInvoiceItemWithHeterogeneousOderItem(InvoiceItem invoiceItem, OrderItem orderItem, decimal availableForInvoice)
        {
            var currentValueInMainUnit = goodUnitConvertorDomainService.GetUnitValueInMainUnit
                (invoiceItem.GoodId, invoiceItem.MeasuringUnitId, invoiceItem.Quantity);
            invoiceItem.UpdateUnitAndQuantity(currentValueInMainUnit.Id, currentValueInMainUnit.Value);
            var newValueInMainUnit = goodUnitConvertorDomainService.GetUnitValueInMainUnit
                (invoiceItem.GoodId, orderItem.MeasuringUnitId, availableForInvoice);

            return newValueInMainUnit.Value;
        }

        /*
        public void CreateBalanceRecordForInvoiceItem(InvoiceItem invoiceItem, List<Order> orderRefrences)
        {
            var orderItemsForGood =
                orderRefrences.SelectMany(c => c.OrderItems.Where(d => d.GoodId == invoiceItem.GoodId && d.GetAvailableForInvoice() > 0)).ToList();

            if (orderItemsForGood.GroupBy(c => c.MeasuringUnitId == invoiceItem.MeasuringUnitId).Count() > 1)
                balanceInvoiceItemInMainUnit(invoiceItem, orderItemsForGood);
            else
                balanceInvoiceItem(invoiceItem, orderItemsForGood);
        }

        private void balanceInvoiceItemInMainUnit(InvoiceItem invoiceItem, IEnumerable<OrderItem> orderItems)
        {
            var requestQuantity = goodUnitConvertorDomainService.ConvertUnitValueToMainUnitValue(invoiceItem.MeasuringUnit, invoiceItem.Quantity);
            foreach (OrderItem orderItem in orderItems)
            {
                var availbleQuantity = goodUnitConvertorDomainService.ConvertUnitValueToMainUnitValue
                    (orderItem.MeasuringUnit, orderItem.GetAvailableForInvoice());

                decimal quantity = requestQuantity >= availbleQuantity ? availbleQuantity : requestQuantity;
                orderItem.UpdateInvoiced(quantity);
                balanceRepository.Add(new OrderItemBalance(orderItem, invoiceItem.Id,  quantity, orderItem.MeasuringUnit.Abbreviation));
                requestQuantity -= quantity;
                if (requestQuantity == 0)
                    break;
            }

            if (requestQuantity > 0)
                throw new BusinessRuleException("", "Not enough Quantity In Refrenced Orders");
        }

        private void balanceInvoiceItem(InvoiceItem invoiceItem, IEnumerable<OrderItem> orderItems)
        {
            var requestQuantity = invoiceItem.Quantity;

            foreach (OrderItem orderItem in orderItems)
            {
                decimal quantity = requestQuantity >= orderItem.GetAvailableForInvoice() ? orderItem.GetAvailableForInvoice() : requestQuantity;
                orderItem.UpdateInvoiced(quantity);
                balanceRepository.Add(new OrderItemBalance(orderItem, invoiceItem, quantity, "Ton"));
                requestQuantity -= quantity;
                if (requestQuantity == 0)
                    break;
            }
            if (requestQuantity > 0)
                throw new BusinessRuleException("", "Not enough Quantity In Refrenced Orders");
        }
        
        private decimal updateInvoiceItemWithHeterogeneousOderItem(InvoiceItem invoiceItem, OrderItem orderItem, decimal availableForInvoice)
        {
            var currentValueInMainUnit = goodUnitConvertorDomainService.GetUnitValueInMainUnit
                (invoiceItem.GoodId, invoiceItem.MeasuringUnitId, invoiceItem.Quantity);
            invoiceItem.UpdateUnitAndQuantity(currentValueInMainUnit.Id, currentValueInMainUnit.Value);
            var newValueInMainUnit = goodUnitConvertorDomainService.GetUnitValueInMainUnit
                (invoiceItem.GoodId, orderItem.MeasuringUnitId, availableForInvoice);

            return newValueInMainUnit.Value;
        }
        */


        public void CreateBalanceRecordForInvoiceItem(InvoiceItem invoiceItem, List<Order> orderRefrences)
        {
            var relatedOrderItemsToInvoiceItem =
                orderRefrences.SelectMany(c => c.OrderItems.Where(d => d.GoodId == invoiceItem.GoodId /*&& d.GetAvailableForInvoice() > 0*/)).ToList();

            balanceInvoiceItemInMainUnit(invoiceItem, relatedOrderItemsToInvoiceItem);
            //if (relatedOrderItemsToInvoiceItem.GroupBy(c => c.MeasuringUnitId == invoiceItem.MeasuringUnitId).Count() > 1)
            //else
            //    balanceInvoiceItem(invoiceItem, relatedOrderItemsToInvoiceItem);
        }

        private void balanceInvoiceItemInMainUnit(InvoiceItem invoiceItem, IEnumerable<OrderItem> orderItems)
        {
            var fuelReportDetialListFetchStrategy = new ListFetchStrategy<FuelReportDetail>()
                            .OrderBy(frd => frd.FuelReport.EventDate);


            var orderIds = orderItems.Select(oi => oi.OrderId).ToList();

            var fuelReportDetailsOfOrderItems = fuelReportDetailRepository.Find(
                                            frd =>
                                                frd.GoodId == invoiceItem.GoodId &&
                                                frd.ReceiveReference.ReferenceType == ReferenceType.Order &&
                                                orderIds.Contains(frd.ReceiveReference.ReferenceId.Value),
                                            fuelReportDetialListFetchStrategy);

            var unassignedInvoiceItemQuantity = goodUnitConvertorDomainService.ConvertUnitValueToMainUnitValue(invoiceItem.MeasuringUnit, invoiceItem.Quantity);

            for (int index = 0; index < fuelReportDetailsOfOrderItems.Count; index++)
            {
                var checkingFuelReportDetail = fuelReportDetailsOfOrderItems[index];

                var fuelReportDetailInvoicedQuantityInMainUnit = balanceRepository.Find(b => b.FuelReportDetailId == checkingFuelReportDetail.Id).Sum(found => found.QuantityAmountInMainUnit);

                var fuelReportDetailReceivedInMainUnit = goodUnitConvertorDomainService.ConvertUnitValueToMainUnitValue(checkingFuelReportDetail.MeasuringUnit, (decimal)checkingFuelReportDetail.Receive.Value);

                var fuelReportDetailNotInvoicedQuantity = fuelReportDetailReceivedInMainUnit - fuelReportDetailInvoicedQuantityInMainUnit;

                if (fuelReportDetailNotInvoicedQuantity <= 0)
                    continue;

                var availableQuantityForBalancing = Math.Min(fuelReportDetailNotInvoicedQuantity, unassignedInvoiceItemQuantity);

                var relevantOrderItem = orderItems.Single(
                        oi =>
                            oi.OrderId == checkingFuelReportDetail.ReceiveReference.ReferenceId.Value &&
                            oi.GoodId == checkingFuelReportDetail.GoodId);

                balanceRepository.Add(new OrderItemBalance(relevantOrderItem, invoiceItem.Id, checkingFuelReportDetail.Id, availableQuantityForBalancing, checkingFuelReportDetail.Good.SharedGood.MainUnit.Abbreviation));

                relevantOrderItem.UpdateInvoiced(availableQuantityForBalancing);

                unassignedInvoiceItemQuantity -= availableQuantityForBalancing;

                if (unassignedInvoiceItemQuantity <= 0)
                    break;
            }

            if (unassignedInvoiceItemQuantity != 0)
            {
                throw new BusinessRuleException("", string.Format("Invoiced Quantity for Good '{0}' has deficiencies with selected Orders.", invoiceItem.Good.Name));
            }
        }

        //private void balanceInvoiceItem(InvoiceItem invoiceItem, IEnumerable<OrderItem> orderItems)
        //{
        //    var requestQuantity = invoiceItem.Quantity;

        //    foreach (OrderItem orderItem in orderItems)
        //    {
        //        decimal quantity = requestQuantity >= orderItem.GetAvailableForInvoiceInMainUnit() ? orderItem.GetAvailableForInvoiceInMainUnit() : requestQuantity;
        //        orderItem.UpdateInvoiced(quantity);
        //        balanceRepository.Add(new OrderItemBalance(orderItem, invoiceItem, quantity, "Ton"));
        //        requestQuantity -= quantity;
        //        if (requestQuantity == 0)
        //            break;
        //    }
        //    if (requestQuantity > 0)
        //        throw new BusinessRuleException("", "Not enough Quantity In Refrenced Orders");
        //}




        public void SetReceivedData(long orderId, long fuelReportDetailId, long goodId, long unitId, decimal receivedQuantity)
        {
            var order = orderRepository.Single(c => c.Id == orderId);
            var orderItem = order.OrderItems.Single(c => c.GoodId == goodId);
            //var quantity = receivedQuantity;

            //if (unitId != orderItem.MeasuringUnitId)
            //{
            //    quantity = goodUnitConvertorDomainService.ConvertUnits(goodId, unitId, orderItem.MeasuringUnitId, receivedQuantity);
            //}
            var quantity = goodUnitConvertorDomainService.ConvertUnitValueToMainUnitValue(unitId, receivedQuantity);

            orderItem.UpdateReceived(quantity, goodUnitConvertorDomainService);

            //A.H : Deleted due to implementation in submitting invoice.
            //var orderItemBalance = new OrderItemBalance(orderItem, fuelReportDetailId, quantity, orderItem.MeasuringUnit.Abbreviation); 
            //balanceRepository.Add(orderItemBalance);
        }
    }
}
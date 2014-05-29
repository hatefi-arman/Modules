using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.CurrencyAndMeasurement.Domain.Contracts;
using MITD.Fuel.Domain.Model.DomainObjects.InvoiceAggreate;

namespace MITD.Fuel.Domain.Model.DomainObjects.OrderAggreate
{
    public class OrderItemBalance
    {
        public OrderItemBalance()
        {
        }

        public long Id { get; private set; }
        public long OrderId { get; private set; }
        public long OrderItemId { get; private set; }
        public virtual OrderItem OrderItem { get; private set; }
        public decimal QuantityAmountInMainUnit { get; private set; }
        public string UnitCode { get; private set; }

        public Quantity Quantity { get; set; }

        public long FuelReportDetailId { get; private set; }
        public virtual FuelReportDetail FuelReportDetail { get; private set; }

        public long InvoiceItemId { get; private set; }

        public virtual InvoiceItem InvoiceItem { get; private set; }

        public byte[] TimeStamp { get; private set; }

        //public long Sequence { get; set; }

        //
        //        public OrderItemBalance(FuelReportDetail fuelReportDetail)
        //        {
        //            FuelReportDetail = fuelReportDetail;
        //            FuelReportCount += decimal.Parse(fuelReportDetail.Transfer.Value.ToString());
        //        }
        //
        //        public OrderItemBalance(decimal fuelReportCount, decimal invoiceItemCount)
        //        {
        //            FuelReportCount = fuelReportCount;
        //            InvoiceItemCount = invoiceItemCount;
        //        } 
        //        public void UpdateOrderItemBalance(decimal fuelReportCount, decimal invoiceItemCount)
        //        {
        //            FuelReportCount += fuelReportCount;
        //            InvoiceItemCount += invoiceItemCount;
        //        }
        //
        //        public OrderItemBalance(InvoiceItem invoiceItem)
        //        {
        //            InvoiceItem = invoiceItem;
        //            InvoiceItemCount += invoiceItem.Quantity;
        //        }

        private void setQuantity(Quantity quantity)
        {
            this.Quantity = quantity;
            this.QuantityAmountInMainUnit = quantity.Amount;
            this.UnitCode = quantity.Unit.Abbreviation;
        }

        private void setQuantity(decimal amount, string unitCode)
        {
            var quantity = new Quantity(amount, new UnitOfMeasure(unitCode));

            setQuantity(quantity);
        }

        public OrderItemBalance(OrderItem orderItem, long invoiceItemId, long fuelReportDetailId, decimal amount, string unitCode)
        {
            OrderItemId = orderItem.Id;
            OrderId = orderItem.OrderId;
            InvoiceItemId = invoiceItemId;
            FuelReportDetailId = fuelReportDetailId;
            this.setQuantity(amount, unitCode);
        }
        //public OrderItemBalance(OrderItem orderItem, long fuelReportDetailId, decimal amount, string unitCode)
        //{
        //    OrderItemId = orderItem.Id;
        //    OrderId = orderItem.OrderId;
        //    FuelReportDetailId = fuelReportDetailId;

        //    this.setQuantity(amount, unitCode);
        //}
    }
}
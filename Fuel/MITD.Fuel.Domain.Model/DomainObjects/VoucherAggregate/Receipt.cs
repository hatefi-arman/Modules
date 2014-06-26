using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class Receipt
    {
        #region Prop
        public long  Id { get; private set; }
       // public string ReceiptNumber { get; private set; }
        public int ReceiptQuantity { get; private set; }
        public decimal ReceiptFee { get; private set; }

      //  public string ReceiptWarehouseCode { get; private set; }

        public decimal Coefficient { get; private set; }

        #endregion

        #region Ctor

        public Receipt()
        {
                
        }

        public Receipt(long id,
            //string receiptNumber,
            int receiptQuantity,
            decimal receiptFee,
            //string receiptWarehouseCode,
            decimal coefficient)
        {

            Id = id;
            ReceiptFee = receiptFee;
            ReceiptQuantity = receiptQuantity;
            //ReceiptNumber = receiptNumber;
            //ReceiptWarehouseCode = receiptWarehouseCode;
            Coefficient = coefficient;


        }
        #endregion
    }
}

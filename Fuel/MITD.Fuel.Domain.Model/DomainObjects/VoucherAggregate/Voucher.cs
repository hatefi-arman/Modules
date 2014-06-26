using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate
{
    public class Voucher
    {
        #region Prop
        public long Id { get;  set; }
        
        public long CurrencyId { get;  set; }
        public virtual Currency Currency { get;  set; }

        public string Description { get; set; }
        public DateTime FinancialVoucherDate { get; set; }
        public DateTime LocalVoucherDate { get; set; }
        public string LocalVoucherNo { get; set; }

        public string ReferenceNo { get; set; }
        public string VoucherRef { get; set; }

        public ReferenceType ReferenceType { get; set; }

        public int ReferenceTypeId { get; set; }

        public List<JournalEntry> JournalEntrieses { get; set; }

        public byte[] TimeStamp { get; set; }
        #endregion


        #region ctor

        public Voucher()
        {
            
        }

        public Voucher(
            long id, long currencyId, 
            string description, DateTime financialVoucherDate,
            DateTime localVoucherDate, string localVoucherNo,
            string referenceNo, string voucherRef, int referenceTypeId,
            List<JournalEntry> journalEntrieses 
            )
        {
            Id = id;
            CurrencyId = currencyId;
            Description = description;
            FinancialVoucherDate = financialVoucherDate;
            LocalVoucherDate = localVoucherDate;
            LocalVoucherNo = localVoucherNo;
            ReferenceNo = referenceNo;
            VoucherRef = voucherRef;
            ReferenceTypeId = referenceTypeId;
            JournalEntrieses = journalEntrieses;
        }
        #endregion


    }
}

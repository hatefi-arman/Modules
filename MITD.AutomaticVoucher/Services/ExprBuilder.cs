using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;
using MITD.Fuel.Domain.Model.DomainObjects;

namespace MITD.AutomaticVoucher.Services
{
    public static class ExprBuilder
    {
        public static Voucher LocalVoucherDate(this Voucher voucher )
        {
            voucher.LocalVoucherDate = DateTime.Now;
            return voucher;
        }

        public static Voucher FinancialVoucherDate(this Voucher voucher, DateTime dateTime)
        {
            voucher.FinancialVoucherDate = dateTime;
            return voucher;
        }

        public static Voucher Description(this Voucher voucher, string description)
        {
            voucher.Description = description;
            return voucher;
        }

        public static Voucher ReferenceNo(this Voucher voucher, string referenceNo)
        {
            voucher.ReferenceNo = referenceNo;
            return voucher;
        }

        public static Voucher LocalVoucherNo(this Voucher voucher, string localVoucherNo)
        {
            voucher.LocalVoucherNo = localVoucherNo;
            return voucher;
        }
        public static Voucher VoucherRef(this Voucher voucher, string voucherRef)
        {
            voucher.VoucherRef = voucherRef;
            return voucher;
        }

        public static Voucher SetReferenceType(this Voucher voucher, ReferenceType referenceType)
        {
            voucher.ReferenceType = referenceType;
            voucher.ReferenceTypeId = referenceType.Id;
            return voucher;
        }

        public static Voucher SetCurrency(this Voucher voucher,MITD.Fuel.Domain.Model.DomainObjects.Currency currency)
        {
            voucher.CurrencyId=currency.Id;
            return voucher;
         
        }
        public static JournalEntry Typ(this JournalEntry journalEntry, int type)
        {
            journalEntry.Typ = type;
            return journalEntry;

        }
        public static JournalEntry IrrAmount(this JournalEntry journalEntry, decimal irrAmount)
        {
            journalEntry.IrrAmount = irrAmount;
            return journalEntry;

        }
        public static JournalEntry Voucher(this JournalEntry journalEntry, long voucherId)
        {
            journalEntry.VoucherId = voucherId;
            return journalEntry;

        }
        public static JournalEntry VoucherRef(this JournalEntry journalEntry, string voucherRef)
        {
            journalEntry.VoucherRef = voucherRef;
            return journalEntry;

        }

        public static JournalEntry Description(this JournalEntry journalEntry, string description)
        {
            journalEntry.Description = description;
            return journalEntry;

        }

        public static JournalEntry AccountNo(this JournalEntry journalEntry, string accountNo)
        {
            journalEntry.AccountNo = accountNo;
            return journalEntry;

        }

        public static JournalEntry ForeignAmount(this JournalEntry journalEntry, decimal foreignAmount)
        {
            journalEntry.ForeignAmount = foreignAmount;
            return journalEntry;

        }

        public static JournalEntry SetSegment(this JournalEntry journalEntry, Action<Segment> action, Segment segment)
        {

             action(segment);

            return journalEntry;
        }
        public static JournalEntry SetSegmentType(this JournalEntry journalEntry, SegmentType segment)
        {
            journalEntry.Segment.SegmentType = segment;
            journalEntry.Segment.SegmentTypeId = segment.Id;
            return journalEntry;

        }

    }
}

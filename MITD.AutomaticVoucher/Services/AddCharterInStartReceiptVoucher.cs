using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MITD.Domain.Repository;
using MITD.Fuel.Domain.Model.DomainObjects;
using MITD.Fuel.Domain.Model.DomainObjects.CharterAggregate;
using MITD.Fuel.Domain.Model.DomainObjects.VoucherAggregate;
using MITD.Fuel.Domain.Model.Repositories;

namespace MITD.AutomaticVoucher.Services
{
    public class AddCharterInStartReceiptVoucher
    {

        private readonly IVoucherRepository _voucherRepository;
        private IUnitOfWorkScope _unitOfWorkScope;

        public AddCharterInStartReceiptVoucher(IVoucherRepository voucherRepository,IUnitOfWorkScope unitOfWorkScope)
        {
            _voucherRepository = voucherRepository;
            _unitOfWorkScope = unitOfWorkScope;
        }
        
        public void Execute(
            CharterIn charterIn, string receiptNumber,
            int receiptQuantity, decimal receiptFee,
            string receiptWarehouseCode, VoucherSeting voucherSeting, decimal coefficient)
        {

            var voucher = new Voucher();
            voucher.LocalVoucherDate()
                .FinancialVoucherDate(new DateTime())
                .Description(voucherSeting.VoucherMainDescription)
                .ReferenceNo(receiptNumber)
                .LocalVoucherNo("01")
                .VoucherRef(Int64.Parse(voucherSeting.VoucherMainRefDescription))
                .SetReferenceType(ReferenceType.CharterIn)
                .SetCurrency(charterIn.Currency);




            var journalEntry = new JournalEntry();
            journalEntry.IrrAmount(receiptQuantity * receiptFee * coefficient)
                .VoucherRef((journalEntry.IsDebit)
                    ? voucherSeting.VoucherDebitRefDescription
                    : voucherSeting.VoucherCeditRefDescription)
                .Description((journalEntry.IsDebit)
                    ? voucherSeting.VoucherDebitDescription
                    : voucherSeting.VoucherCreditDescription)
                .AccountNo(journalEntry.IsDebit
                    ? voucherSeting.DebitAccounts[0].Code
                    : voucherSeting.CreditAccounts[0].Code)
                .ForeignAmount(receiptFee * receiptQuantity)
                .SetSegmentType(journalEntry.IsDebit 
                    ? SegmentType.Vessel 
                    : SegmentType.Company)
                .SetSegment(segment =>
                         {
                             if (segment.SegmentType.Id == 1)
                             {
                                 segment.Code = receiptWarehouseCode;
                             }
                             else if (segment.SegmentType.Id == 4)
                             {
                                 segment.Code = charterIn.Owner.Code;
                             }
                         }, journalEntry.Segment);

            voucher.JournalEntrieses.Add(journalEntry);
            _voucherRepository.Add(voucher);
            _unitOfWorkScope.Commit();



        }




    }
}
